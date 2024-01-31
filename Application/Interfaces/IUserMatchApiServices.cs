using Application.Models;
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IUserMatchApiServices
    {
        Task<List<UserMatch>> GetAllMatches();

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
