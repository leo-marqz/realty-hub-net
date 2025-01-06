using System.ComponentModel.DataAnnotations;

namespace RealtyHub.Models.Forms.AuthForms
{
    public class ResetPasswordForm
    {
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password, ErrorMessage = "La contraseña no es válida")]
        public string? Password { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [Required(ErrorMessage = "La confirmación de la contraseña es requerida")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El código es requerido")]
        public string? Code { get; set; }

    }
}