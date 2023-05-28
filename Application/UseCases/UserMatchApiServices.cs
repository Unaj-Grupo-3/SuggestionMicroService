using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        }
        public async Task<List<UserMatch>> GetAllMatches()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var response = await _httpClient.GetAsync(_url + "/Worker");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync();
                    var listResponse = new List<UserMatch>();
                    var list = JArray.Parse(jsonResult.Result);

                    foreach (var item in list)
                    {
                        UserMatch mapp = new UserMatch()
                        {
                            UserMatchId = item.SelectToken("usermatchid") != null ? (int)item.SelectToken("usermatchid") : 0,
                            User1 = item.SelectToken("user1") != null ? (int)item.SelectToken("user1") : 0,
                            User2 = item.SelectToken("user2") != null ? (int)item.SelectToken("user2") : 0,
                            CreatedAt= item.SelectToken("createat") != null ? DateTime.Parse(item.SelectToken("createat").ToString()) : DateTime.MinValue,
                            UpdatedAt = item.SelectToken("updatedat") != null ? DateTime.Parse(item.SelectToken("updatedat").ToString()) : DateTime.MinValue,
                            LikeUser2= item.SelectToken("likeuser2") != null ? (int)item.SelectToken("likeuser2") : 0,
                            LikeUser1 = item.SelectToken("likeuser1") != null ? (int)item.SelectToken("likeuser1") : 0,
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
