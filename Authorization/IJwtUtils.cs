using OntrackDb.Authentication;
using OntrackDb.Model;
using System.Threading.Tasks;

namespace OntrackDb.Authorization
{
    public interface IJwtUtils
    {
        public Task<string> GenerateToken(LoginModel model);
        string ValidateToken(string token);
        string GetRole(string token);

        Task<string> GenerateTokenForVerification(string email, int duration);
    }
}
