
using Application.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Interfaces
{
    public interface IUserApiServices
    {
        Task<JsonDocument> GetAllUsers();
        Task<List<UserResponse>> GetAllUsersObj();
        Task<List<int>> GetAllUsersIdsByFilters(List<int> gendersId, int minAge, int maxAge, int distance, double longitude, double latitude);
        Task<List<UserResponse>?> GetUsersByList(List<int> userIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();

    }
}
