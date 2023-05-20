using Application.Interfaces;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.UseCases
{
    public class UserApiServices : IUserApiServices
    {
        private string? _message;
        private string? _response;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _url;

        public UserApiServices(HttpClient httpClient)
        {
            _url = "https://localhost:7020/api/v1/User";
            _httpClient = httpClient;
        }

        public async Task<JsonNode> GetAllUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync(_url);
                if(response.IsSuccessStatusCode)
                {
                    var jsonResult= JsonObject.Parse(response.Content.ReadAsStringAsync().Result);
                    _message = "Se ha obtenido el documento correctamente";
                    _statusCode = 200;
                    return jsonResult;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return JsonObject.Parse("{ }");
            }
            catch (Exception e)
            {
                _message= e.Message;
                return JsonObject.Parse("{ }");
            }
        }

        public Task<bool> GetUsersByList(List<int> userIds)
        {
            try
            {
                var response = await _httpClient.GetAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = JsonObject.Parse(response.Content.ReadAsStringAsync().Result);
                    _message = "Se ha obtenido el documento correctamente";
                    _statusCode = 200;
                    return jsonResult;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return JsonObject.Parse("{ }");
            }
            catch (Exception e)
            {
                _message = e.Message;
                return JsonObject.Parse("{ }");
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
