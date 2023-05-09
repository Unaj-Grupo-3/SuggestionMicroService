namespace Application.Models
{
    public class SuggestionResponse
    {
        public int Id { get; set; }
        public int MainUser { get; set; }
        public int SuggestedUser { get; set; }
        public bool View { get; set; }

    }
}
