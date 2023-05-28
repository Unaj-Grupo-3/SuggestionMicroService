using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISuggestionQueries
    {
        Task<IList<Suggestion>> GetAllSuggestions();
        Task<IList<Suggestion>> GetSuggestionsByUserId(int userId);
        Task<List<(int MainUser, int CountSuggestions)>> CountSuggestionsUsers();

    }
}
