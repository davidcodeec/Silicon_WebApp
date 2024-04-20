using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class AccountDetailsViewModel
{
    public bool IsExternalAccount { get; set; }
    public AccountBasicInfo? Basic { get; set; }
    public AccountAddressInfo? Address { get; set; }
}

public class AccountBasicInfo
{
    [DataType(DataType.Text)]
    [Display(Name = "First Name", Prompt = "Enter your first name", Order = 0)]
    [Required(ErrorMessage = "First name is required")]
    [MinLength(2, ErrorMessage = "Enter a valid firstname")]
    public string FirstName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Last Name", Prompt = "Enter your last name", Order = 1)]
    [Required(ErrorMessage = "Last name is required")]
    [MinLength(2, ErrorMessage = "Enter a valid lastname")]
    public string LastName { get; set; } = null!;

    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email Address", Prompt = "Enter your email address", Order = 2)]
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(?:\.[a-zA-Z]{2,})?$", ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = null!;

    [Display(Name = "Phone (optional)", Prompt = "Enter your phone", Order = 3)]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Bio (optional)", Prompt = "Add a short bio....", Order = 4)]
    [DataType(DataType.MultilineText)]
    public string? Bio {  get; set; }
}


public class AccountAddressInfo
{
    [Display(Name = "Address line 1", Prompt = "Enter your first address line", Order = 0)]
    [Required(ErrorMessage = "Address is required")]
    [MinLength(2, ErrorMessage = "Enter a valid firstname")]
    public string AddressLine_1 { get; set; } = null!;

    [Display(Name = "Address line 2", Prompt = "Enter your second address line", Order = 1)]
    public string? AddressLine_2 { get; set;}

    [Display(Name = "Postal Code", Prompt = "Enter your postal code", Order = 2)]
    [Required(ErrorMessage = "Postal code is required")]
    [MinLength(2, ErrorMessage = "Enter a valid valid postal code")]
    public string PostalCode { get; set; } = null!;

    [Display(Name = "City", Prompt = "Enter first address line", Order = 3)]
    [Required(ErrorMessage = "City is required")]
    [MinLength(2, ErrorMessage = "Enter a valid city")]
    public string City { get; set; } = null!;
}