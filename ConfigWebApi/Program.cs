using ConfigWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration'ı ekleyelim (Bu zaten varsayılan olarak yapılıyor)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Servisleri ekle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis için Connection ekleme (Singleton olarak tanımlanabilir)
builder.Services.AddSingleton<RedisService>();

var app = builder.Build();

// Swagger'ı aktif et
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authentication ve Authorization (eğer gereklidiyse)
app.UseAuthentication();
app.UseAuthorization();

// Controller'ları map et
app.MapControllers();

// Uygulama çalıştır
app.Run();
