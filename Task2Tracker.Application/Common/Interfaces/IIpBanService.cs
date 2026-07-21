public interface IIpBanService
{
    bool IsBanned(string ipAddress);

    void Ban(string ipAddress, TimeSpan duration);
}