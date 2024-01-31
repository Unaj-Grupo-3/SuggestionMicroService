using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISuggestionServices
    {
        public Task<IList<Suggestion>> GetAll();
        public Task<SuggestionResponse> GetSuggestionsByUserId(int userIds);
        public Task<bool> DeleteWorkerSuggByUserIdAndUserSuggested(int userId, int userSuggested);

    }
}
