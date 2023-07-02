using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json;


namespace Application.UseCases
{
    public class UserApiServices : IUserApiServices
    {
        private string? _message;
        private string? _response;
        private readonly  string _apiKey;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _url;

        public UserApiServices(HttpClient httpClient, IConfiguration configuration)
        {
            _url = "https://localhost:7020/api/v1/User";
            _httpClient = httpClient;
            _apiKey = configuration["ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
        }

        public async Task<JsonDocument> GetAllUsers()
        {
            try
            {
                //_httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
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
                //_httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
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

        public async Task<List<UserResponse>?> GetUsersByList(List<int> userIds)
        {
            try
            {
                List<UserResponse> userList = new List<UserResponse>();

                string paramRequest = "";
                for (int i = 0; i < userIds.Count; i++)
                {
                    paramRequest = paramRequest + string.Format("usersId={0}&", userIds[i]);
                }

                //_httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var response = await _httpClient.GetAsync(_url + "/true?" + paramRequest);

                if (response != null && response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JArray array = JArray.Parse(content);
                    foreach (var i in array)
                    {
                        UserResponse user = new UserResponse();
                        user.UserId = (int)i.SelectToken("userId");
                        user.Name = (string)i.SelectToken("name");
                        user.LastName = (string)i.SelectToken("lastName");
                        user.Description = (string)i.SelectToken("description");
                        user.Birthday = (DateTime)i.SelectToken("birthday");
                        //birthday description location {id latitude longitude address } images[{id url}] gender {genderId description}
                        // location
                        JToken locationToken = i.SelectToken("location");
                        if (locationToken != null)
                        {
                            LocationResponse location = new LocationResponse
                            {
                                Id = (int)locationToken.SelectToken("id"),
                                Latitude = (double)locationToken.SelectToken("latitude"),
                                Longitude = (double)locationToken.SelectToken("longitude"),
                                Address = (string)locationToken.SelectToken("address")
                            };
                            user.Location = location;
                        }

                        // images
                        JToken imagesToken = i.SelectToken("images");
                        if (imagesToken != null && imagesToken.Type == JTokenType.Array)
                        {
                            IList<ImageResponse> images = new List<ImageResponse>();
                            foreach (JToken imageToken in imagesToken)
                            {
                                ImageResponse image = new ImageResponse
                                {
                                    Id = (int)imageToken.SelectToken("id"),
                                    Url = (string)imageToken.SelectToken("url")
                                };
                                images.Add(image);
                            }
                            user.Images = images;
                        }

                        // gender
                        JToken genderToken = i.SelectToken("gender");
                        if (genderToken != null)
                        {
                            GenderResponse gender = new GenderResponse
                            {
                                GenderId = (int)genderToken.SelectToken("genderId"),
                                Description = (string)genderToken.SelectToken("description")
                            };
                            user.Gender = gender;
                        }

                        userList.Add(user);
                    }

                    return userList;
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<List<int>> GetAllUsersIdsByFilters(List<int> gendersId, int minAge, int maxAge, int distance, double longitude, double latitude)
        {
            try
            {
                var filterUrl = "";

                for (int i = 0;  i < gendersId.Count; i++)
                {
                    filterUrl += i > 0 ? $"&GendersId={gendersId[i]}" : $"?GendersId={gendersId[i]}";
                }

                filterUrl += $"&Latitude={latitude.ToString().Replace(",",".")}&Longitude={longitude.ToString().Replace(",", ".")}&Distance={distance}&MinEdad={minAge}&MaxEdad={maxAge}";

                //_httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var response = await _httpClient.GetAsync(_url + filterUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync();
                    var listResponse = new List<int>();
                    var list = JArray.Parse(jsonResult.Result);

                    foreach( var id in list)
                    {
                        listResponse.Add((int)id);
                    }
                     
                    return listResponse;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return new List<int>();
            }
            catch (Exception e)
            {
                _message = e.Message;
                return new List<int>();
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
