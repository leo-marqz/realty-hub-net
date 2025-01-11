
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RealtyHub.Extensions
{
    public static class IdentityErrorExtension
    {
        public static void handleIdentityErrors(this IdentityResult result, ModelStateDictionary modelState)
        {
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError("", error.Description);
                }
            }
        }
    }
}