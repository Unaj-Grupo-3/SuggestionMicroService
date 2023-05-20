using System.Security.Claims;

namespace Application.Interfaces
{
    public interface ITokenServices
    {
        bool ValidateUserId(ClaimsIdentity identity, int userId);
        int GetUserId(ClaimsIdentity identity);

    }
}
