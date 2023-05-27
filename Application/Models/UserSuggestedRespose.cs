

namespace Application.Models
{
    public class UserSuggestedRespose
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int Distance { get; set; }
        public GenderResponse Gender { get; set; }
        public IList<ImageResponse> Images { get; set; }
        public PreferenceSuggestedResponse OurPreferences { get; set; }
    }
}
