using StackExchange.Redis;
using System.Collections.Concurrent;

namespace ConfigurationLibrary.Operation
{
    public class ConfigurationReader : IDisposable
    {
        private readonly string _applicationName;
        private readonly string _connectionString;
        private readonly int _refreshInterval;
        private readonly ConcurrentDictionary<string, string> _configurations;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cts;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ConfigurationReader(string applicationName, string connectionString, int refreshInterval)
        {
            _applicationName = applicationName;
            _connectionString = connectionString;
            _refreshInterval = refreshInterval;
            _configurations = new ConcurrentDictionary<string, string>();
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(refreshInterval));
            _cts = new CancellationTokenSource();

            // Redis bağlantısını oluştur
            _redis = ConnectionMultiplexer.Connect(_connectionString);
            _database = _redis.GetDatabase();

            // Güncellemeleri başlat
            Task.Run(() => StartRefreshingAsync(_cts.Token));
        }

        private async Task StartRefreshingAsync(CancellationToken cancellationToken)
        {
            while (await _timer.WaitForNextTickAsync(cancellationToken))
            {
                await RefreshConfigurationsAsync();
            }
        }

        private async Task RefreshConfigurationsAsync()
        {
            Console.WriteLine($"[{DateTime.Now}] Konfigürasyon verileri Redis'ten güncelleniyor...");

            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            var keys = server.Keys(pattern: $"{_applicationName}:*");

            foreach (var key in keys)
            {
                string? value = await _database.StringGetAsync(key);
                if (value != null)
                {
                    string configKey = key.ToString().Replace($"{_applicationName}:", "");
                    _configurations[configKey] = value;
                }
            }
        }

        public T GetValue<T>(string key)
        {
            if (_configurations.TryGetValue(key, out var value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            throw new KeyNotFoundException($"Key {key} bulunamadı.");
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _redis.Dispose();
        }
    }
}
