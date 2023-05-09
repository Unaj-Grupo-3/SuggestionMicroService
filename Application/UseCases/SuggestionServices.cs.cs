using Application.Interfaces;
using Application.Models;

namespace Application.UseCases
{
    public class SuggestionServices : ISuggestionServices
    {
        private readonly ISuggestionCommands _commands;
        private readonly ISuggestionQueries _queries;

        public SuggestionServices(ISuggestionCommands commands, ISuggestionQueries queries)
        {
            _commands = commands;
            _queries = queries;
        }

        public async Task<IList<SuggestionResponse>> GetAll()
        {
            IList<SuggestionResponse> sugg = await _queries.GetAllSuggestions();
            return sugg;
        }
    }
}
