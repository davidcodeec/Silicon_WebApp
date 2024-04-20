using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities;

public class UserEntity : IdentityUser
{
    [ProtectedPersonalData]
    public string FirstName { get; set; } = null!;

    [ProtectedPersonalData]
    public string LastName { get; set;} = null!;

    [ProtectedPersonalData]
    public string? ProfileImage { get; set; } = "avatar.jpg";

    [ProtectedPersonalData]
    public string? Bio {  get; set; }

    public bool IsExternalAccount { get; set; } = false;


    public int? AddressId { get; set; }
    public AddressEntity? Address { get; set; }
}
