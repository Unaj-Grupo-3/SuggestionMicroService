

namespace Domain.Entities
{
    public class Suggestion
    {
        public int Id { get; set; }
        public int MainUser { get; set; }
        public int SuggestedUser { get; set; }
        public DateTime? DateView { get; set; }
        public bool View { get; set; }

    }
}
