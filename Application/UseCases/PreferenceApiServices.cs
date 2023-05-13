

using Application.Interfaces;
using System.Text.Json;

namespace Application.UseCases
{
    public class PreferenceApiServices : IPreferenceApiServices
    {
        private string? _message;
        private string? _response;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _url;

        public PreferenceApiServices(HttpClient httpClient)
        {
            _url = "https://localhost:7175/api/v1/UserPreferences";
            _httpClient = httpClient;
        }

        public Task<bool> GetAllPreference()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPreferencesByList(List<int> preferenceIds)
        {
            throw new NotImplementedException();
        }

        public string GetMessage()
        {
            return _message;
        }

        public JsonDocument GetResponse()
        {
            if (_response == null || _response == "")
            {
                return JsonDocument.Parse("{}");
            }

            return JsonDocument.Parse(_response);
        }

        public int GetStatusCode()
        {
            return _statusCode;
        }
    }
}
