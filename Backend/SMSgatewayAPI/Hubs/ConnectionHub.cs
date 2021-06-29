using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SMSgatewayAPI.Managers;

namespace SMSgatewayAPI.Hubs
{
    public class ConnectionHub : Hub
    {
        private readonly ILogger<ConnectionHub> _logger;

        private readonly IHubContext<WebHub> _webHub;

        private readonly DevicesManager _devicesManager;
        private readonly SessionTokenManager _tokenManager;

        public ConnectionHub(ILogger<ConnectionHub> logger, IHubContext<WebHub> webHub, DevicesManager devicesManager,
            SessionTokenManager tokenManager)
        {
            _logger = logger;
            _webHub = webHub;
            _devicesManager = devicesManager;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// Gets called when a new devices connects
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // Gets the connection ID
            var connectionId = Context.ConnectionId;

            // Gets HTTP headers with a Token and a DeviceID 
            var httpContext = Context.GetHttpContext();
            var token = httpContext.Request.Headers["TOKEN"].ToString();
            var deviceId = httpContext.Request.Headers["DEVICE_ID"].ToString();

            // Checks if the tokens match
            if (!_tokenManager.DoesTokenMatch(deviceId, token))
            {
                // Disconnect the device, because it sent wrong token
                Context.Abort();

                _logger.Log(LogLevel.Warning, $"Device with ID {deviceId} tried to log in with wrong token!");

                return;
            }

            _devicesManager.AddDevice(deviceId, connectionId);

            _logger.Log(LogLevel.Information, $"Device with ID {deviceId} connected");

            // Notifies all web client about new device connecting
            await _webHub.Clients.All.SendAsync("DevicesChange");
            await _webHub.Clients.All.SendAsync("StatisticsChange");

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Gets called when a device disconnects
        /// </summary>
        /// <param name="exception">Why the device disconnected</param>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Gets the connection ID
            var connectionId = Context.ConnectionId;

            // Gets DeviceID from the ID of the connection
            var deviceId = _devicesManager.GetDeviceIdFromConnection(connectionId);

            // Since the device has disconnected, remove them from the managers
            _tokenManager.RemoveDeviceToken(deviceId);
            _devicesManager.RemoveDevice(deviceId);

            _logger.Log(LogLevel.Information, $"Device with ID {deviceId} disconnected");

            // Notifies all web client about device disconnection
            await _webHub.Clients.All.SendAsync("DevicesChange");
            await _webHub.Clients.All.SendAsync("StatisticsChange");

            await base.OnDisconnectedAsync(exception);
        }
    }
}