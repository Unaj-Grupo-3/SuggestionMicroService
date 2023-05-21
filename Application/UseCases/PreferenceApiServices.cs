

using Application.Interfaces;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public async Task<JsonDocument> GetAllPreference()
        {
            try
            {
                var response = await _httpClient.GetAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
                    _message = "Se ha obtenido el documento correctamente";
                    _statusCode = 200;
                    return jsonResult;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return JsonDocument.Parse("{ }");
            }
            catch (Exception e)
            {
                _message = e.Message;
                return JsonDocument.Parse("{ }");
            }
        }

        public async Task<JsonDocument> GetPreferencesByList(List<int> preferenceIds)
        {
            try
            {
                string paramRequest = "";
                for (int i = 0; i < preferenceIds.Count; i++)
                {
                    paramRequest = paramRequest + string.Format("usersId={0}&", preferenceIds[i]);
                }
                var response = await _httpClient.GetAsync(_url + "/userByIds?" + paramRequest);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
                    _message = "Se ha obtenido el documento correctamente";
                    _statusCode = 200;
                    return jsonResult;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return JsonDocument.Parse("{ }");
            }
            catch (Exception e)
            {
                _message = e.Message;
                return JsonDocument.Parse("{ }");
            }
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
