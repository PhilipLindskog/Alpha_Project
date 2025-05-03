using Business.Models;

namespace Business.Interfaces
{
    public interface IClientService
    {
        Task<ClientResult> GetClientsAsync();
    }
}