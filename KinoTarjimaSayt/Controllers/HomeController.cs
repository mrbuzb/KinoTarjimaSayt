using KinoTarjimaMVC.Models;
using KinoTarjimaSayt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text;
using Kino.Repository.Services;
using System.Threading.Tasks;

namespace KinoTarjimaSayt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IMovieRepository _movieRepository;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, IMovieRepository movieRepository)
        {
            _logger = logger;
            _env = env;
            _movieRepository = movieRepository;
        }
        [HttpGet("Subtitle/Translate")]
        public async Task<IActionResult> Translate(double time, string title)
        {
            var translatedText = await GetTranslatedTextAsync(time, title);
            return Content(translatedText);
        }

        public async Task<IActionResult> Index()
        {

            var moviesFromDb = await _movieRepository.GetAllMovies();

            var movies = moviesFromDb.Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                VideoPath = m.VideoPath,
                SubtitlePath = m.SubtitlePath
            }).ToList();

            return View(movies);
        }


        public IActionResult Watch(string title)
        {
            var movie = new Movie
            {
                Title = title,
                VideoPath = $"/videos/{title}.mp4",
                SubtitlePath = $"/subtitles/{title}.vtt"
            };

            return View(movie);
        }



        private async Task<string> GetTranslatedTextAsync(double time, string title)
        {
            var subtitleText = await GetSubtitleTextByTimeAsync(time, title);
            if (string.IsNullOrEmpty(subtitleText))
                return "";

            return await TranslateWithLingvanexAsync(subtitleText);
        }


        [HttpGet]
        public async Task<IActionResult> Original(double time, string title)
        {
            var originalText = await GetSubtitleTextByTimeAsync(time, title);
            return Content(originalText);
        }



        private async Task<string> GetSubtitleTextByTimeAsync(double time, string title)
        {
            // ✅ Foydalanuvchi yuborgan title bo‘yicha fayl nomini olamiz
            var fileName = $"{title}.vtt";
            var subtitlePath = Path.Combine(_env.WebRootPath, "subtitles", fileName);

            if (!System.IO.File.Exists(subtitlePath))
                return "";

            var subtitleContent = await System.IO.File.ReadAllTextAsync(subtitlePath);

            var blocks = Regex.Split(subtitleContent.Trim(), @"\r?\n\r?\n");

            foreach (var block in blocks)
            {
                var lines = block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2) continue;

                var timeLine = lines[0].Contains("-->") ? lines[0] : lines[1];
                var match = Regex.Match(timeLine, @"(\d{2}:\d{2}:\d{2}[.,]\d{3})\s*-->\s*(\d{2}:\d{2}:\d{2}[.,]\d{3})");
                if (!match.Success) continue;

                var start = match.Groups[1].Value.Replace(',', '.');
                var end = match.Groups[2].Value.Replace(',', '.');

                if (IsTimeInRange(time, start, end))
                {
                    var textLines = lines.SkipWhile(l => !l.Contains("-->")).Skip(1);
                    return string.Join(" ", textLines).Trim();
                }
            }

            return "";
        }

        private bool IsTimeInRange(double currentTime, string start, string end)
        {
            var startTime = TimeSpan.Parse(start);
            var endTime = TimeSpan.Parse(end);
            var time = TimeSpan.FromSeconds(currentTime);

            return time >= startTime && time <= endTime;
        }

        private async Task<string> TranslateWithLingvanexAsync(string subtitleText)
        {
            using var client = new HttpClient();

            var apiKey = "a_4oAaTM61QSiippfrPMUR3btqfFL4aHsvSBwCbMhknhmc3os1wUf4Yv5FKIR2bMwX87C3BhYeNQDQRmmS"; // Diqqat: API kalitni ochiq saqlamang

            var requestBody = new
            {
                from = "en",
                to = "uz",
                text = subtitleText,
                platform = "api"
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await client.PostAsync("https://api-b2b.backenster.com/b1/api/v3/translate", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"Xato: {response.StatusCode}\n{responseBody}";
            }

            var json = JsonDocument.Parse(responseBody);
            var translatedText = json.RootElement.GetProperty("result").GetString();
            return translatedText;
        }
    }
}
