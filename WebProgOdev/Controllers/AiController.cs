using Microsoft.AspNetCore.Mvc;
using WebProgOdev.Data;
using WebProgOdev.Models;
using System.Linq;
using System.Text;
using System.Net;

namespace WebProgOdev.Controllers
{
    public class AiController : Controller
    {
        private readonly AppDbContext _context;

        public AiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AiViewModel());
        }

        [HttpPost]
        public IActionResult Index(AiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // DB'den aktif servis ve eğitmenleri al
            var services = _context.Services.Where(s => s.IsActive).ToList();
            var trainers = _context.Trainers.Where(t => t.IsActive).ToList();

            // 1) Görsel üretimi (Pollinations - API key yok)
            // Prompt'u Goal + Notes üzerinden biraz zenginleştiriyoruz
            string imgPrompt = model.Goal;
            if (!string.IsNullOrWhiteSpace(model.Notes))
            {
                imgPrompt += " " + model.Notes;
            }

            // "spor salonu / fitness" gibi daha doğru görseller için ek kelime
            imgPrompt += " fitness gym illustration, simple, modern";

            // Pollinations URL
            string encodedPrompt = WebUtility.UrlEncode(imgPrompt);
            model.ImageUrl = $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=768&height=512&seed=42";

            // 2) Basit "AI önerisi" (noob, if/else, deterministic)
            // Ama DB'deki servis/eğitmen listesini kullanarak gerçekçi sonuç veriyoruz
            string goalLower = (model.Goal ?? "").ToLowerInvariant();
            string notesLower = (model.Notes ?? "").ToLowerInvariant();

            // Basit eşleştirme anahtarları
            bool wantLoseWeight = goalLower.Contains("kilo") || goalLower.Contains("zayıf") || goalLower.Contains("yağ");
            bool wantMuscle = goalLower.Contains("kas") || goalLower.Contains("güç") || goalLower.Contains("hacim");
            bool wantFlex = goalLower.Contains("esnek") || goalLower.Contains("yoga") || goalLower.Contains("pilates");
            bool hasKneePain = notesLower.Contains("diz") || notesLower.Contains("ağrı") || notesLower.Contains("sakat");

            // Servis önerisi: isim içinde geçen kelime ile yakala (basit)
            // Yoksa ilk aktif servisi seç
            Service? pickedService = null;

            if (wantLoseWeight)
            {
                pickedService = services.FirstOrDefault(s => (s.Name ?? "").ToLower().Contains("kardiyo"))
                               ?? services.FirstOrDefault(s => (s.Name ?? "").ToLower().Contains("bisiklet"))
                               ?? services.FirstOrDefault();
            }
            else if (wantMuscle)
            {
                pickedService = services.FirstOrDefault(s => (s.Name ?? "").ToLower().Contains("kuvvet"))
                               ?? services.FirstOrDefault(s => (s.Name ?? "").ToLower().Contains("ağırlık"))
                               ?? services.FirstOrDefault();
            }
            else if (wantFlex)
            {
                pickedService = services.FirstOrDefault(s => (s.Name ?? "").ToLower().Contains("pilates"))
                               ?? services.FirstOrDefault(s => (s.Name ?? "").ToLower().Contains("yoga"))
                               ?? services.FirstOrDefault();
            }
            else
            {
                pickedService = services.FirstOrDefault();
            }

            // Eğitmen önerisi: Specialty içinde geçen kelime ile yakala
            Trainer? pickedTrainer = null;

            if (wantLoseWeight)
            {
                pickedTrainer = trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("kardiyo"))
                               ?? trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("yağ"));
            }
            else if (wantMuscle)
            {
                pickedTrainer = trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("kuvvet"))
                               ?? trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("ağırlık"));
            }
            else if (wantFlex)
            {
                pickedTrainer = trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("pilates"))
                               ?? trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("yoga"));
            }

            // Diz ağrısı vs varsa "hafif/rehabilitasyon" gibi bir eğitmen yakalamaya çalış
            if (hasKneePain)
            {
                var rehabTrainer = trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("rehab"))
                                 ?? trainers.FirstOrDefault(t => (t.Specialty ?? "").ToLower().Contains("hafif"));
                if (rehabTrainer != null)
                {
                    pickedTrainer = rehabTrainer;
                }
            }

            // Eğer hala eğitmen bulunamadıysa, ilk aktif eğitmeni öner
            if (pickedTrainer == null)
            {
                pickedTrainer = trainers.FirstOrDefault();
            }

            // Sonucu üret
            var sb = new StringBuilder();
            sb.AppendLine("1) Önerilen Servis: " + (pickedService != null ? pickedService.Name : "Servis bulunamadı"));
            if (pickedService != null)
            {
                sb.AppendLine("   - Süre: " + pickedService.DurationMinutes + " dk");
                sb.AppendLine("   - Fiyat: " + pickedService.Price);
            }

            sb.AppendLine();
            sb.AppendLine("2) Önerilen Eğitmen: " + (pickedTrainer != null ? (pickedTrainer.FirstName + " " + pickedTrainer.LastName) : "Eğitmen bulunamadı"));
            if (pickedTrainer != null)
            {
                sb.AppendLine("   - Uzmanlık: " + pickedTrainer.Specialty);
                sb.AppendLine("   - Çalışma: " + pickedTrainer.StartHour.ToString("00") + ":00 - " + pickedTrainer.EndHour.ToString("00") + ":00");
            }

            sb.AppendLine();
            sb.AppendLine("3) Neden:");
            if (wantLoseWeight)
            {
                sb.AppendLine("- Yağ yakımı için düzenli kardiyo + kontrollü beslenme iyi sonuç verir.");
            }
            if (wantMuscle)
            {
                sb.AppendLine("- Kas kazanımı için kuvvet antrenmanı ve progressive overload önemlidir.");
            }
            if (wantFlex)
            {
                sb.AppendLine("- Esneklik için düzenli mobilite/pilates/yoga çalışmaları uygundur.");
            }
            if (hasKneePain)
            {
                sb.AppendLine("- Diz ağrısı olduğunda düşük etkili egzersizler ve kontrollü program önerilir.");
            }
            if (!wantLoseWeight && !wantMuscle && !wantFlex)
            {
                sb.AppendLine("- Hedefinize göre temel bir başlangıç programı önerildi.");
            }

            model.Result = sb.ToString();
            return View(model);
        }
    }
}
