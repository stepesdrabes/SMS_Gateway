using System.Collections.Generic;

namespace SMSgatewayAPI.Managers;

public class SessionTokenManager
{
    private readonly Dictionary<string, string> _deviceTokens;

    public SessionTokenManager()
    {
        _deviceTokens = new Dictionary<string, string>();
    }

    /// <summary>
    /// Adds device with a token to a dictionary
    /// </summary>
    /// <param name="deviceId">ID of the device to add</param>
    /// <param name="token">Token</param>
    public void AddDeviceToken(string deviceId, string token)
    {
        lock (_deviceTokens)
        {
            if (_deviceTokens.ContainsKey(deviceId))
            {
                _deviceTokens.Remove(deviceId);
            }

            _deviceTokens.Add(deviceId, token);
        }
    }

    /// <summary>
    /// Removes the device from the list
    /// </summary>
    /// <param name="deviceId">ID of the device to remove</param>
    public void RemoveDeviceToken(string deviceId)
    {
        lock (_deviceTokens)
        {
            if (_deviceTokens.ContainsKey(deviceId))
            {
                _deviceTokens.Remove(deviceId);
            }
        }
    }

    /// <summary>
    /// Checks if the token matches the device
    /// </summary>
    /// <param name="deviceId">DeviceID</param>
    /// <param name="token">Token</param>
    /// <returns>Whether the token matches or not</returns>
    public bool DoesTokenMatch(string deviceId, string token)
    {
        lock (_deviceTokens)
        {
            if (!_deviceTokens.ContainsKey(deviceId))
            {
                return false;
            }

            return _deviceTokens.GetValueOrDefault(deviceId) == token;
        }
    }
}