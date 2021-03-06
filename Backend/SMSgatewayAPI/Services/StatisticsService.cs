using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;

namespace SMSgatewayAPI.Services;

public interface IStatisticsService
{
    /// <summary>
    /// Gets number of total sent messages
    /// </summary>
    /// <returns>Number of sent messages</returns>
    Task<int> GetSentMessages();

    /// <summary>
    /// Gets number of active devices
    /// </summary>
    /// <returns>Number of active devices</returns>
    Task<int> GetActiveDevices();

    /// <summary>
    /// Gets number of registered devices
    /// </summary>
    /// <returns>Number of registered devices</returns>
    Task<int> GetRegisteredDevices();
}

public class StatisticsService : IStatisticsService
{
    private readonly DatabaseContext _context;
    private readonly IDeviceService _deviceService;

    public StatisticsService(DatabaseContext context, IDeviceService deviceService)
    {
        _context = context;
        _deviceService = deviceService;
    }

    public async Task<int> GetSentMessages() => await _context.Messages.CountAsync();

    public async Task<int> GetActiveDevices() => (await _deviceService.GetConnectedDevices()).Count();

    public async Task<int> GetRegisteredDevices() => await _context.Devices.CountAsync();
}