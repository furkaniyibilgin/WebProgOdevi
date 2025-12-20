using System.Net;

namespace WebProgOdev.Services
{
    public class PollinationsService
    {
        public string GenerateImageUrl(string prompt)
        {
            var encodedPrompt = WebUtility.UrlEncode(prompt);
            return $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=512&height=512";
        }
    }
}
