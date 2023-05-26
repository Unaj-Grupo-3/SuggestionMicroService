

namespace Application.Models
{
    public class InterestCategoryResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<PreferenceResponseFull> InterestPreferencesId { get; set; }
    }
}
