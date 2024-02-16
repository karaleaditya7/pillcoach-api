using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Model
{
    public class ResetPasswordModel
    {
        
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
