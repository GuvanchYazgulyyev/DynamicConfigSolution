

using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace ConfigWebApi.Services
{
    public class RedisService
    {
        private static Lazy<ConnectionMultiplexer> _redisLazy;
        private readonly IDatabase _database;
        private string? redisConnectionString;

        // RedisService sınıfı oluşturulurken bağlantı dizesi alınabilir
        public RedisService(IConfiguration configuration)
        {
            // appsettings.json'dan Redis bağlantı dizesini almak
            string redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";

            if (_redisLazy == null)
            {
                _redisLazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConnectionString));
            }

            _database = _redisLazy.Value.GetDatabase();
        }

        public RedisService(string? redisConnectionString)
        {
            this.redisConnectionString = redisConnectionString;
        }

        // Yapılandırma verilerini almak için asenkron metod
        public async Task<List<KeyValuePair<string, string>>> GetConfigurationsAsync(string applicationName)
        {
            var server = _redisLazy.Value.GetServer(_redisLazy.Value.GetEndPoints()[0]);
            var keys = server.Keys(pattern: $"{applicationName}:*").ToList(); // ToList() ekledik

            var configs = new List<KeyValuePair<string, string>>();

            foreach (var key in keys)
            {
                string? value = await _database.StringGetAsync(key);
                if (value != null && value.Contains("IsActive") && value.Contains("1"))
                {
                    configs.Add(new KeyValuePair<string, string>(key.ToString().Replace($"{applicationName}:", ""), value));
                }
            }
            return configs;
        }


        // Yeni bir yapılandırma eklemek için asenkron metod
        public async Task<bool> SetConfigurationAsync(string applicationName, string key, string value)
        {
            return await _database.StringSetAsync($"{applicationName}:{key}", value);
        }
    }
}











//using StackExchange.Redis;

//namespace ConfigWebApi.Services
//{
//    public class RedisService
//    {
//        private static Lazy<ConnectionMultiplexer> _redisLazy;
//        private readonly IDatabase _database;

//        static RedisService()
//        {
//            // Redis bağlantısı, uygulama boyunca sadece bir kez oluşturulur.
//            _redisLazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("localhost:6379"));
//        }

//        public RedisService(string? redisConnectionString)
//        {
//            _database = _redisLazy.Value.GetDatabase();
//        }

//        public async Task<List<KeyValuePair<string, string>>> GetConfigurationsAsync(string applicationName)
//        {
//            var server = _redisLazy.Value.GetServer(_redisLazy.Value.GetEndPoints()[0]);
//            var keys = server.Keys(pattern: $"{applicationName}:*");

//            var configs = new List<KeyValuePair<string, string>>();
//            foreach (var key in keys)
//            {
//                string? value = await _database.StringGetAsync(key);
//                if (value != null)
//                {
//                    // IsActive kontrolü eklenebilir
//                    if (value.Contains("IsActive") && value.Contains("1"))
//                    {
//                        configs.Add(new KeyValuePair<string, string>(key.ToString().Replace($"{applicationName}:", ""), value));
//                    }
//                }
//            }
//            return configs;
//        }

//        public async Task<bool> SetConfigurationAsync(string applicationName, string key, string value)
//        {
//            return await _database.StringSetAsync($"{applicationName}:{key}", value);
//        }
//    }

//}
