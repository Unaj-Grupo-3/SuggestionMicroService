
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IPreferenceApiServices
    {
        Task<bool> GetAllPreference();

        Task<bool> GetPreferencesByList(List<int> preferenceIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
