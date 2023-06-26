using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // The Web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        static readonly string[] scopeRequiredByApi = new string[] { "Employee.R" };

        public EmployeesController(ILogger<EmployeesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);

            Random rnd = new Random();
            var testEmployees = new Faker<Employee>()
                .StrictMode(true)
                .RuleFor(o => o.Ntid, f => f.UniqueIndex.ToString())
                .RuleFor(o => o.DisplayName, f => f.Name.FullName())
                .RuleFor(o => o.Email, f => f.Internet.Email());

            return testEmployees.Generate(rnd.Next(1000)).ToList();
        }
    }
}
