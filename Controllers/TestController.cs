using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IConfiguration _configuration;


        public TestController(ILogger<EmployeesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET api/<TestController>/5
        [HttpGet("localconfig/{value}")]
        public string GetLocalConfig(string value)
        {
            return _configuration[value].ToString();
        }

        // GET api/<TestController>/5
        [HttpGet("config")]
        public async Task<string> GetConfig()
        {

            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
            };
            var client = new SecretClient(new Uri("https://argonkeyvaultsecrect.vault.azure.net/"), new DefaultAzureCredential(), options);

            KeyVaultSecret secret = client.GetSecret("databasepassworrd");

            string secretValue = secret.Value;

            return secretValue;
        }
    }
}
