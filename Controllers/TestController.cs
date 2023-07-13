using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Bogus.Bson;
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
        [HttpGet("config/{value}")]
        public async Task<ActionResult<string>> GetConfig(string value)
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
            var client = new SecretClient(new Uri(_configuration["KeyVaultConfiguration:KeyVaultURL"]), new DefaultAzureCredential(), options);

            KeyVaultSecret secret = await client.GetSecretAsync(value);

            string secretValue = secret.Value;

            return Ok(secretValue);
        }
    }
}
