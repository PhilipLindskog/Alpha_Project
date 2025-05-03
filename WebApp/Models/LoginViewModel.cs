using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class LoginViewModel
{
    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    [Required(ErrorMessage = "Required")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter password")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "Invalid password")]
    [Required(ErrorMessage = "Required")]
    public string Password { get; set; } = null!;

    public bool IsPersistent { get; set; }
}
