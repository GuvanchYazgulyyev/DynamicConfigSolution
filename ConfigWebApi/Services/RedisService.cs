using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigWebApi.Services
{
    public class RedisService : IDisposable
    {
        private Lazy<ConnectionMultiplexer> _redisLazy;
        private readonly IDatabase _database;
        private bool _disposed = false;

        // Constructor: Redis bağlantısını başlatır
        public RedisService(IConfiguration configuration)
        {
            string redisConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";

            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new ArgumentNullException(nameof(redisConnectionString), "Redis bağlantı dizesi boş olamaz.");
            }

            _redisLazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConnectionString));
            _database = _redisLazy.Value.GetDatabase();
        }

        // Constructor: Redis bağlantısı dizisi ile oluşturulmuş alternatif yapı
        public RedisService(string redisConnectionString)
        {
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                throw new ArgumentNullException(nameof(redisConnectionString), "Redis bağlantı dizesi boş olamaz.");
            }

            _redisLazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConnectionString));
            _database = _redisLazy.Value.GetDatabase();
        }

        // Redis bağlantısını test etmek için kullanılan metot
        public async Task TestConnectionAsync()
        {
            try
            {
                if (_redisLazy == null || _redisLazy.Value == null)
                {
                    throw new InvalidOperationException("Redis bağlantısı başlatılamadı.");
                }

                var pingResult = await _database.PingAsync();
                Console.WriteLine($"Redis ping süresi: {pingResult.TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis bağlantı testi hatası: {ex.Message}");
            }
        }

        // Belirli bir uygulamaya ait konfigürasyonları Redis'ten almak için kullanılan metot
        public async Task<List<KeyValuePair<string, string>>> GetConfigurationsAsync(string applicationName)
        {
            try
            {
                if (_redisLazy == null || _redisLazy.Value == null)
                {
                    throw new InvalidOperationException("Redis bağlantısı başlatılamadı. Lütfen bağlantı dizesini kontrol edin.");
                }

                var endPoints = _redisLazy.Value.GetEndPoints();
                if (endPoints == null || !endPoints.Any())
                {
                    throw new InvalidOperationException("Redis uç noktası bulunamadı. Lütfen Redis sunucusunun çalıştığından emin olun.");
                }

                var server = _redisLazy.Value.GetServer(endPoints[0]);
                var keys = server.Keys(pattern: $"{applicationName}:*").ToList();

                var configs = new List<KeyValuePair<string, string>>();

                foreach (var key in keys)
                {
                    try
                    {
                        string? value = await _database.StringGetAsync(key);
                        if (value != null && value.Contains("IsActive") && value.Contains("1"))
                        {
                            configs.Add(new KeyValuePair<string, string>(key.ToString().Replace($"{applicationName}:", ""), value));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Anahtar: {key} okunurken hata oluştu: {ex.Message}");
                    }
                }
                return configs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetConfigurationsAsync hatası: {ex.Message}");
                return new List<KeyValuePair<string, string>>();
            }
        }

        // Konfigürasyon verisini Redis'e yazmak için kullanılan metot
        public async Task<bool> SetConfigurationAsync(string applicationName, string key, string value, string name, string surname, string email, string phone)
        {
            try
            {
                if (_redisLazy == null || _redisLazy.Value == null)
                {
                    throw new InvalidOperationException("Redis bağlantısı başlatılamadı. Lütfen bağlantı dizesini kontrol edin.");
                }

                // Veriyi Redis'e yaz
                await _database.StringSetAsync($"{applicationName}:{key}:value", value);
                await _database.StringSetAsync($"{applicationName}:{key}:name", name);
                await _database.StringSetAsync($"{applicationName}:{key}:surname", surname);
                await _database.StringSetAsync($"{applicationName}:{key}:email", email);
                await _database.StringSetAsync($"{applicationName}:{key}:phone", phone);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SetConfigurationAsync hatası: {ex.Message}");
                return false;
            }
        }


        // IDisposable implementasyonu: Bağlantı kapanışı
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Nesne dispose işlemi
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_redisLazy != null && _redisLazy.IsValueCreated)
                    {
                        _redisLazy.Value.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        // Finalizer
        ~RedisService()
        {
            Dispose(false);
        }
    }
}
