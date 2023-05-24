using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Helpers;

namespace Application.UseCases
{
    public class SuggestionWorkerServices: ISuggestionWorkerServices
    {
        private readonly IPreferenceApiServices _preferenceApiServices;
        private readonly IUserApiServices _userApiServices;
        private readonly ISuggestionCommands _suggestionCommand;
        private readonly ISuggestionQueries _suggestionQueries;
        private string? _message;
        private int _statusCode;

        public SuggestionWorkerServices(IUserApiServices userApiServices, IPreferenceApiServices preferenceApiServices, ISuggestionCommands suggestionCommand, ISuggestionQueries suggestionQueries)
        {
            _preferenceApiServices = preferenceApiServices;
            _userApiServices = userApiServices;
            _suggestionCommand = suggestionCommand;
            _suggestionQueries = suggestionQueries;
        }
        public SuggestionWorkerServices() { }

        public async Task GenerateSuggestionXUser(int userId)
        {
            throw new ArgumentException();
        }

        public async Task GenerateSuggestionAll()
        {
            // borrado general en full - borrado por ID en comun
            // Hay que insertar los casos de sugerencias
            JsonDocument jsonUser = await _userApiServices.GetAllUsers();
            JsonDocument jsonPreference = await _preferenceApiServices.GetAllPreference();
            var convertUser = ConvertJsonToUserResponse(jsonUser);
            var convertPreference = ConvertJsonToUserPreferencesResponse(jsonPreference);

            foreach(UserResponse item in convertUser.Result)
            {
                var mainUserId = item.UserId;
                var mainPreferenceResponse = convertPreference.Result.FirstOrDefault(x => x.UserId == mainUserId);
                var mainAge = DateTime.Today.AddTicks(item.Birthday.Ticks).Year - 1;
                var mainGender = item.Gender.GenderId;

                foreach (UserPreferencesResponse suggestedPreference in convertPreference.Result)
                {
                    var suggestedUser = convertUser.Result.FirstOrDefault(x => x.UserId == suggestedPreference.UserId);
                    if (mainPreferenceResponse == null)
                    {
                        Suggestion suggestion = new Suggestion()
                        {
                            MainUser = mainUserId,
                            SuggestedUser = suggestedUser.UserId,
                            DateView = null,
                            View = false
                        };
                        await _suggestionCommand.InsertSuggestion(suggestion);
                    }
                    if (mainUserId.Equals(suggestedPreference.UserId)) { continue; } // No calcula el mismo usuario
                    var suggestedGender = suggestedUser.Gender.GenderId;
                    var suggestedAge= DateTime.Today.AddTicks(suggestedUser.Birthday.Ticks).Year - 1;

                    // Calcular distancia
                    var distance = CalculateDistance.Calculate(item.Location.Longitude, suggestedUser.Location.Longitude, item.Location.Latitude, suggestedUser.Location.Latitude);
                    if(distance > mainPreferenceResponse.Distance) {continue;} //contemplar una tolerancia de +- 1km

                    if (!mainPreferenceResponse.GendersPreferencesId.Contains(suggestedGender) && mainPreferenceResponse.GendersPreferencesId.Count>0) { continue; } // Si no es del genero en preferencia y si tiene preferencias de genero, saltea
                    if(suggestedAge < mainPreferenceResponse.SinceAge || suggestedAge > mainPreferenceResponse.UntilAge) { continue; } //Si no esta dentro de la edad preferida, saltea
                    var suggestedInterest = suggestedPreference.OwnInterestPreferencesId;
                    var mainInterest = mainPreferenceResponse.InterestPreferencesId;

                    
                    foreach(int main in mainInterest)
                    {
                        foreach (int sugg in suggestedInterest)
                        {
                            if ( sugg.Equals(main))
                            {
                                Suggestion suggestion = new Suggestion()
                                {
                                    MainUser = mainUserId,
                                    SuggestedUser = suggestedUser.UserId,
                                    DateView = null,
                                    View = false
                                };
                                await _suggestionCommand.InsertSuggestion(suggestion);
                            }
                        }
                    }
                        
                    
                }
            }
        }


        public async Task<List<UserResponse>> ConvertJsonToUserResponse(JsonDocument jsonUser)
        {
            var listResponse = new List<UserResponse>();
            var list = JsonObject.Parse(jsonUser.RootElement.ToString()).AsArray();

            for (int i = 0; i < list.Count; i++)
            {
                UserResponse mapp = new UserResponse()
                {
                    UserId = list[i]["userId"] != null ? (int)list[i]["userId"] : 0,
                    Name = list[i]["name"] != null ? (string)list[i]["name"] : "",
                    LastName = list[i]["lastName"] != null ? (string)list[i]["lastName"] : "",
                    Birthday = list[i]["birthday"] != null ? (DateTime)list[i]["birthday"] : DateTime.MinValue,
                    Description = list[i]["description"] != null ? (string)list[i]["description"] : "",
                    Location = new LocationResponse(),
                    Images = new List<ImageResponse>(),
                    Gender = new GenderResponse(),
                };

                mapp.Location.Id = list[i]["location"]["id"] != null ? (int)list[i]["location"]["id"] : 0;
                mapp.Location.Latitude = list[i]["location"]["latitude"] != null ? (float)list[i]["location"]["latitude"] : 0;
                mapp.Location.Longitude = list[i]["location"]["longitude"] != null ? (float)list[i]["location"]["longitude"] : 0;
                mapp.Location.Address = list[i]["location"]["address"] != null ? (string)list[i]["location"]["address"] : "";

                mapp.Gender.GenderId = list[i]["gender"]["genderId"] != null ? (int)list[i]["gender"]["genderId"] : 0;
                mapp.Gender.Description = list[i]["gender"]["description"] != null ? (string)list[i]["gender"]["description"] : "";

                if (list[i]["images"] != null)
                {
                    foreach (var item in list[i]["images"].AsArray())
                    {
                        mapp.Images.Add(new ImageResponse
                        {
                            Id = item["id"] != null ? (int)item["id"] : 0,
                            Url = item["url"] != null ? (string)item["url"] : ""
                        });
                    }
                }

                listResponse.Add(mapp);
            }

            return listResponse;
        }
        public async Task<List<UserPreferencesResponse>> ConvertJsonToUserPreferencesResponse(JsonDocument jsonPreference)
        {
            var listResponse= new List<UserPreferencesResponse>();
            var list = JsonObject.Parse(jsonPreference.RootElement.ToString()).AsArray();

            for(int i=0; i< list.Count; i++)
            {
                UserPreferencesResponse mapp = new UserPreferencesResponse()
                {
                    UserId = list[i]["userId"] != null ? (int)list[i]["userId"] : 0,
                    SinceAge = list[i]["sinceAge"] != null ? (int)list[i]["sinceAge"] : 0,
                    UntilAge = list[i]["untilAge"] != null ? (int)list[i]["untilAge"] : 99,
                    Distance = list[i]["distance"] != null ? (int)list[i]["distance"] : 0,
                    GendersPreferencesId= new List<int>(),
                    InterestPreferencesId= new List<int>(),
                    OwnInterestPreferencesId= new List<int>()
                };
                if(list[i]["gendersPreferencesId"] != null)
                {
                    foreach (var item in list[i]["gendersPreferencesId"].AsArray())
                    {
                        mapp.GendersPreferencesId.Add((int)item);
                    }
                }
                
                if (list[i]["interestPreferencesId"] != null)
                {
                    foreach (var item in list[i]["interestPreferencesId"].AsArray())
                    {
                        mapp.InterestPreferencesId.Add((int)item);
                    }
                }
                if (list[i]["ownInterestPreferencesId"] != null)
                {
                    foreach (var item in list[i]["ownInterestPreferencesId"].AsArray())
                    {
                        mapp.OwnInterestPreferencesId.Add((int)item);
                    }
                }
                listResponse.Add(mapp);
            }

            return listResponse;
        }

        public Task<int> CountSuggestion(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
