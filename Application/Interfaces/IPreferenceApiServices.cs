
using Application.Models;
using System.Text.Json;

namespace Application.Interfaces
{
    public interface IPreferenceApiServices
    {
        Task<JsonDocument> GetAllPreference();
        Task<List<UserPreferencesResponse>> GetAllPreferenceObj();

        Task<IList<PreferenceResponse>> GetPreferencesByList(List<int> preferenceIds);

        string GetMessage();

        JsonDocument GetResponse();

        int GetStatusCode();
    }
}
