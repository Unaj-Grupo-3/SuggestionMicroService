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
            var responseUser = await _userApiServices.GetAllUsersObj();
            var responsePreference = await _preferenceApiServices.GetAllPreferenceObj();
            var userMain = responseUser.FirstOrDefault(x => x.UserId == userId);
            var suggestionsUser = await _suggestionQueries.GetSuggestionsByUserId(userId);

            var countList = await _suggestionQueries.CountSuggestionsUsers();

            foreach(var item in countList)
            {

            }
        }

        public async Task GenerateSuggestionAll()
        {
            // borrado general en full - borrado por ID en comun
            await _suggestionCommand.DeleteSuggestionAll();
            // Hay que insertar los casos de sugerencias
            var jsonUser = await _userApiServices.GetAllUsersObj();
            var jsonPreference = await _preferenceApiServices.GetAllPreferenceObj();

            foreach(UserResponse item in jsonUser)
            {
                var mainUserId = item.UserId;
                var mainPreferenceResponse = jsonPreference.FirstOrDefault(x => x.UserId == mainUserId);
                var mainAge = DateTime.Today.AddTicks(-item.Birthday.Ticks).Year - 1;
                var mainGender = item.Gender.GenderId;

                foreach (UserPreferencesResponse suggestedPreference in jsonPreference)
                {
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
                    if (mainUserId.Equals(suggestedPreference.UserId)) { continue; } // No calcula el mismo usuario
                    var suggestedGender = suggestedUser.Gender.GenderId;
                    var suggestedAge= DateTime.Today.AddTicks(- suggestedUser.Birthday.Ticks).Year - 1;

                    // Calcular distancia
                    var distance = CalculateDistance.Calculate(item.Location.Longitude, suggestedUser.Location.Longitude, item.Location.Latitude, suggestedUser.Location.Latitude);
                    if(distance >= mainPreferenceResponse.Distance +1) {continue;} //contemplar una tolerancia de +- 1km

                    if (!mainPreferenceResponse.GendersPreferencesId.Contains(suggestedGender) && mainPreferenceResponse.GendersPreferencesId.Count>0) { continue; } // Si no es del genero en preferencia y si tiene preferencias de genero, saltea
                    if(suggestedAge < mainPreferenceResponse.SinceAge || suggestedAge > mainPreferenceResponse.UntilAge) { continue; } //Si no esta dentro de la edad preferida, saltea
                    var suggestedInterest = suggestedPreference.OwnInterestPreferencesId;
                    var mainInterest = mainPreferenceResponse.InterestPreferencesId;

                    bool flagFoundSuggested = false; //Flag para cortar el bucle de busqueda de mainInterest
                    // Recorre todos los intereses y si coincide alguno, lo toma como sugerencia
                    foreach(int main in mainInterest)
                    {
                        if (flagFoundSuggested || suggestedInterest.Count.Equals(0)) { break; }
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
                                flagFoundSuggested = true;
                                break;
                            }
                        }
                    }
                        
                    
                }
            }
        }
        
    }
}
