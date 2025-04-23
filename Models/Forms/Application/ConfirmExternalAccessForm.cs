
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Models.Forms.Application
{
    public class ConfirmExternalAccessForm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}