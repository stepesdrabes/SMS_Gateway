using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SMSgatewayAPI.Models;
using SMSgatewayAPI.Services;

namespace SMSgatewayAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ServiceStatisticsController : Controller
    {
        private readonly ILogger<ServiceStatisticsController> _logger;

        private readonly IStatisticsService _statisticsService;

        public ServiceStatisticsController(ILogger<ServiceStatisticsController> logger,
            IStatisticsService statisticsService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
        }

        [HttpGet, Route("service/statistics")]
        public async Task<ActionResult<ServiceStatistics>> GetStatistics()
        {
            var serviceStatistics = new ServiceStatistics
            {
                SentMessages = await _statisticsService.GetSentMessages(),
                ActiveDevices = await _statisticsService.GetActiveDevices(),
                RegisteredDevices = await _statisticsService.GetRegisteredDevices()
            };

            _logger.Log(LogLevel.Information, "Service statistics were sent through HTTP");

            return Ok(serviceStatistics);
        }
    }
}