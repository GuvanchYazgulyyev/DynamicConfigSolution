var builder = WebApplication.CreateBuilder(args);

// Servisleri ekle
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Çevresel değişkenlere göre Redis veya diğer servis URL'lerini ayarla
var configApiUrl = builder.Configuration.GetValue<string>("ConfigApiUrl") ?? "http://configwebapi:5000";

// HTTP Client için base URL ayarını yap
builder.Services.AddHttpClient("ConfigApi", client => client.BaseAddress = new Uri(configApiUrl));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Vitrin}/{action=Index}/{id?}");
app.Run();
