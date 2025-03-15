using System.Diagnostics;
using ConfigWebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConfigWebUI.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _httpClient.GetAsync("http://localhost:5000/api/configuration/SERVICE-A");
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
        var response = await _httpClient.PostAsync("http://localhost:5000/api/configuration", content);

        return RedirectToAction("Index");
    }
}
