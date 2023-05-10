using Application.Interfaces;
using Application.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Infrastructure.Queries
{
    public class SuggestionQueries : ISuggestionQueries
    {
        private readonly AppDbContext _context;

        public SuggestionQueries(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IList<SuggestionResponse>> GetAllSuggestions()
        {
            IList<SuggestionResponse> sugg = await _context.Suggestions
                .Select(e => new SuggestionResponse
                {
                    Id = e.Id,
                    MainUser = e.MainUser,
                    SuggestedUser = e.SuggestedUser,
                    View = e.View,
                })
                .ToListAsync();
            return sugg;
        }

        public async Task<IList<SuggestionResponse>> GetSuggestionsByUserId(int userId)
        {
            IList<SuggestionResponse> sugg = await _context.Suggestions
                .Where(s => s.MainUser == userId)
                .Select(s => new SuggestionResponse
                {
                    Id= s.Id,
                    MainUser = s.MainUser,
                    SuggestedUser= s.SuggestedUser,
                    View= s.View,
                })
                .ToListAsync();
            return sugg;
        }
    }
}
