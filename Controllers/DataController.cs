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
    [Route("api/v1/data")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly ICosmosDbService _cosmosDbService;

        public DataController(
            ILogger<DataController> logger,
            IOptions<CosmosConfig> config,
            ICosmosDbService cosmosDbService)
        {
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet("token")]
        public async Task<object> Get([FromQuery] string userId)
        {
            var token = await _cosmosDbService.GetResourceToken(userId);
            return new
            {
                timestamp = DateTime.Now.ToString(),
                userId = userId,
                token = token
            };
        }
    }
}
