
using System;
using Microsoft.AspNetCore.Identity;

namespace RealtyHub.Extensions
{
    public static class IdentityErrorExtension
    {
        public static void handleIdentityErrors(this IdentityResult result)
        {
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }
            }
        }
    }
}