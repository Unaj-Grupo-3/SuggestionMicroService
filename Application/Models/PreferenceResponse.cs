namespace Application.Models
{
    public class PreferenceResponse
    {
        public int UserId { get; set; }
        public int SinceAge { get; set; } //OVERALL
        public int UntilAge { get; set; } //OVERALL
        public int Distance { get; set; } //OVERALL
        public IEnumerable<GenderPreferenceResponse>? GendersPreferencesId { get; set; } //GENDERS
        public IEnumerable<InterestCategoryResponse>? CategoryPreferencesId { get; set; }
    }
}
