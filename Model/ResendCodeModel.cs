using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Model;

public class ResendCodeModel
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}
