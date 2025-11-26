using Microsoft.AspNetCore.Mvc;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System.Linq;

namespace WebProgOdev.Controllers
{
    public class TrainerController : Controller
    {
        private readonly AppDbContext _context;

        public TrainerController(AppDbContext context)
        {
            _context = context;
        }

        public ActionResult Egitmen(int id = 1)
        {
            var trainer = _context.Trainers.FirstOrDefault(t => t.Id == id);

            if (trainer == null)
            {
                return Content("No trainer found.");
            }

            return View();
        }
    }
}
