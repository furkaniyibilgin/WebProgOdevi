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
