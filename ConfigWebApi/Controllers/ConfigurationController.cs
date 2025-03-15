using Microsoft.Extensions.Configuration;
using ConfigWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigWebApi.Controllers
{
    [ApiController]
    [Route("api/configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly RedisService _configService;
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
            var redisConnectionString = _configuration.GetValue<string>("Redis:ConnectionString");
            _configService = new RedisService(redisConnectionString);
        }

        [HttpGet("{applicationName}")]
        public async Task<IActionResult> GetConfigurations(string applicationName)
        {
            var configs = await _configService.GetConfigurationsAsync(applicationName);
            return Ok(configs);
        }

        [HttpPost]
        public async Task<IActionResult> SetConfiguration([FromBody] ConfigRequest request)
        {
            var success = await _configService.SetConfigurationAsync(request.ApplicationName, request.Key, request.Value);
            if (success)
                return Ok(new { message = "Configuration saved successfully!" });

            return BadRequest(new { message = "Failed to save configuration." });
        }
    }

    public class ConfigRequest
    {
        public string ApplicationName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
