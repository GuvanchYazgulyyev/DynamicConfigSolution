using ConfigWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConfigWebUI.Controllers
{
    public class VitrinController : Controller
    {
        private readonly HttpClient _httpClient;

        public VitrinController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("http://configwebapi:5000/api/configuration/SERVICE-A");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<ConfigItem>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var configList = JsonConvert.DeserializeObject<List<ConfigItem>>(jsonString);
            return View(configList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ConfigItem model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");

            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("http://configwebapi:5000/api/configuration", content);

            if (response.IsSuccessStatusCode)
                TempData["Message"] = "Konfigürasyon başarıyla eklendi!";
            else
                TempData["Message"] = "Konfigürasyon eklenirken bir hata oluştu.";

            return RedirectToAction("Index");
        }
    }
}
