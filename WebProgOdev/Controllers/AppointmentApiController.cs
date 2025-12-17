using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System;
using System.Linq;

namespace WebProgOdev.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/appointmentsapi
        [HttpGet]
        public IActionResult GetAll(string? status, int? trainerId, int? userId, string? date)
        {
            var query = _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .AsQueryable();

            // trainer filter
            if (trainerId.HasValue && trainerId.Value > 0)
            {
                query = query.Where(a => a.TrainerId == trainerId.Value);
            }

            // user filter
            if (userId.HasValue && userId.Value > 0)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            // status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                AppointmentStatus parsedStatus;
                bool ok = Enum.TryParse(status, true, out parsedStatus);
                if (ok)
                {
                    query = query.Where(a => a.Status == parsedStatus);
                }
            }

            // date filter (yyyy-MM-dd)
            if (!string.IsNullOrWhiteSpace(date))
            {
                DateTime parsedDate;
                bool ok = DateTime.TryParse(date, out parsedDate);
                if (ok)
                {
                    var d = parsedDate.Date;
                    query = query.Where(a => a.StartTime.Date == d);
                }
            }

            var result = query
                .OrderByDescending(a => a.StartTime)
                .Select(a => new
                {
                    a.Id,
                    a.ServiceId,
                    ServiceName = a.Service.Name,
                    a.TrainerId,
                    TrainerName = a.Trainer.FirstName + " " + a.Trainer.LastName,
                    a.UserId,
                    UserEmail = a.User.Email,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    a.Price,
                    Status = a.Status.ToString()
                })
                .ToList();

            return Ok(result);
        }
    }
}
