
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<JsonNode> GetAllUsers();

        Task<bool> GetUsersByList(List<int> userIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
