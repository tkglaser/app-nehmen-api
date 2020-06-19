using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app_nehmen_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace app_nehmen_api.Controllers
{
    [ApiController]
    [Route("api/v1/test")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IOptions<CosmosConfig> _config;

        public TestController(ILogger<TestController> logger, IOptions<CosmosConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public object Get()
        {
            return new
            {
                success = true,
                timestamp = DateTime.Now.ToString(),
                config = _config
            };
        }
    }
}
