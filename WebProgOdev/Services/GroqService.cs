using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebProgOdev.Services
{
    public class GroqService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public GroqService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            string? apiKey = _config["Groq:ApiKey"];
            string model = _config["Groq:Model"] ?? "llama-3.3-70b-versatile";

            if (string.IsNullOrWhiteSpace(apiKey))
                return "Groq API key bulunamadı. appsettings içine ekleyin.";

            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var bodyObj = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "Sen bir spor salonu asistanısın. Kısa, madde madde ve Türkçe yaz." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 500
            };

            string jsonBody = JsonSerializer.Serialize(bodyObj);
            req.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var resp = await _http.SendAsync(req);
            string respText = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return "Groq hata verdi: " + resp.StatusCode + "\n" + respText;
            }

            try
            {
                using var doc = JsonDocument.Parse(respText);
                var content = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return content ?? "AI yanıtı boş.";
            }
            catch
            {
                return "AI yanıtı parse edilemedi:\n" + respText;
            }
        }
    }
}
