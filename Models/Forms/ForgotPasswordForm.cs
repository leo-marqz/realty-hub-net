using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealtyHub.Models.Forms
{
    public class ForgotPasswordForm
    {
        [Display(Name = "Correo Electronico")]
        [Required(ErrorMessage = "Correo Electronico es requerido.")]
        [EmailAddress(ErrorMessage = "Correo Electronico es inválido.")]
        public string Email { get; set; } = string.Empty;
    }
}