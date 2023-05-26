using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Models
{
    public class SuggestionResponse
    {
        public UserResponse MainUser { get; set; }
        public IList<SuggestedUser> SuggestedUsers { get; set; } 

    }
}
