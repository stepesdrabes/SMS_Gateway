using System.Collections.Generic;

namespace SMSgatewayAPI.Managers;

public class DevicesManager
{
    private readonly Dictionary<string, string> _devices;
    private readonly Dictionary<string, string> _connectionToDevices;

    public DevicesManager()
    {
        _devices = new Dictionary<string, string>();
        _connectionToDevices = new Dictionary<string, string>();
    }

    /// <summary>
    /// Add the device to a dictionary of connected devices
    /// </summary>
    /// <param name="deviceId">ID of the device to add</param>
    /// <param name="connectionId">ID of the connection</param>
    public void AddDevice(string deviceId, string connectionId)
    {
        lock (_devices)
        {
            if (_devices.ContainsKey(deviceId))
            {
                return;
            }

            _devices.Add(deviceId, connectionId);

            _connectionToDevices.Add(connectionId, deviceId);
        }
    }

    /// <summary>
    /// Removes the device from a dictionary of connected devices
    /// </summary>
    /// <param name="deviceId">ID of the device to remove</param>
    public void RemoveDevice(string deviceId)
    {
        lock (_devices)
        {
            if (!_devices.ContainsKey(deviceId))
            {
                return;
            }

            var connectionId = _devices[deviceId];

            _devices.Remove(deviceId);

            _connectionToDevices.Remove(connectionId);
        }
    }

    /// <summary>
    /// Gets if the device is currently connected to the SignalR server
    /// </summary>
    /// <param name="deviceId">ID of the device</param>
    /// <returns>Whether or not the device is currently connected to the server</returns>
    public bool IsDeviceConnected(string deviceId)
    {
        lock (_devices)
        {
            return _devices.ContainsKey(deviceId);
        }
    }

    /// <summary>
    /// Gets SignalR connection ID by device ID
    /// </summary>
    /// <param name="deviceId">ID of the device</param>
    /// <returns>ConnectionID of the device</returns>
    public string GetConnectionId(string deviceId)
    {
        lock (_devices)
        {
            return _devices.GetValueOrDefault(deviceId, null);
        }
    }

    /// <summary>
    /// Gets DeviceID from connection ID
    /// </summary>
    /// <param name="connectionId">Connection ID to get device ID from</param>
    /// <returns>DeviceID corresponding to the connection ID</returns>
    public string GetDeviceIdFromConnection(string connectionId)
    {
        lock (_devices)
        {
            return _connectionToDevices.GetValueOrDefault(connectionId, null);
        }
    }
}