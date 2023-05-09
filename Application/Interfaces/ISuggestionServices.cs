using Application.Models;

namespace Application.Interfaces
{
    public interface ISuggestionServices
    {
        public Task<IList<SuggestionResponse>> GetAll();

    }
}
