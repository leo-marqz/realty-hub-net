
using Microsoft.AspNetCore.Identity;

namespace RealtyHub.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}