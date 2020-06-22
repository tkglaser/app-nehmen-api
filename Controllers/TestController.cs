using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app_nehmen_api.Models;
using app_nehmen_api.Services;
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
        private readonly ICosmosDbService _cosmosDbService;

        public TestController(
            ILogger<TestController> logger,
            IOptions<CosmosConfig> config,
            ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _config = config;
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<object> Get([FromQuery] string userId)
        {
            var token = await _cosmosDbService.GetResourceToken(userId);
            return new
            {
                success = true,
                timestamp = DateTime.Now.ToString(),
                userId = userId,
                token = token
                // config = _config
            };
        }
    }
}
