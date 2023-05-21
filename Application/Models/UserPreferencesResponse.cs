namespace Application.Models
{
    public class UserPreferencesResponse
    {
        public int UserId { get; set; }
        public int SinceAge { get; set; } //OVERALL
        public int UntilAge { get; set; } //OVERALL
        public int Distance { get; set; } //OVERALL
        public List<int>? GendersPreferencesId { get; set; } //GENDERS
        public List<int>? InterestPreferencesId { get; set; } //PREFERENCES -> Where Like=1
        public List<int>? OwnInterestPreferencesId { get; set; }
    }
}
