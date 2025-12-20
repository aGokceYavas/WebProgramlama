using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace GymManagementApp.Controllers
{
    public class YapayZekaController : Controller
    {
        // API KEY
        private const string ApiKey = "...";

        // MODEL: gemini-2.0-flash
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent";

        public IActionResult Index()
        {
            return View();
        }

        // program oluştur
        [HttpPost]
        public async Task<IActionResult> ProgramOlustur(int yas, int kilo, int boy, string cinsiyet, string hedef)
        {
            try
            {
                string prompt = $"Spor hocasısın. {yas} yaş, {kilo} kg, {boy} cm, {cinsiyet}, hedef: {hedef}. 1 haftalık antrenman programı yaz. HTML (ul, li, h3) formatında ver. Asla markdown kullanma.";
                string sonuc = await GeminiCagir(prompt, null);
                ViewBag.Sonuc = sonuc;
            }
            catch (Exception ex) { ViewBag.Hata = "Hata: " + ex.Message; }

            ViewBag.Yas = yas; ViewBag.Kilo = kilo; ViewBag.Boy = boy;
            ViewBag.AktifSekme = "program";
            return View("Index");
        }

        // görsel analiz (yemek)
        [HttpPost]
        public async Task<IActionResult> YemekAnalizi(IFormFile resimDosyasi)
        {
            ViewBag.AktifSekme = "yemek";

            if (resimDosyasi == null || resimDosyasi.Length == 0)
            {
                ViewBag.Hata = "Lütfen fotoğraf yükleyin.";
                return View("Index");
            }

            try
            {

                string base64Resim;
                using (var ms = new MemoryStream())
                {
                    await resimDosyasi.CopyToAsync(ms);
                    base64Resim = Convert.ToBase64String(ms.ToArray());
                }
                ViewBag.YuklenenResim = $"data:{resimDosyasi.ContentType};base64,{base64Resim}";


                string prompt = "Bu yemek fotoğrafını analiz et. Sporculara uygun healthy swap (sağlıklı değişim) versiyonunu düşün.\n" +
                                "Cevabı şu formatta ver:\n" +
                                "VISUAL: [SADECE yeni yemeğin İngilizce görsel tasviri. Kısa tut.]\n" +
                                "***AYIRAC***\n" +
                                "TEXT: [Türkçe olarak; 1. Eski yemek neydi? 2. Yeni yemek ne? 3. Kalori farkı? HTML (ul, li) formatında yaz]";

                //gönder
                string sonuc = await GeminiCagir(prompt, base64Resim);

                // Cevabı işle
                string imagePrompt = "healthy delicious food plate";
                string aciklama = sonuc;

                if (!string.IsNullOrEmpty(sonuc) && sonuc.Contains("***AYIRAC***"))
                {
                    var parts = sonuc.Split(new[] { "***AYIRAC***" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        
                        string rawPrompt = parts[0].Replace("VISUAL:", "").Trim();
                        
                        imagePrompt = Regex.Replace(rawPrompt, "[^a-zA-Z0-9, ]", "");

                        aciklama = parts[1].Replace("TEXT:", "").Trim();
                    }
                }

                // çiz
                if (imagePrompt.Length > 200) imagePrompt = imagePrompt.Substring(0, 199);
                string encodedPrompt = System.Net.WebUtility.UrlEncode(imagePrompt);

                var seed = new Random().Next(1, 99999);

                ViewBag.UretilenResim = $"https://image.pollinations.ai/prompt/{encodedPrompt}?width=800&height=600&seed={seed}&model=turbo";

                ViewBag.YemekAciklamasi = aciklama.Replace("```html", "").Replace("```", "");
            }
            catch (Exception ex) { ViewBag.Hata = "Analiz Hatası: " + ex.Message; }

            return View("Index");
        }

        private async Task<string> GeminiCagir(string prompt, string base64Image)
        {
            using (var client = new HttpClient())
            {
                object requestBody;
                if (base64Image == null)
                    requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                else
                    requestBody = new { contents = new[] { new { parts = new List<object> { new { text = prompt }, new { inline_data = new { mime_type = "image/jpeg", data = base64Image } } } } } };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{ApiUrl}?key={ApiKey}", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonNode = JsonNode.Parse(responseString);
                    return jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? "Cevap yok.";
                }
                throw new Exception("API Hatası: " + response.StatusCode);
            }
        }
    }
}