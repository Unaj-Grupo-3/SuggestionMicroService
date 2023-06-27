using Application.Models;
using Domain.Entities;
using System.Text.Json;

namespace Application.Interfaces
{
    public interface ISuggestionServices
    {
        public Task<IList<Suggestion>> GetAll();
        //Task<IList<SuggestionResponse>> GetSuggestionsByUserId(int userId);
        public Task<SuggestionResponse> GetSuggestionsByUserId(int userIds);
        public Task<bool> DeleteWorkerSuggByUserIdAndUserSuggested(int userId, int userSuggested);

    }
}
