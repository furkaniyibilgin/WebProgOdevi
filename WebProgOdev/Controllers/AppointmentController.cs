using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System.Linq;

namespace WebProgOdev.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        public ActionResult ViewOne(int id = 1)
        {
            var appointment = _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return Content("No appointment found.");
            }

            return View(appointment);
        }
    }
}
