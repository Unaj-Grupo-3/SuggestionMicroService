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
            List<int> prueba = new List<int> { 6, 7, 8 }; //borrar

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

            if (userList.Count > 1) 
            {
                for (int i = 1; i < userList.Count; i++)
                {
                    SuggestedUser suggestedUser = new SuggestedUser
                    {
                        Id = suggestions[i-1].Id,
                        User = userList[i],
                        DateView = suggestions[i-1].DateView,
                        View = suggestions[i-1].View,
                    };
                    suggestedUsers.Add(suggestedUser);
                };
            };

            SuggestionResponse response = new()
            {
                MainUser = userList[0],
                SuggestedUsers = suggestedUsers,
            };

            return response;
        }
    }
}
         