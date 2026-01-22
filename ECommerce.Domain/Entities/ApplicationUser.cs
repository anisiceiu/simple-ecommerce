using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
namespace ECommerce.Domain;
public class ApplicationUser : IdentityUser
{
    // Future extension point (Phone, Address, etc.)
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public override string PhoneNumber { get; set; }
    public ICollection<Address> Addresses { get; set; }
}