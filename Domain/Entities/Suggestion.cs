

namespace Domain.Entities
{
    public class Suggestion
    {
        public int Id { get; set; }
        public Guid MainUser { get; set; }
        public Guid SuggestedUser { get; set; }
        public DateTime? DateView { get; set; }
        public bool View { get; set; }

    }
}
