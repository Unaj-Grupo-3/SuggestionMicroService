using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using System.Text.Json;

namespace Application.UseCases
{
    public class SuggestionServices : ISuggestionServices
    {
        private readonly ISuggestionCommands _commands;
        private readonly ISuggestionQueries _queries;
        private readonly IUserApiServices _userApiServices;


        public SuggestionServices(ISuggestionCommands commands, ISuggestionQueries queries, IUserApiServices userApiServices)
        {
            _commands = commands;
            _queries = queries;
            _userApiServices = userApiServices;
        }

        public async Task<IList<Suggestion>> GetAll()
        {
            IList<Suggestion> sugg = await _queries.GetAllSuggestions();
            return sugg;
        }



        public async Task<SuggestionResponse> GetSuggestionsByUserId(int userIds)
        {
            SuggestionResponse response = new();

            //lista de ids [user, sug...]            
            List<int> ids = new();
            ids.Add(userIds);
            IList<Suggestion> suggestions = new List<Suggestion>();
            suggestions = await _queries.GetSuggestionsByUserId(userIds);
            if(suggestions != null)
            {
                for (int i = 0; i < suggestions.Count; i++)
                {
                    ids.Add(suggestions[i].SuggestedUser);
                }
            }

            //Info de lista de usuarios
            IList<UserResponse> userList = new List<UserResponse>();
            userList = await _userApiServices.GetUsersByList(ids);

            //response
            IList<SuggestedUser> suggestedUsers = new List<SuggestedUser>();

            UserResponse main = userList.FirstOrDefault(x => x.UserId == userIds);
            userList = userList.Where(x => x.UserId != userIds).ToList();

            if (userList != null && userList.Count > 0) 
            {
                double longitud1 = main.Location.Longitude;
                double latitud1 = main.Location.Latitude;

                for (int i = 0; i < userList.Count; i++)
                {
                    double longitud2 = userList[i].Location.Longitude;
                    double latitud2 = userList[i].Location.Latitude;
                    int distance = CalculateDistance.Calculate(longitud1, longitud2, latitud1, latitud2);
                    SuggestedUser suggestedUser = new SuggestedUser
                    {
                        Id = suggestions[i].Id,
                        User = userList.FirstOrDefault(x => x.UserId == suggestions[i].SuggestedUser),
                        DateView = suggestions[i].DateView,
                        View = suggestions[i].View,
                        Distance = distance,
                        Preferences = null
                    };
                    suggestedUsers.Add(suggestedUser);
                };
                response = new()
                {
                    MainUser = main,
                    SuggestedUsers = suggestedUsers,
                };
            }

            return response;
        }
    }
}
         