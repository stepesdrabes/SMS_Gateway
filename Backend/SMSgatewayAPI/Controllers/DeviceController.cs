using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SMSgatewayAPI.Hubs;
using SMSgatewayAPI.Managers;
using SMSgatewayAPI.Models;
using SMSgatewayAPI.Services;

namespace SMSgatewayAPI.Controllers;

[ApiController]
[Route("api")]
public class DeviceController : Controller
{
    private readonly ILogger<DeviceController> _logger;

    private readonly IHubContext<WebHub> _webHub;

    private readonly IDeviceService _deviceService;
    private readonly SessionTokenManager _tokenManager;

    public DeviceController(ILogger<DeviceController> logger, IHubContext<WebHub> webHub,
        IDeviceService deviceService, SessionTokenManager tokenManager)
    {
        _logger = logger;
        _webHub = webHub;
        _deviceService = deviceService;
        _tokenManager = tokenManager;
    }

    /// <summary>
    /// Registers the device on the database and creates session token
    /// Method - POST /api/devices
    /// </summary>
    /// <param name="model">Model of the device to register</param>
    /// <returns>Session token for the current connection</returns>
    [HttpPost]
    [Route("devices")]
    public async Task<ActionResult<SessionToken>> RegisterDevice([FromBody] DeviceModel model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        // Generates new Guid to be used as a token
        var token = Guid.NewGuid().ToString();

        // Adds the devices token to TokenManager
        _tokenManager.AddDeviceToken(model.DeviceId, token);

        await _deviceService.RegisterDevice(model);

        // Creates new instance of SessionToken with the token and the device model
        var sessionToken = new SessionToken
        {
            Token = token,
            Model = model
        };

        _logger.Log(LogLevel.Debug, $"New token was generated for a device with ID {model.DeviceId} [{token}]");

        // Notifies all web clients about statistics change
        await _webHub.Clients.All.SendAsync("StatisticsChange");

        return Ok(sessionToken);
    }

    /// <summary>
    /// Gets a device with certain ID 
    /// Method - GET /api/device/{DeviceID}
    /// </summary>
    /// <param name="deviceId">ID of the device</param>
    /// <returns>Device with certain ID</returns>
    [HttpGet]
    [Route("device/{deviceId}")]
    public async Task<ActionResult<DeviceModel>> GetDeviceById(string deviceId)
    {
        // Device ID can't be null of empty string
        if (string.IsNullOrEmpty(deviceId))
        {
            return BadRequest();
        }

        var device = await _deviceService.GetDevice(deviceId);

        // Checks if the device with DeviceID exists
        if (device == null)
        {
            return NotFound();
        }

        return Ok(device);
    }

    /// <summary>
    /// Gets list of all connected devices
    /// </summary>
    /// <returns>List of all connected devices</returns>
    [HttpGet]
    [Route("devices")]
    public async Task<ActionResult<List<DeviceModel>>> GetConnectedDevices() =>
        Ok(await _deviceService.GetConnectedDevices());
}