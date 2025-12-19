using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GymManagementApp.Controllers
{

    public class YapayZekaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string ApiKey = "AIzaSyDpIGEWWUJS7lBydCh_zzIsukixnwVTEog";
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

        public YapayZekaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProgramOlustur(int yas, int kilo, int boy, string cinsiyet, string hedef)
        {
            try
            {
                // PROMT
                string prompt = $"Spor hocasısın. {yas} yaş, {kilo} kg, {boy} cm, {cinsiyet}, hedef: {hedef}. 1 haftalık program yaz. HTML formatında ver (h3, ul, li). Kod bloğu kullanma.";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var client = _httpClientFactory.CreateClient();

                // SEND
                var response = await client.PostAsync($"{ApiUrl}?key={ApiKey}", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonNode = JsonNode.Parse(responseString);
                    string gelenMetin = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                    // SHOW RESULT
                    ViewBag.Sonuc = gelenMetin?.Replace("```html", "")?.Replace("```", "");
                }
                else
                {                 
                    ViewBag.Hata = $"GOOGLE HATASI: {response.StatusCode}. Mesaj: {responseString}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "SİSTEM HATASI: " + ex.Message;
            }

            ViewBag.Yas = yas; ViewBag.Kilo = kilo; ViewBag.Boy = boy;
            return View("Index");
        }
    }
}