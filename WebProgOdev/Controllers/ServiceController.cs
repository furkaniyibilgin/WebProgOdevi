using Microsoft.AspNetCore.Mvc;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System.Linq;

namespace WebProgOdev.Controllers
{
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        public IActionResult List()
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            var services = _context.Services.ToList();
            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            return View(new Service());
        }

        [HttpPost]
        public IActionResult Create(Service model)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Services.Add(model);
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            var service = _context.Services.FirstOrDefault(s => s.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            var service = _context.Services.FirstOrDefault(s => s.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            _context.SaveChanges();

            return RedirectToAction("List");
        }
        public ActionResult Servisler(int id = 1)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == id);

            if (service == null)
            {
                return Content("No service found.");
            }

            return View();
        }
    }
}
