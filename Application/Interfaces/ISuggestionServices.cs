using Application.Models;

namespace Application.Interfaces
{
    public interface ISuggestionServices
    {
        public Task<IList<SuggestionResponse>> GetAll();
        Task<IList<SuggestionResponse>> GetSuggestionsByUserId(int userId);
    }
}
