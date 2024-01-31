﻿using Application.Interfaces;
using Application.Models;
using Domain.Entities;
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
                
                var responseUser = await _userApiServices.GetAllUsersObj();
                var responsePreference = await _preferenceApiServices.GetAllPreferenceObj();
                var responseUserMatch = await _userMatchApiServices.GetAllMatches();

                var mainUser = responseUser.FirstOrDefault(x => x.UserId == userId); // Response de usuario
                var mainPreference = responsePreference.FirstOrDefault(x => x.UserId == userId); // Response de preferencias de usuario
                var mainAge = DateTime.Today.AddTicks(-mainUser.Birthday.Ticks).Year - 1; // Edad del main User
                var mainGender = mainUser.Gender.GenderId; // Genero del main User
                var suggestionsUser = await _suggestionQueries.GetSuggestionsByUserId(userId); // Sugerencias ya calculadas del usuario
                var lsUser1Match = responseUserMatch.Where(x => x.User1 == userId || x.User2 == userId).ToList();// lista de matches del usuario param

                var usersFiltred = new List<int>();
                var genderPreference = new List<int>() { -1 };


                if (mainPreference == null)
                {
                    var edad = mainAge;
                    var minEdad = mainAge - 10 < 18 ? 18 : mainAge - 10;
                    var maxEdad = mainAge + 10;
                    var distance = 15;
                    usersFiltred = await _userApiServices.GetAllUsersIdsByFilters(genderPreference,
                                                                                  minEdad,
                                                                                  maxEdad,
                                                                                  distance,
                                                                                  mainUser.Location.Longitude,
                                                                                  mainUser.Location.Latitude);
                }
                else
                {
                    usersFiltred = await _userApiServices.GetAllUsersIdsByFilters(mainPreference.GendersPreferencesId.Count == 0? genderPreference : mainPreference.GendersPreferencesId , 
                                                                                  mainPreference.SinceAge, 
                                                                                  mainPreference.UntilAge, 
                                                                                  mainPreference.Distance ,
                                                                                  mainUser.Location.Longitude, 
                                                                                  mainUser.Location.Latitude);
                }
                
                foreach (var suggId in usersFiltred)
                {

                    if (suggId == userId) { continue; } // No calculamos a la misma persona parametrizada

                    if (suggestionsUser.Select(x => x.SuggestedUser).ToList().Contains(suggId)) { continue; } // Si ya tiene al usuario calculado, lo ignora                    


                    var checkLike1 = lsUser1Match.FirstOrDefault( x => x.User2 == suggId && x.LikeUser1 != 0);
                    var checkLike2 = lsUser1Match.FirstOrDefault( x => x.User1 == suggId && x.LikeUser2 != 0);

                    if (checkLike1 != null || checkLike2 != null) { continue; } // Si el usuario param ya dio like o dislike al usuario, lo ignora

                    if (mainPreference == null) // Si el usuario no tiene preferencias, se le calcula una sugerencia igual
                    {
                        await InsertSuggestionDefault(mainUser.UserId, suggId);
                        continue;
                    }

                    var suggestedPreference = responsePreference.FirstOrDefault(x => x.UserId == suggId);

                    if (suggestedPreference == null) { continue; }

                    var suggestedInterest = suggestedPreference.OwnInterestPreferencesId; // Intereses del sugerido
                    var mainInterest = mainPreference.InterestPreferencesId; // Intereses del Main User

                    if (mainPreference.InterestPreferencesId.Count.Equals(0) || mainPreference.InterestPreferencesId.Count < 10)
                    {
                        await InsertSuggestionDefault(mainUser.UserId, suggId);
                        continue;
                    }

                    bool flagFoundSuggested = false; //Flag para cortar el bucle de busqueda de mainInterest
                                                     // Recorre todos los intereses y si coincide alguno, lo toma como sugerencia
                    foreach (int main in mainInterest)
                    {
                        if (flagFoundSuggested || suggestedInterest.Count.Equals(0)) { break; }
                        foreach (int sugg in suggestedInterest)
                        {
                            if (sugg.Equals(main))
                            {
                                await InsertSuggestionDefault(mainUser.UserId, suggId);
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

        public async Task InsertSuggestionDefault(int UserId, int SuggestedUserId)
        {
            Suggestion suggestion = new Suggestion()
            {
                MainUser = UserId,
                SuggestedUser = SuggestedUserId,
                DateView = null,
                View = false
            };
            await _suggestionCommand.InsertSuggestion(suggestion);
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

        public async Task DeleteSuggestionsAll()
        {
            try
            {
                await _suggestionCommand.DeleteSuggestionAll();
            }
            catch (Exception e)
            {
                _message = e.Message;
            }
        }


        public async Task<bool> DeleteSuggestionsById(int userId)
        {
            try
            {
                await _suggestionCommand.DeleteWorkerSuggByUserId(userId);
                await GenerateSuggestionXUser(userId);
                return true;
            }
            catch (Exception e)
            {
                _message = e.Message;
                return false;
            }
        }
    }
}
