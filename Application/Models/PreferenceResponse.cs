namespace Application.Models
{
    public class PreferenceResponse
    {
        public int UserId { get; set; }
        public int SinceAge { get; set; } //OVERALL
        public int UntilAge { get; set; } //OVERALL
        public int Distance { get; set; } //OVERALL
        public IList<GenderPreferenceResponse>? GendersPreferences { get; set; } //GENDERS
        public IList<InterestCategoryResponse>? CategoryPreferences { get; set; }
    }
}
