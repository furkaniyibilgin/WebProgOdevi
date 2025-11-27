using Microsoft.AspNetCore.Mvc;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace WebProgOdev.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string email, string password, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email ve şifre zorunlu.";
                return View();
            }

            var existing = _context.Users.FirstOrDefault(u => u.Email == email);
            if (existing != null)
            {
                ViewBag.Error = "Bu email zaten kayıtlı.";
                return View();
            }

            var user = new User
            {
                Email = email,
                Password = password,   // Basic intro, no hashing
                FirstName = firstName ?? "",
                LastName = lastName ?? "",
                Role = "Member"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // AFTER REGISTER → REDIRECT TO LOGIN PAGE (no auto login)
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email ve şifre zorunlu.";
                return View();
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Email veya şifre hatalı.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
