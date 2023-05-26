namespace Application.Models
{
    public class SuggestedUser
    {
        public int Id { get; set; }
        public UserResponse User { get; set; }
        public DateTime? DateView { get; set; }
        public bool View { get; set; }

    }
}
