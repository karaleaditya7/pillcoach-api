using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        private IConfiguration _configuration;
        public UtilityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("health")]
        public string health()
        {
            Console.WriteLine("test test");
            Console.WriteLine("test test");
            Console.WriteLine("test test");
            return "App is healthy";
        }
    }
}
