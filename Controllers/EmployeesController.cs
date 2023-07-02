using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using webapi.Models;

namespace webapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly IConfiguration _configuration;

        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        static readonly string[] scopeRequiredByApi = new string[] { "Employee.R" };

        public EmployeesController(ILogger<EmployeesController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            Random rnd = new Random();
            var testEmployees = new Faker<Employee>()
                .StrictMode(true)
                .RuleFor(o => o.Ntid, f => f.UniqueIndex.ToString())
                .RuleFor(o => o.DisplayName, f => f.Name.FullName())
                .RuleFor(o => o.Email, f => f.Internet.Email());

            var listEmployee = testEmployees.Generate(rnd.Next(1000)).ToList();

            return listEmployee;
        }

        [HttpGet("config")]
        public async Task<string> GetConfigs()
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
