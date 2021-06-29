using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using SMSgatewayAPI.Managers;

namespace SMSgatewayAPI.Services
{
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
        private readonly DevicesManager _devicesManager;

        public DeviceService(DevicesManager devicesManager)
        {
            _devicesManager = devicesManager;
        }

        public async Task RegisterDevice(DeviceModel deviceModel)
        {
            await using var context = new DatabaseContext();

            var device = await context.Devices.FirstOrDefaultAsync(model => model.DeviceId == deviceModel.DeviceId);

            // Checks if the device isn't already in the database
            if (device != null)
            {
                return;
            }

            deviceModel.RegisteredAt = DateTime.Now;

            // Adds the device to the database and saves the changes
            await context.Devices.AddAsync(deviceModel);
            await context.SaveChangesAsync();
        }

        public async Task<DeviceModel> GetDevice(string deviceId)
        {
            await using var context = new DatabaseContext();

            // Returns the first device that has the given ID
            return await context.Devices.FirstOrDefaultAsync(model => model.DeviceId == deviceId);
        }

        public async Task<IEnumerable<DeviceModel>> GetConnectedDevices()
        {
            await using var context = new DatabaseContext();

            // Gets list of all devices
            var devices = context.Devices.ToList();

            // Returns collection of devices that are connected
            return devices.Where(model => _devicesManager.IsDeviceConnected(model.DeviceId));
        }
    }
}