using Application.Models;

namespace Application.Interfaces
{
    public interface ISuggestionQueries
    {
        Task<IList<SuggestionResponse>> GetAllSuggestions();
        Task<IList<SuggestionResponse>> GetSuggestionsByUserId(int userId);


    }
}
