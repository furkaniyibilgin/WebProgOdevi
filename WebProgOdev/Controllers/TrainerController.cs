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
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        public IActionResult List()     //admin lisdt
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            var trainers = _context.Trainers.ToList();
            return View(trainers);
        }
        public IActionResult PublicList()   //user için list
        {
            var trainers = _context.Trainers
                .Where(t => t.IsActive)
                .ToList();

            return View(trainers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            return View(new Trainer());
        }

        [HttpPost]
        public IActionResult Create(Trainer model)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            if (model.StartHour >= model.EndHour)
            {
                ModelState.AddModelError("", "Başlangıç saati bitiş saatinden küçük olmalıdır.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Trainers.Add(model);
            _context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            var trainer = _context.Trainers.FirstOrDefault(t => t.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        [HttpPost]
        public IActionResult Edit(Trainer model)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            if (model.StartHour >= model.EndHour)
            {
                ModelState.AddModelError("", "Başlangıç saati bitiş saatinden küçük olmalıdır.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var trainer = _context.Trainers.FirstOrDefault(t => t.Id == model.Id);
            if (trainer.IsActive == true && model.IsActive == false)
            {
                bool hasUpcoming = _context.Appointments
                    .Any(a => a.TrainerId == trainer.Id && a.EndTime > DateTime.Now);

                if (hasUpcoming)
                {
                    ModelState.AddModelError("", "Bu eğitmenin gelecekte randevusu var. Bu yüzden pasif yapılamaz.");
                    return View(model);
                }
            }

            trainer.FirstName = model.FirstName;
            trainer.LastName = model.LastName;
            trainer.Specialty = model.Specialty;
            trainer.Bio = model.Bio;
            trainer.IsActive = model.IsActive;

            trainer.StartHour = model.StartHour;
            trainer.EndHour = model.EndHour;

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

            var trainer = _context.Trainers.FirstOrDefault(t => t.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return Content("Bu sayfaya sadece admin erişebilir.");
            }

            var trainer = _context.Trainers.FirstOrDefault(t => t.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            bool hasUpcoming = _context.Appointments
                .Any(a => a.TrainerId == id && a.EndTime > DateTime.Now);

            if (hasUpcoming)
            {
                ViewBag.Error = "Bu eğitmenin gelecekte randevusu var. Önce randevuları iptal edin veya eğitmeni pasif yapın.";
                return View("Delete", trainer);
            }

            _context.Trainers.Remove(trainer);
            _context.SaveChanges();

            return RedirectToAction("List");
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
