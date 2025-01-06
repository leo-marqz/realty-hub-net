using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Models.Forms.AuthForms
{
    public class SignUpForm
    {
        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "Nombre es requerido.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nombre debe tener entre 3 y 50 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "Correo Electronico es requerido.")]
        [EmailAddress(ErrorMessage = "Correo Electronico es inválido.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password, ErrorMessage = "La contraseña es inválida.")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirmar Contraseña")]
        [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}