using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Application.UseCases
{
    public class UserMatchApiServices : IUserMatchApiServices
    {
        private string? _message;
        private string? _response;
        private readonly string _apiKey;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _url;

        public UserMatchApiServices(HttpClient httpClient, IConfiguration configuration)
        {
            _url = "https://localhost:7199/api/v1/UserMatch";
            _httpClient = httpClient;
            _apiKey = configuration["ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        }
        public async Task<List<UserMatch>> GetAllMatches()
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var response = await _httpClient.GetAsync(_url + "/Worker");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync();
                    var listResponse = new List<UserMatch>();
                    var a = JsonDocument.Parse(jsonResult.Result);
                    var list = JArray.Parse(a.RootElement.GetProperty("response").ToString());

                    foreach (var item in list)
                    {
                        UserMatch mapp = new UserMatch()
                        {
                            UserMatchId = item.SelectToken("userMatchId") != null ? (int)item.SelectToken("userMatchId") : 0,
                            User1 = item.SelectToken("user1") != null ? (int)item.SelectToken("user1") : 0,
                            User2 = item.SelectToken("user2") != null ? (int)item.SelectToken("user2") : 0,
                            CreatedAt= item.SelectToken("createdAt") != null ? DateTime.Parse(item.SelectToken("createdAt").ToString()) : DateTime.MinValue,
                            UpdatedAt = item.SelectToken("updatedAt") != null ? DateTime.Parse(item.SelectToken("updatedAt").ToString()) : DateTime.MinValue,
                            LikeUser2= item.SelectToken("likeUser2") != null ? (int)item.SelectToken("likeUser2") : 0,
                            LikeUser1 = item.SelectToken("likeUser1") != null ? (int)item.SelectToken("likeUser1") : 0,
                        };
                        listResponse.Add(mapp);
                    }
                    return listResponse;

                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return new List<UserMatch>();
            }
            catch (Exception e)
            {
                _message = e.Message;
                return new List<UserMatch>();
            }
        }

        public string GetMessage()
        {
            throw new NotImplementedException();
        }

        public JsonDocument GetResponse()
        {
            throw new NotImplementedException();
        }

        public int GetStatusCode()
        {
            throw new NotImplementedException();
        }
    }
}
