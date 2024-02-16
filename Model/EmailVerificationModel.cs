using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Model;

public class EmailVerificationModel
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; }
    
    [Required(ErrorMessage = "Device Id is required")]
    public string DeviceId { get; set; }
}
