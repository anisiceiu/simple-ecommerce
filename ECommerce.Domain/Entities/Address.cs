using ECommerce.Domain;
using System.ComponentModel.DataAnnotations;
namespace ECommerce.Domain;
public class Address
{
    public int Id { get; set; }

    public string UserId { get; set; }

    [Required, MaxLength(300)]
    public string AddressLine { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(20)]
    public string PostalCode { get; set; }

    // Navigation
    public ApplicationUser User { get; set; }
}
