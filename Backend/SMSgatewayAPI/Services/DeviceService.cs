using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using SMSgatewayAPI.Managers;

namespace SMSgatewayAPI.Services;

public interface IDeviceService
{
    /// <summary>
    /// Registers the device in the database,
    /// </summary>
    /// <param name="deviceModel">Device to register</param>
    Task RegisterDevice(DeviceModel deviceModel);

    /// <summary>
    /// Gets device with certain ID from the database
    /// </summary>
    /// <param name="deviceId">ID of the device</param>
    /// <returns>DeviceModel with certain ID or null</returns>
    Task<DeviceModel> GetDevice(string deviceId);

    /// <summary>
    /// Gets list of all currently connected devices
    /// </summary>
    /// <returns>List of connected devices</returns>
    Task<IEnumerable<DeviceModel>> GetConnectedDevices();
}

public class DeviceService : IDeviceService
{
    private readonly DatabaseContext _context;
    private readonly DevicesManager _devicesManager;

    public DeviceService(DatabaseContext context, DevicesManager devicesManager)
    {
        _context = context;
        _devicesManager = devicesManager;
    }

    public async Task<DeviceModel> GetDevice(string deviceId) => await _context.Devices.FindAsync(deviceId);

    public async Task RegisterDevice(DeviceModel deviceModel)
    {
        var device = await _context.Devices.FirstOrDefaultAsync(model => model.DeviceId == deviceModel.DeviceId);

        // Checks if the device isn't already in the database
        if (device != null)
        {
            return;
        }

        deviceModel.RegisteredAt = DateTime.Now;

        // Adds the device to the database and saves the changes
        await _context.Devices.AddAsync(deviceModel);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DeviceModel>> GetConnectedDevices()
    {
        var devices = await _context.Devices.ToListAsync();

        // Returns collection of devices that are connected
        return devices.Where(model => _devicesManager.IsDeviceConnected(model.DeviceId));
    }
}