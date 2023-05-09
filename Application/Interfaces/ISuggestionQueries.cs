using Application.Models;

namespace Application.Interfaces
{
    public interface ISuggestionQueries
    {
        public Task<IList<SuggestionResponse>> GetAllSuggestions();

    }
}
