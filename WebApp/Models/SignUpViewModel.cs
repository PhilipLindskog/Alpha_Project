using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SignUpViewModel
{
    [Display(Name = "First Name", Prompt = "Enter first name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Your first name is required")]
    public string FirstName { get; set; } = null!;

    [Display(Name = "Last Name", Prompt = "Enter last name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Your last name required")]
    public string LastName { get; set; } = null!;

    [Display(Name = "Email", Prompt = "Enter email address")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email")]
    [Required(ErrorMessage = "You have to enter a valid email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Password", Prompt = "Enter password")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*\d)(?=.*\W).{8,}$", ErrorMessage = "Invalid password")]
    [Required(ErrorMessage = "You have to enter a password")]
    public string Password { get; set; } = null!;

    [Display(Name = "Confirm Password", Prompt = "Confirm password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage ="Password don't match")]
    [Required(ErrorMessage = "You have to confirm your password")]
    public string ConfirmPassword { get; set; } = null!;

    [Range(typeof(bool), "true", "true")]
    public bool Terms { get; set; }
}
