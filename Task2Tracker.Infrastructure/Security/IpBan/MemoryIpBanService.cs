using System.Collections.Concurrent;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Infrastructure.Security.IpBan;

public sealed class MemoryIpBanService : IIpBanService
{
    private readonly ConcurrentDictionary<string, DateTime> _bannedIps = new();

    public bool IsBanned(string ipAddress)
    {
        if (!_bannedIps.TryGetValue(ipAddress, out var bannedUntil))
            return false;

        if (bannedUntil > DateTime.UtcNow)
            return true;

        _bannedIps.TryRemove(ipAddress, out _);

        return false;
    }

    public void Ban(string ipAddress, TimeSpan duration)
    {
        _bannedIps[ipAddress] =
            DateTime.UtcNow.Add(duration);
    }
}