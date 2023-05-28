using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<List<(int MainUser, int CountSuggestions)>> CountSuggestionsUsers()
        {
            var query = await (from s in _context.Suggestions
                        group s by s.MainUser into s2
                        select new { MainUser = s2.Key, CountSuggestions = s2.Count() }).ToListAsync();
            List<(int, int)> list = new List<(int, int)>();

            foreach (var item in query)
            {
                list.Add((item.MainUser, item.CountSuggestions));
            }
            return list;
        }
    }
}
