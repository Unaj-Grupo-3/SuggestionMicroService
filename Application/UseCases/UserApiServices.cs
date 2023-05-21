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

        public async Task<JsonDocument> GetAllUsers()
        {
            try
            {
                var response = await _httpClient.GetAsync(_url);
                if(response.IsSuccessStatusCode)
                {
                    var jsonResult= JsonDocument.Parse(response.Content.ReadAsStringAsync().Result);
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
                _message= e.Message;
                return JsonDocument.Parse("{ }");
            }
        }

        public async Task<JsonDocument> GetUsersByList(List<int> userIds)
        {
            try
            {
                string paramRequest = "";
                for (int i = 0; i < userIds.Count; i++)
                {
                    paramRequest = paramRequest + string.Format("usersId={0}&", userIds[i]);
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
