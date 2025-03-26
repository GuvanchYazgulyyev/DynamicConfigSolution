
#region 2. Test Edilmedi
using ConfigWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Redis için servisleri ekle
builder.Services.AddSingleton<RedisService>();

// Configuration'ı ekleyelim (appsettings.json)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Servisleri ekleyelim
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

#endregion








//using ConfigWebApi.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Configuration'ı ekleyelim (Bu zaten varsayılan olarak yapılıyor)
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//// Servisleri ekle
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Redis için Connection
//builder.Services.AddSingleton<RedisService>();

//var app = builder.Build();

//// Swagger'ı aktif et
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//// Authentication ve Authorization (eğer gereklidiyse)
//app.UseAuthentication();
//app.UseAuthorization();

//// Controller'ları map et
//app.MapControllers();

//// Uygulama çalıştır
//app.Run();
