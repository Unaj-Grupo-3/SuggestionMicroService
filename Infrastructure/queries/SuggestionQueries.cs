using Application.Interfaces;
using Application.Models;
using Domain.Entities;
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
        public async Task<IList<Suggestion>> GetAllSuggestions()
        {
            IList<Suggestion> sugg = await _context.Suggestions
                .ToListAsync();
            return sugg;
        }

        public async Task<IList<Suggestion>> GetSuggestionsByUserId(int userId)
        {
            IList<Suggestion> sugg = await _context.Suggestions
                .Where(s => s.MainUser == userId)
                .ToListAsync();
            return sugg;
        }
    }
}
