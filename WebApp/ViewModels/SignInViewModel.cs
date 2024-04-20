using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class SignInViewModel
{
    [DataType(DataType.EmailAddress)]
    [Display(Name = "E-mail address", Prompt = "Enter your e-mail address", Order = 0)]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Password ", Prompt = "Enter your password", Order = 1)]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;

    [Display(Name = "Remember me ", Prompt = "Remember me",  Order = 2)]
    public bool IsPersistent { get; set; }
}
