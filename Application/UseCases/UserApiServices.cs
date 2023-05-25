using Application.Interfaces;
using Application.Models;
using Newtonsoft.Json.Linq;
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

        public async Task<List<UserResponse>> GetAllUsersObj()
        {
            try
            {

                var response = await _httpClient.GetAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync();
                    var listResponse = new List<UserResponse>();
                    var list = JArray.Parse(jsonResult.Result);

                    foreach(var item in list)
                    {
                        UserResponse mapp = new UserResponse()
                        {
                            UserId = item.SelectToken("userId") != null ? (int)item.SelectToken("userId") : 0,
                            Name = item.SelectToken("name") != null ? (string)item.SelectToken("name") : "",
                            LastName = item.SelectToken("lastName") != null ? (string)item.SelectToken("lastName") : "",
                            Birthday = item.SelectToken("birthday") != null ? (DateTime)item.SelectToken("birthday") : DateTime.MinValue,
                            Description = item.SelectToken("description") != null ? (string)item.SelectToken("description") : "",
                            Location = new LocationResponse(),
                            Images = new List<ImageResponse>(),
                            Gender = new GenderResponse(),
                        };

                        mapp.Location.Id = item.SelectToken("location").SelectToken("id") != null ? (int)item.SelectToken("location").SelectToken("id") : 0;
                        mapp.Location.Latitude = item.SelectToken("location").SelectToken("latitude") != null ? (float)item.SelectToken("location").SelectToken("latitude") : 0;
                        mapp.Location.Longitude = item.SelectToken("location").SelectToken("longitude") != null ? (float)item.SelectToken("location").SelectToken("longitude") : 0;
                        mapp.Location.Address = item.SelectToken("location").SelectToken("address") != null ? (string)item.SelectToken("location").SelectToken("address") : "";

                        mapp.Gender.GenderId = item.SelectToken("gender").SelectToken("genderId") != null ? (int)item.SelectToken("gender").SelectToken("genderId") : 0;
                        mapp.Gender.Description = item.SelectToken("gender").SelectToken("description") != null ? (string)item.SelectToken("gender").SelectToken("description") : "";

                        if (item.SelectToken("images") != null)
                        {
                            foreach (var subItem in item.SelectToken("images").ToList())
                            {
                                mapp.Images.Add(new ImageResponse
                                {
                                    Id = subItem.SelectToken("id") != null ? (int)subItem.SelectToken("id") : 0,
                                    Url = subItem.SelectToken("url") != null ? (string)subItem.SelectToken("url") : ""
                                });
                            }
                        }

                        listResponse.Add(mapp);
                    }

                    return listResponse;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return new List<UserResponse>();
            }
            catch (Exception e)
            {
                _message = e.Message;
                return new List<UserResponse>();
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
