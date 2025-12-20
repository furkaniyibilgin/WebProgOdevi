using Microsoft.AspNetCore.Mvc;
using WebProgOdev.Data;
using WebProgOdev.Models;
using WebProgOdev.Services;
using System.Linq;
using System.Net;
using System.Text;

namespace WebProgOdev.Controllers
{
    public class AiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly GroqService _groq;

        public AiController(AppDbContext context, GroqService groq)
        {
            _context = context;
            _groq = groq;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new AiViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(AiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // DB'den aktif servis ve eğitmenleri al
            var services = _context.Services.Where(s => s.IsActive).ToList();
            var trainers = _context.Trainers.Where(t => t.IsActive).ToList();

            // 1) Görsel üretimi (Pollinations)
            string imgPrompt = model.Goal;
            if (!string.IsNullOrWhiteSpace(model.Notes))
                imgPrompt += " " + model.Notes;

            imgPrompt += " fitness gym illustration, simple, modern";
            string encodedPrompt = WebUtility.UrlEncode(imgPrompt);
            model.ImageUrl = $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=768&height=512&seed=42";

            // 2) Metin önerisi (Groq - gerçek LLM)
            var sb = new StringBuilder();
            sb.AppendLine("Kullanıcının hedefine göre uygun servis ve eğitmen öner.");
            sb.AppendLine("Kısa, madde madde cevap ver. Türkçe yaz.");
            sb.AppendLine();
            sb.AppendLine("Kullanıcı hedefi: " + model.Goal);
            if (!string.IsNullOrWhiteSpace(model.Notes))
                sb.AppendLine("Notlar: " + model.Notes);

            sb.AppendLine();
            sb.AppendLine("Mevcut Servisler:");
            foreach (var s in services)
                sb.AppendLine("- " + s.Name + " (Fiyat: " + s.Price + ", Süre: " + s.DurationMinutes + " dk)");

            sb.AppendLine();
            sb.AppendLine("Mevcut Eğitmenler:");
            foreach (var t in trainers)
                sb.AppendLine("- " + t.FirstName + " " + t.LastName + " (Uzmanlık: " + t.Specialty + ")");

            sb.AppendLine();
            sb.AppendLine("Çıktı formatı:");
            sb.AppendLine("1) Önerilen Servis: ...");
            sb.AppendLine("2) Önerilen Eğitmen: ...");
            sb.AppendLine("3) Neden: ...");

            string prompt = sb.ToString();
            model.Result = await _groq.GenerateAsync(prompt);

            return View(model);
        }
    }
}
