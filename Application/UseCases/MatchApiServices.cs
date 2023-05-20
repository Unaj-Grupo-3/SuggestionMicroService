
using Application.Interfaces;
using System.Text.Json;

namespace Application.UseCases
{
    public class MatchApiServices : IMatchApiServices
    {
        private string? _message;
        private string? _response;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _url;

        public MatchApiServices(HttpClient httpClient)
        {
            _url = "https://localhost:7199/api/v1/Match";
            _httpClient = httpClient;
        }

        public Task<bool> GetAllMatches()
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
