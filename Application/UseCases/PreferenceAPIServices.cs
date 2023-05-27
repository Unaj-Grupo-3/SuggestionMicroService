

using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.UseCases
{
    public class PreferenceApiServices : IPreferenceApiServices
    {
        private string? _message;
        private string? _response;
        private readonly string _apiKey;
        private int _statusCode;
        private HttpClient _httpClient;
        private readonly string _url;

        public PreferenceApiServices(HttpClient httpClient, IConfiguration configuration)
        {
            _url = "https://localhost:7175/api/v1/UserPreferences";
            _httpClient = httpClient;
            _apiKey = configuration["ApiKey"];
        }

        public async Task<JsonDocument> GetAllPreference()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
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

        public async Task<List<UserPreferencesResponse>> GetAllPreferenceObj()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var response = await _httpClient.GetAsync(_url);
                if (response.IsSuccessStatusCode)
                {
                    List<UserPreferencesResponse> listResponse = new List<UserPreferencesResponse>();
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    JArray list = JArray.Parse(jsonResult);
                    foreach (var item in list)
                    {
                        UserPreferencesResponse mapp = new UserPreferencesResponse()
                        {
                            UserId = item.SelectToken("userId") != null ? (int)item.SelectToken("userId") : 0,
                            SinceAge = item.SelectToken("sinceAge") != null ? (int)item.SelectToken("sinceAge") : 0,
                            UntilAge = item.SelectToken("untilAge") != null ? (int)item.SelectToken("untilAge") : 99,
                            Distance = item.SelectToken("distance") != null ? (int)item.SelectToken("distance") : 0,
                            GendersPreferencesId = new List<int>(),
                            InterestPreferencesId = new List<int>(),
                            OwnInterestPreferencesId = new List<int>()
                        };
                        if (item.SelectToken("gendersPreferencesId") != null)
                        {
                            foreach (var subItem in item.SelectToken("gendersPreferencesId").ToList())
                            {
                                mapp.GendersPreferencesId.Add((int)subItem);
                            }
                        }

                        if (item.SelectToken("interestPreferencesId") != null)
                        {
                            foreach (var SubItem in item.SelectToken("interestPreferencesId").ToList())
                            {
                                mapp.InterestPreferencesId.Add((int)SubItem);
                            }
                        }
                        if (item.SelectToken("ownInterestPreferencesId") != null)
                        {
                            foreach (var subItem in item.SelectToken("ownInterestPreferencesId").ToList())
                            {
                                mapp.OwnInterestPreferencesId.Add((int)subItem);
                            }
                        }
                        listResponse.Add(mapp);
                    }
                    _message = "Se ha obtenido el documento correctamente";
                    _statusCode = 200;
                    return listResponse;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return new List<UserPreferencesResponse>();
            }
            catch (Exception e)
            {
                _message = e.Message;
                return new List<UserPreferencesResponse>();
            }
        }

        public async Task<IList<PreferenceResponse>> GetPreferencesByList(List<int> preferenceIds)
        {
            try
            {
                string paramRequest = "";
                for (int i = 0; i < preferenceIds.Count; i++)
                {
                    paramRequest = paramRequest + string.Format("userIds={0}&", preferenceIds[i]);
                }
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                var response = await _httpClient.GetAsync(_url + "/Ids?" + paramRequest + "fullResponse=true");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = response.Content.ReadAsStringAsync();
                    _message = "Se ha obtenido el documento correctamente";
                    _statusCode = 200;

                    var preferencesList = new List<PreferenceResponse>();

                    var list = JArray.Parse(jsonResult.Result);

                    // Mapeo de preferencias 

                    foreach (var preference in list)
                    {
                        PreferenceResponse preferenceMapp = new PreferenceResponse()
                        {
                            UserId = (int)preference.SelectToken("userId"),
                            SinceAge = (int)preference.SelectToken("sinceAge"),
                            UntilAge = (int)preference.SelectToken("untilAge"),
                            Distance = (int)preference.SelectToken("distance"),
                            GendersPreferences = new List<GenderPreferenceResponse>(),
                            CategoryPreferences = new List<InterestCategoryResponse>(),

                        };

                        //Mapeo la preferencia del genero
                        if (preference.SelectToken("gendersPreferencesId") != null)
                        {
                            foreach (var gender in preference.SelectToken("gendersPreferencesId").ToList())
                            {
                                GenderPreferenceResponse genderMapp = new GenderPreferenceResponse()
                                {
                                    UserId = (int)gender.SelectToken("userId"),
                                    GenderId = (int)gender.SelectToken("genderId"),
                                    GenderName = (string)gender.SelectToken("genderName")
                                };

                                preferenceMapp.GendersPreferences.Add(genderMapp);
                            }
                        }

                        // Mapeo los intereses
                        if (preference.SelectToken("categoryPreferencesId") != null)
                        {
                            foreach (var category in preference.SelectToken("categoryPreferencesId").ToList())
                            {
                                InterestCategoryResponse categoryMapp = new InterestCategoryResponse()
                                {
                                    Id = (int)category.SelectToken("id"),
                                    Description = (string)category.SelectToken("description"),
                                    InterestPreferencesId = new List<PreferenceResponseFull>()
                                };

                                if (category.SelectToken("interestPreferencesId") != null)
                                {
                                    foreach(var interest in category.SelectToken("interestPreferencesId").ToList())
                                    {
                                        PreferenceResponseFull interestMapp = new PreferenceResponseFull()
                                        {
                                            Interest = new InterestResponse()
                                            {
                                                Id = (int)interest.SelectToken("interest").SelectToken("id"),
                                                Description = (string)interest.SelectToken("interest").SelectToken("description")
                                            },
                                            OwnInterest = (bool)interest.SelectToken("ownInterest"),
                                            Like = (bool)interest.SelectToken("like")

                                        };

                                        categoryMapp.InterestPreferencesId.Add(interestMapp);
                                    }
                                }

                                preferenceMapp.CategoryPreferences.Add(categoryMapp);
                            }
                        }

                        preferencesList.Add(preferenceMapp);
                    }

                    return preferencesList;
                }
                _message = "No se ha podido obtener el documento mediante la peticion.";
                _statusCode = 404;
                return new List<PreferenceResponse>();

            }
            catch (Exception e)
            {
                _message = e.Message;
                return new List<PreferenceResponse>();
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
