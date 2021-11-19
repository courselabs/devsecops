using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Numbers.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RngController : ControllerBase
    {
        private static Random _Random = new Random();
        private static int _CallCount;

        private readonly IConfiguration _config;
        private readonly ILogger<RngController> _logger;

        public RngController(IConfiguration config, ILogger<RngController> logger)
        {
            _config = config;
            _logger = logger;
            if (_CallCount == 0)
            {
                _logger.LogInformation("Random number generator initialized");
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            _CallCount++;
            if (_config.GetValue<bool>("Rng:FailAfter:Enabled") && _CallCount > _config.GetValue<int>("Rng:FailAfter:CallCount"))
            {
                if (_config["Rng:FailAfter:Action"] == "Exit")
                {
                    _logger.LogError($"FailAfter enabled. Call: {_CallCount}. Exiting.");
                    Environment.Exit(100);
                }
                _logger.LogWarning($"FailAfter enabled. Call: {_CallCount}. Going unhealthy.");
                Status.Healthy = false;
            }

            if (Status.Healthy)
            {
                var min = _config.GetValue<int>("Rng:Range:Min");
                var max = _config.GetValue<int>("Rng:Range:Max");
                var n = _Random.Next(min, max);
                _logger.LogDebug($"Call: {_CallCount}. Returning random number: {n}, from min: {min}, max: {max}");
                return Ok(n);
            }
            else
            {
                _logger.LogWarning("Unhealthy!");
                return StatusCode(500);
            }
        }
    }
}
