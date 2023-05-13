

using System.Text.Json;

namespace Application.Interfaces
{
    public interface IMatchApiServices
    {
        Task<bool> GetAllMatches();

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
