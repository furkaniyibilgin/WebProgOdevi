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
        private bool IsLoggedIn()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            return userId.HasValue && userId.Value > 0;
        }

        private int GetCurrentUserId()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return 0;
            }

            return userId.Value;
        }


        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var services = _context.Services
                .Where(s => s.IsActive)
                .ToList();

            var trainers = _context.Trainers
                .Where(t => t.IsActive)
                .ToList();

            ViewBag.Services = services;
            ViewBag.Trainers = trainers;

            var model = new AppointmentCreateViewModel();
            model.StartTime = DateTime.Now.AddHours(1); //Default deger 1 saat.

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(AppointmentCreateViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var services = _context.Services
                .Where(s => s.IsActive)
                .ToList();

            var trainers = _context.Trainers
                .Where(t => t.IsActive)
                .ToList();

            ViewBag.Services = services;
            ViewBag.Trainers = trainers;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var service = _context.Services
                .FirstOrDefault(s => s.Id == model.ServiceId && s.IsActive);

            var trainer = _context.Trainers
                .FirstOrDefault(t => t.Id == model.TrainerId && t.IsActive);

            if (service == null)
            {
                ModelState.AddModelError("", "Geçerli bir servis seçilmelidir.");
                return View(model);
            }

            if (trainer == null)
            {
                ModelState.AddModelError("", "Geçerli bir eğitmen seçilmelidir.");
                return View(model);
            }

            if (model.StartTime <= DateTime.Now)
            {
                ModelState.AddModelError("", "Randevu zamanı bugünden sonra olmalıdır.");
                return View(model);
            }

            var endTime = model.StartTime.AddMinutes(service.DurationMinutes);


            var appointment = new Appointment();
            appointment.ServiceId = service.Id;
            appointment.TrainerId = trainer.Id;
            appointment.UserId = userId;
            appointment.StartTime = model.StartTime;
            appointment.EndTime = endTime;
            appointment.Price = service.Price;
            appointment.Status = 0;

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("MyAppointments");
        }

        [HttpGet]
        public IActionResult MyAppointments()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.StartTime)
                .ToList();

            return View(appointments);
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
