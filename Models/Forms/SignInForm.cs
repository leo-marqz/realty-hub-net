
using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Models.Forms
{
    public class SignInForm
    {
        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "Correo Electronico es requerido.")]
        [EmailAddress(ErrorMessage = "Correo Electronico es inválido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password, ErrorMessage = "La contraseña es inválida.")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; } = false;
    }
}