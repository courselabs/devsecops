using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Numbers.Web.Services
{
    public class RandomNumberService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public RandomNumberService(IConfiguration config, ILogger<RandomNumberService> logger)
        {
            _config = config;
            _logger = logger;
            _logger.LogInformation($"Using API at: {_config["RngApi:Url"]}");
        }

        public int GetNumber()
        {
            var client = new RestClient(_config["RngApi:Url"]);
            var request = new RestRequest();
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                throw new Exception("Service call failed");
            }
            return int.Parse(response.Content);
        }
    }
}
