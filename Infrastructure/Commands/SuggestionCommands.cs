

using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Commands
{
    public class SuggestionCommands : ISuggestionCommands
    {
        private readonly AppDbContext _context;

        public SuggestionCommands(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteSuggestion(Suggestion suggestion)
        {
            _context.Suggestions.Remove(suggestion);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Suggestion> InsertSuggestion(Suggestion suggestion)
        {
            _context.Add(suggestion);

            await _context.SaveChangesAsync();

            return suggestion;
        }

        public async Task<Suggestion> UpdateSuggestion(Suggestion suggestion)
        {
            var suggestionUpdate = (from s in _context.Suggestions
                                    where s.Id.Equals(suggestion.Id)
                                    select s).FirstOrDefault();
            suggestionUpdate.MainUser = suggestion.MainUser != null ? suggestion.MainUser : suggestionUpdate.MainUser;
            suggestionUpdate.SuggestedUser = suggestion.SuggestedUser != null ? suggestion.SuggestedUser : suggestionUpdate.SuggestedUser;
            suggestionUpdate.DateView = suggestion.DateView != null ? suggestion.DateView : suggestionUpdate.DateView;
            suggestionUpdate.View= suggestion.View != null ? suggestion.View : suggestionUpdate.View;

            await _context.SaveChangesAsync();
            return suggestionUpdate;


        }
    }
}
