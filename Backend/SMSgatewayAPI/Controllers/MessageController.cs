using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DAL.Entities;
using DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SMSgatewayAPI.Hubs;
using SMSgatewayAPI.Managers;
using SMSgatewayAPI.Models;
using SMSgatewayAPI.Services;

namespace SMSgatewayAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;

        private readonly IHubContext<ConnectionHub> _hub;
        private readonly IHubContext<WebHub> _webHub;

        private readonly IDeviceService _deviceService;
        private readonly DevicesManager _devicesManager;
        private readonly IMessageService _messageService;

        private readonly SessionTokenManager _tokenManager;

        public MessageController(ILogger<MessageController> logger, IHubContext<ConnectionHub> hub,
            IHubContext<WebHub> webHub, IDeviceService deviceService, DevicesManager devicesManager,
            IMessageService messageService, SessionTokenManager tokenManager)
        {
            _logger = logger;
            _hub = hub;
            _webHub = webHub;
            _deviceService = deviceService;
            _devicesManager = devicesManager;
            _messageService = messageService;
            _tokenManager = tokenManager;
        }

        /// <summary>
        /// Gets all messages
        /// Method - GET /api/messages
        /// </summary>
        /// <returns>List of all messages</returns>
        [HttpGet]
        [Route("messages")]
        public async Task<ActionResult<List<MessageModel>>> GetAllMessages()
        {
            var messages = await _messageService.GetAllMessages();

            _logger.Log(LogLevel.Debug,
                $"List of all messages [{messages.Count}] was sent thought HTTP");

            // Returns the list of all messages
            return Ok(messages);
        }

        /// <summary>
        /// Gets all messages sent by the device
        /// Method - GET /api/messages/{DeviceID}?={token}
        /// </summary>
        /// <param name="deviceId">ID of the device</param>
        /// <param name="token">Session token</param>
        /// <returns>List of all messages</returns>
        [HttpGet]
        [Route("messages/{deviceId}")]
        public async Task<ActionResult<List<MessageModel>>> GetMessages(string deviceId, string token)
        {
            if (!_tokenManager.DoesTokenMatch(deviceId, token))
            {
                _logger.Log(LogLevel.Warning, $"Device with ID {deviceId} tried to get all messages with wrong token");

                return Forbid();
            }

            var messages = await _messageService.GetMessages(deviceId);

            _logger.Log(LogLevel.Debug,
                $"List of all messages [{messages.Count}] was sent to a device with ID {deviceId}");

            // Returns the list of all messages with certain device ID
            return Ok(messages);
        }

        /// <summary>
        /// Gets a message with certain message ID
        /// Method - GET /api/message/{DeviceID}/{MessageID}?={token}
        /// </summary>
        /// <param name="deviceId">ID of the device</param>
        /// <param name="messageId">ID of the message</param>
        /// <param name="token">Session token</param>
        /// <returns>Message with certain ID</returns>
        [HttpGet]
        [Route("message/{deviceId}/{messageId}")]
        public async Task<ActionResult<MessageModel>> GetMessage(string deviceId, ulong messageId, string token)
        {
            if (!_tokenManager.DoesTokenMatch(deviceId, token))
            {
                _logger.Log(LogLevel.Warning, $"Device with ID {deviceId} tried to get a message with wrong token");

                return Forbid();
            }

            _logger.Log(LogLevel.Debug, $"Message [{messageId}] was sent to a device with ID {deviceId}");

            // Returns the list of all messages with certain device ID
            return Ok(await _messageService.GetMessage(messageId));
        }

        [HttpPut]
        [Route("message/{deviceId}/{messageId}")]
        public async Task<ActionResult> UpdateMessage(string deviceId, ulong messageId, string token,
            [FromBody] MessageUpdateModel model)
        {
            if (!_tokenManager.DoesTokenMatch(deviceId, token))
            {
                _logger.Log(LogLevel.Warning, $"Device with ID {deviceId} tried to update a message with wrong token");

                return Forbid();
            }

            var message = await _messageService.GetMessage(messageId);

            if (message == null)
            {
                return NotFound();
            }

            // Can't update the message twice or more times
            if (message.Updated)
            {
                return Forbid();
            }

            var messageState = (MessageState) model.State;

            await _messageService.UpdateMessage(message, messageState);

            _logger.Log(LogLevel.Debug, $"Message [{messageId}] was updated to state {messageState.ToString()}");

            if (message.ConnectionId != null)
            {
                await _webHub.Clients.Client(message.ConnectionId).SendAsync("MessageSent", model.State);
            }
            
            // 204 - UPDATED
            return StatusCode(204);
        }

        /// <summary>
        /// Saves the message on the database and contacts the device to send it
        /// Method - POST /api/messages
        /// </summary>
        /// <param name="model">Message creation model</param>
        /// <returns>HTTP result of the action</returns>
        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageModel model)
        {
            // Parameters cannot be null
            if (model.Content == null || model.Recipient == null)
            {
                _logger.Log(LogLevel.Warning, "Wrong request for sending message was sent");

                return BadRequest();
            }

            // In case device ID is not specified
            if (model.DeviceId == null)
            {
                var random = new Random();

                // Gets a random connected device
                var randomDevice = (await _deviceService.GetConnectedDevices()).OrderBy(_ => random.NextDouble())
                    .FirstOrDefault();

                if (randomDevice == null)
                {
                    _logger.Log(LogLevel.Warning, "No devices are connected");

                    return NotFound();
                }

                model.DeviceId = randomDevice.DeviceId;
            }

            // Checks if the device is connected
            if (!_devicesManager.IsDeviceConnected(model.DeviceId))
            {
                _logger.Log(LogLevel.Warning, $"Device with ID {model.DeviceId} is not connected");

                return NotFound();
            }

            // Checks validity of phone number
            if (!IsValidPhoneNumber(model.Recipient))
            {
                _logger.Log(LogLevel.Warning, "Wrong request with invalid phone number was sent");

                return BadRequest();
            }

            var message = await _messageService.CreateMessage(model);

            // Gets the connection ID from the device ID
            var connectionId = _devicesManager.GetConnectionId(model.DeviceId);

            // Send request to the client to send the message
            await _hub.Clients.Client(connectionId).SendAsync("SendMessage", message.MessageId.ToString());

            // Notifies all web clients about statistics change
            await _webHub.Clients.All.SendAsync("StatisticsChange");

            _logger.Log(LogLevel.Debug,
                $"Message with ID {message.MessageId} was created and sent to a device with ID {model.DeviceId}");

            return StatusCode(201);
        }

        /// <summary>
        /// Checks if the input string is a valid phone number
        /// </summary>
        /// <param name="phoneNumber">Input string to check</param>
        /// <returns>Is or isn't a phone number</returns>
        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            var regex = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");

            var match = regex.Match(phoneNumber);

            return match.Success;
        }
    }
}