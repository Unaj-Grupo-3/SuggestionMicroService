using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;

namespace Application.UseCases
{
    public class SuggestionServices : ISuggestionServices
    {
        private readonly ISuggestionCommands _commands;
        private readonly ISuggestionQueries _queries;
        private readonly IUserApiServices _userApiServices;
        private readonly IPreferenceApiServices _preferenceApiServices;

        public SuggestionServices(ISuggestionCommands commands, ISuggestionQueries queries, IUserApiServices userApiServices, IPreferenceApiServices preferenceApiServices)
        {
            _commands = commands;
            _queries = queries;
            _userApiServices = userApiServices;
            _preferenceApiServices = preferenceApiServices;
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

            //Info de lista de preferencias
            IList<PreferenceResponse> preferenceList = new List<PreferenceResponse>();
            preferenceList = await _preferenceApiServices.GetPreferencesByList(ids);

            //Response
            //IList<SuggestedUser> suggestedUsers = new List<SuggestedUser>();
            IList<UserSuggestedRespose> suggestedUsers = new List<UserSuggestedRespose>();
            IList<UserPreferencesResponse> suggestedPreference = new List<UserPreferencesResponse>();

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

                    UserSuggestedRespose userSuggestedRespose = new UserSuggestedRespose()
                    {
                        UserId = userList[i].UserId,
                        Name = userList[i].Name,
                        LastName = userList[i].LastName,
                        Description = userList[i].Description,
                        Birthday = userList[i].Birthday,
                        Location = userList[i].Location.Address,
                        Distance = distance,
                        Gender = userList[i].Gender,
                        Images = userList[i].Images,
                        OurPreferences = new PreferenceSuggestedResponse()
                        {
                            OwnCategoryPreferences = new List<InterestCategoryResponse>(),
                        }
                    };

                    var PreferenceByUserId = preferenceList.FirstOrDefault(x => x.UserId == userList[i].UserId);

                    foreach (var category in PreferenceByUserId.CategoryPreferences)
                    {
                        category.InterestPreferencesId = category.InterestPreferencesId.Where(x => x.OwnInterest).ToList();
                    }

                    userSuggestedRespose.OurPreferences.OwnCategoryPreferences = PreferenceByUserId.CategoryPreferences;

                    //var intereses = PreferenceByUserId.CategoryPreferences.SelectMany(c => c.InterestPreferencesId)
                    //        .Where(x => x.OwnInterest == true).ToList();



                    //userSuggestedRespose.OurPreferences.OwnCategoryPreferences = intereses;

                    //SuggestedUser suggestedUser = new SuggestedUser
                    //{
                    //    Id = suggestions[i].Id,
                    //    User = userList.FirstOrDefault(x => x.UserId == suggestions[i].SuggestedUser),
                    //    DateView = suggestions[i].DateView,
                    //    View = suggestions[i].View,
                    //    Distance = distance,
                    //    Preferences = null
                    //};

                   suggestedUsers.Add(userSuggestedRespose);
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
         