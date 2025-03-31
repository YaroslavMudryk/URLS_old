using URLS.Domain.Models;

namespace URLS.Application.Services.Interfaces;

public interface ILocationService
{
    Task<Location> GetIpInfoAsync(string ip);
}
