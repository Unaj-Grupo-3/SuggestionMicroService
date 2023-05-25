
using Application.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<JsonDocument> GetAllUsers();
        Task<List<UserResponse>> GetAllUsersObj();

        Task<JsonDocument> GetUsersByList(List<int> userIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
