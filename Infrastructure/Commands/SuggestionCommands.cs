

using Application.Interfaces;
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
    }
}
