

namespace Application.Interfaces
{
    public interface ISuggestionWorkerServices
    {
        public Task GenerateSuggestionAll();
        public Task GenerateSuggestionXUser(int userId);
        public Task<List<int>> CountSuggestionsUsers(int valueRecalculate);
        public Task DeleteSuggestionsAll();
        public Task<bool> DeleteSuggestionsById(int userId);
        public Task<List<int>> UsersNew();
    }
}
