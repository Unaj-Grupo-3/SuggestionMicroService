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
        private readonly IUserMatchApiServices _userMatchApiServices;
        private readonly ISuggestionCommands _suggestionCommand;
        private readonly ISuggestionQueries _suggestionQueries;
        private string? _message;
        private int _statusCode;

        public SuggestionWorkerServices(IUserApiServices userApiServices, IPreferenceApiServices preferenceApiServices, ISuggestionCommands suggestionCommand, ISuggestionQueries suggestionQueries, IUserMatchApiServices userMatchApiServices)
        {
            _preferenceApiServices = preferenceApiServices;
            _userApiServices = userApiServices;
            _suggestionCommand = suggestionCommand;
            _suggestionQueries = suggestionQueries;
            _userMatchApiServices = userMatchApiServices;
        }
        public SuggestionWorkerServices() { }

        public async Task GenerateSuggestionXUser(int userId)
        {
            try
            {
                await _suggestionCommand.DeleteWorkerSuggByUserId(userId); // borra todas las sugerencias por ID 
                var responseUser = await _userApiServices.GetAllUsersObj();
                var responsePreference = await _preferenceApiServices.GetAllPreferenceObj();
                var responseUserMatch = await _userMatchApiServices.GetAllMatches();

                var mainUser = responseUser.FirstOrDefault(x => x.UserId == userId); // Response de usuario
                var mainPreference = responsePreference.FirstOrDefault(x => x.UserId == userId); // Response de preferencias de usuario
                var mainAge = DateTime.Today.AddTicks(-mainUser.Birthday.Ticks).Year - 1; // Edad del main User
                var mainGender = mainUser.Gender.GenderId; // Genero del main User
                var suggestionsUser = await _suggestionQueries.GetSuggestionsByUserId(userId); // Sugerencias ya calculadas del usuario
                var lsUser1Match = responseUserMatch.Where(x => x.User1 == userId).ToList();// lista de matches del usuario param

                foreach (var item in responseUser)
                {
                    var suggestedUser = responseUser.FirstOrDefault(x => x.UserId == item.UserId);
                    if (suggestedUser.UserId == userId) { continue; } // No calculamos a la misma persona parametrizada
                    if (suggestionsUser.Select(x => x.SuggestedUser).ToList().Contains(item.UserId)) { continue; } // Si ya tiene al usuario calculado, lo ignora
                    

                    if (lsUser1Match.Exists(x=> x.User2 == item.UserId && (x.LikeUser1 == 1 || x.LikeUser2 == -1))) { continue; } // Si el usuario sugerido esta en la lista de likes del usuario param, lo ignora
                    if (lsUser1Match.Exists(x => x.User2 == item.UserId && x.LikeUser1 == -1 )) { continue; } // Si el usuario sugerido esta en la lista de DontLikes del usuario param, lo ignora

                    if (mainPreference == null) // Si el usuario no tiene preferencias, se le calcula una sugerencia igual
                    {
                        Suggestion suggestion = new Suggestion()
                        {
                            MainUser = mainUser.UserId,
                            SuggestedUser = suggestedUser.UserId,
                            DateView = null,
                            View = false
                        };
                        await _suggestionCommand.InsertSuggestion(suggestion);
                        continue;
                    }
                    // Calcular distancia
                    var distance = CalculateDistance.Calculate(mainUser.Location.Longitude, item.Location.Longitude, mainUser.Location.Latitude, item.Location.Latitude);
                    if (distance >= mainPreference.Distance + 1) { continue; } //contemplar una tolerancia de +- 1km

                    var suggestedAge = DateTime.Today.AddTicks(-item.Birthday.Ticks).Year - 1; // Edad del Suggested User
                    var suggestedGender = item.Gender.GenderId; // Genero del Suggested User

                    if (!mainPreference.GendersPreferencesId.Contains(suggestedGender) && mainPreference.GendersPreferencesId.Count > 0) { continue; } // Si no es del genero en preferencia y si tiene preferencias de genero, saltea
                    if (suggestedAge < mainPreference.SinceAge || suggestedAge > mainPreference.UntilAge) { continue; } //Si no esta dentro de la edad preferida, saltea
                    var suggestedPreference = responsePreference.FirstOrDefault(x => x.UserId == item.UserId);

                    if (suggestedPreference == null) { continue; }

                    var suggestedInterest = suggestedPreference.OwnInterestPreferencesId; // Intereses del sugerido
                    var mainInterest = mainPreference.InterestPreferencesId; // Intereses del Main User

                    bool flagFoundSuggested = false; //Flag para cortar el bucle de busqueda de mainInterest
                                                     // Recorre todos los intereses y si coincide alguno, lo toma como sugerencia
                    foreach (int main in mainInterest)
                    {
                        if (flagFoundSuggested || suggestedInterest.Count.Equals(0)) { break; }
                        foreach (int sugg in suggestedInterest)
                        {
                            if (sugg.Equals(main))
                            {
                                Suggestion suggestion = new Suggestion()
                                {
                                    MainUser = mainUser.UserId,
                                    SuggestedUser = suggestedUser.UserId,
                                    DateView = null,
                                    View = false
                                };
                                await _suggestionCommand.InsertSuggestion(suggestion);
                                flagFoundSuggested = true;
                                break;
                            }
                        }
                    }

                }
            }
            catch(Exception e)
            {
                _message = e.Message;
            }


        }

        public async Task GenerateSuggestionAll()
        {
            try
            {
                // borrado general en full - borrado por ID en comun
                await _suggestionCommand.DeleteSuggestionAll();
                // Hay que insertar los casos de sugerencias
                var jsonUser = await _userApiServices.GetAllUsersObj();
                var jsonPreference = await _preferenceApiServices.GetAllPreferenceObj();

                foreach (UserResponse item in jsonUser)
                {
                    var mainUserId = item.UserId;
                    var mainPreferenceResponse = jsonPreference.FirstOrDefault(x => x.UserId == mainUserId);
                    var mainAge = DateTime.Today.AddTicks(-item.Birthday.Ticks).Year - 1;
                    var mainGender = item.Gender.GenderId;

                    foreach (UserPreferencesResponse suggestedPreference in jsonPreference)
                    {
                        if (mainUserId.Equals(suggestedPreference.UserId)) { continue; } // No calcula el mismo usuario
                        var suggestedUser = jsonUser.FirstOrDefault(x => x.UserId == suggestedPreference.UserId);
                        if (mainPreferenceResponse == null) // Si el usuario principal no tiene preferencias, se sugiere a todos los que si tienen preferencias
                        {
                            Suggestion suggestion = new Suggestion()
                            {
                                MainUser = mainUserId,
                                SuggestedUser = suggestedUser.UserId,
                                DateView = null,
                                View = false
                            };
                            await _suggestionCommand.InsertSuggestion(suggestion);
                            continue;
                        }

                        var suggestedGender = suggestedUser.Gender.GenderId;
                        var suggestedAge = DateTime.Today.AddTicks(-suggestedUser.Birthday.Ticks).Year - 1;

                        // Calcular distancia
                        var distance = CalculateDistance.Calculate(item.Location.Longitude, suggestedUser.Location.Longitude, item.Location.Latitude, suggestedUser.Location.Latitude);
                        if (distance >= mainPreferenceResponse.Distance + 1) { continue; } //contemplar una tolerancia de +- 1km

                        if (!mainPreferenceResponse.GendersPreferencesId.Contains(suggestedGender) && mainPreferenceResponse.GendersPreferencesId.Count > 0) { continue; } // Si no es del genero en preferencia y si tiene preferencias de genero, saltea
                        if (suggestedAge < mainPreferenceResponse.SinceAge || suggestedAge > mainPreferenceResponse.UntilAge) { continue; } //Si no esta dentro de la edad preferida, saltea
                        var suggestedInterest = suggestedPreference.OwnInterestPreferencesId;
                        var mainInterest = mainPreferenceResponse.InterestPreferencesId;

                        bool flagFoundSuggested = false; //Flag para cortar el bucle de busqueda de mainInterest
                                                         // Recorre todos los intereses y si coincide alguno, lo toma como sugerencia
                        foreach (int main in mainInterest)
                        {
                            if (flagFoundSuggested || suggestedInterest.Count.Equals(0)) { break; }
                            foreach (int sugg in suggestedInterest)
                            {
                                if (sugg.Equals(main))
                                {
                                    Suggestion suggestion = new Suggestion()
                                    {
                                        MainUser = mainUserId,
                                        SuggestedUser = suggestedUser.UserId,
                                        DateView = null,
                                        View = false
                                    };
                                    await _suggestionCommand.InsertSuggestion(suggestion);
                                    flagFoundSuggested = true;
                                    break;
                                }
                            }
                        }


                    }
                }
            }
            catch(Exception e)
            {
                _message = e.Message;
            }
        }

        //Usuarios nuevos no calculados por el worker
        public async Task<List<int>> UsersNew() 
        {
            try
            {
                var responseUser = await _userApiServices.GetAllUsersObj();
                var count = await _suggestionQueries.CountSuggestionsUsers();
                var usersNew = new List<int>();
                foreach(var user in responseUser)
                {
                    var suggestedExist = count.Select(u => u.MainUser).Contains(user.UserId);
                    if (!suggestedExist) // Si el usuario de la Lista de Usuarios no tiene alguna sugerencia calculada, se considera nuevo
                    {
                        usersNew.Add(user.UserId);
                    }
                }

                return usersNew;
            }
            catch(Exception e)
            {
                _message = e.Message;
                return new List<int>();
            }
        }

        public async Task<List<int>> CountSuggestionsUsers(int valueRecalculate)
        {
            try 
            { 
                var count = await _suggestionQueries.CountSuggestionsUsers();
                var listRecalculate = count.Where(w => w.CountSuggestions <= valueRecalculate).Select(x => x.MainUser).ToList();

                return listRecalculate;
            }
            catch (Exception e)
            {
                _message = e.Message;
                return new List<int>();
            }
        }
        
    }
}
