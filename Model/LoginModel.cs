using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Device Id is required")]
        public string DeviceId { get; set; }
    }
}
