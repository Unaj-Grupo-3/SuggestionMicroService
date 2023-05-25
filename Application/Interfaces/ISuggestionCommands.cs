using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISuggestionCommands
    {
        public Task<Suggestion> InsertSuggestion(Suggestion suggestion);
        public Task<bool> DeleteSuggestion(Suggestion suggestion);
        public Task<bool> DeleteSuggestionAll();
        public Task<Suggestion> UpdateSuggestion(Suggestion suggestion);

    }
}
