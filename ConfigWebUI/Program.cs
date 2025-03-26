var builder = WebApplication.CreateBuilder(args);

// Servisleri ekle
builder.Services.AddControllersWithViews();

// HTTP Client için ConfigApi base URL ayarını yap
var configApiUrl = builder.Configuration.GetValue<string>("ConfigApiUrl") ?? "http://config_api:5000"; // Docker içindeki api adı

builder.Services.AddHttpClient("ConfigApi", client =>
{
    client.BaseAddress = new Uri(configApiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Controller'ları map et
app.MapControllerRoute(name: "default", pattern: "{controller=Vitrin}/{action=Index}/{id?}");

app.Run();
