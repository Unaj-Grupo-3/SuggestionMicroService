
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<bool> GetAllUsers();

        Task<bool> GetUsersByList(List<int> userIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
