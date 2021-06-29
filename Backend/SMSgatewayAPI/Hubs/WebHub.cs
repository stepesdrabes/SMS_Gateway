using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SMSgatewayAPI.Hubs
{
    public class WebHub : Hub
    {
        private readonly ILogger<WebHub> _logger;

        public WebHub(ILogger<WebHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            // Sends connectionID back to the connected client
            Clients.Client(Context.ConnectionId).SendAsync("ConnectionId", Context.ConnectionId);

            _logger.Log(LogLevel.Information, $"New web client has connected with ConnectionID {Context.ConnectionId}");

            return base.OnConnectedAsync();
        }
    }
}