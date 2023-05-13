using Application.Interfaces;
using System.Text.Json;

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

        public Task<bool> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetUsersByList(List<int> userIds)
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
