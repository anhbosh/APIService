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
            try
            {
                var secretClient = new SecretClient(new Uri(_configuration["KeyVaultConfiguration:KeyVaultURL"]),
                                                         new DefaultAzureCredential());

                var secret = await secretClient.GetSecretAsync("https://argonkeyvaultsecrect.vault.azure.net/secrets/databasepassworrd/2dbd1bf8cb444643bcdb8dad52adf9cf");

                return secret.Value.ToString();
            }
            catch (Exception ex)
            {
                var e = ex;
                return ex.Message;
            }
        }
    }
}
