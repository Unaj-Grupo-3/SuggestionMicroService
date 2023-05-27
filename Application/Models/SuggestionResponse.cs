
namespace Application.Models
{
    public class SuggestionResponse
    {
        public UserResponse MainUser { get; set; }
        // public IList<SuggestedUser> SuggestedUsers { get; set; } 
        public IList<UserSuggestedRespose> SuggestedUsers { get; set; }
    }
}
