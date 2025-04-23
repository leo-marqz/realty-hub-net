using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealtyHub.Extensions;
using RealtyHub.Models;
using RealtyHub.Models.Forms.Application;
using RealtyHub.Models.Forms.AuthForms;
using RealtyHub.Services.Email;

namespace RealtyHub.Controllers
{
    [Route("[controller]")] 
    public class AuthController : Controller
    {
        // This is a logger that will be used to log information
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(
            UserManager<User> userManager, SignInManager<User> signInManager, 
            ILogger<AuthController> logger, IEmailService emailService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; 
            _emailService = emailService;
        }

        #region  SignUp

        [HttpGet("signup")] // This is the route for the SignUp page
        public IActionResult SignUp(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View( new SignUpForm() );
        }

        [HttpPost("signup")]
        [ValidateAntiForgeryToken] // This is a security feature to prevent CSRF attacks
        public async Task<IActionResult> SignUp([FromForm] SignUpForm model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // returnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration attempt.");

                return View(model);
            }

            var usr = new User {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email
            };

            IdentityResult response = await _userManager.CreateAsync(usr, model.Password);

            if(response.Succeeded){

                _logger.LogInformation("User created a new account with password.");

                // code to confirm email
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(usr);

                // url to confirm email
                var callbackUrl = Url.Action(
                    action: "ConfirmAccount", 
                    controller: "Auth",
                    values: new {
                        userId = usr.Id,
                        code = code
                    }, 
                    protocol: HttpContext.Request.Scheme
                );

                _logger.LogInformation("Email confirmation link: {0}", callbackUrl);

                // send email confirmation
                await _emailService.SendEmailConfirmationAsync(usr.Email, callbackUrl);

                // Redirect to the confirmation page
                return RedirectToAction("SignUpConfirmation");

                // if(returnUrl.IsNullOrEmpty()){
                //     return LocalRedirect(returnUrl);
                // }
                // await _signInManager.SignInAsync(usr, isPersistent: false);
                // return RedirectToAction("Index", "Property");
            }

            response.handleIdentityErrors(ModelState);

            return View(model);
        }

        [HttpGet("signup-confirmation")]
        public IActionResult SignUpConfirmation()
        {
            return View();
        }

        #endregion

        #region ConfirmAccount

        [HttpGet("confirm-acount")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmAccount([FromQuery] string userId, [FromQuery] string code)
        {
            if (userId.IsNullOrEmpty() || code.IsNullOrEmpty())
            {
                ModelState.AddModelError(string.Empty, "User ID and Code are required.");
                return View();
            }

            var usr = await _userManager.FindByIdAsync(userId);

            if (usr == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View();
            }

            var response = await _userManager.ConfirmEmailAsync(
                    user: usr, 
                    token: code
                );

            if(response.Succeeded)
            {
                return RedirectToAction("ConfirmAccountConfirmation");
            }

            response.handleIdentityErrors(ModelState);

            return View();
        }

        [HttpGet("confirm-account-confirmation")]
        public IActionResult ConfirmAccountConfirmation()
        {
            return View();
        }

        #endregion

        #region SignIn

        [HttpGet("signin")]
        public IActionResult SignIn(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View( new SignInForm() );
        }

        [HttpPost("signin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([FromForm] SignInForm model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // returnUrl = returnUrl ?? Url.Content("~/");

            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt.");

                return View(model);
            }

            var response = await _signInManager.PasswordSignInAsync(
                    userName: model.Email, 
                    password: model.Password, 
                    isPersistent: model.RememberMe, 
                    lockoutOnFailure: true
                );

            if(response.Succeeded)
            {
                if(returnUrl.IsNullOrEmpty()){
                    // Redirect to the properties page
                    return RedirectToAction("Index", "Property");
                }
                // Protect from open redirect attacks
                return LocalRedirect(returnUrl);

            }else if(response.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada por intentos fallidos.");
            }else{
                _logger.LogWarning("Invalid login attempt.");
                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
            }

            return View(model);
        }

        #endregion

        #region SignOut

        [HttpPost("signout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut([FromForm] object model)
        {
            await _signInManager.SignOutAsync();
            
            _logger.LogInformation("User signed out.");

            return RedirectToAction("SignIn", "Auth");
        }

        #endregion

        #region ForgotPassword

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            _logger.LogInformation("Forgot password page requested.");

            return View( new ForgotPasswordForm() );
        }

        [HttpPost("forgot-password")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword([FromForm] ForgotPasswordForm model)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Email is required to reset password.");

                return View(model);
            }

            var usr = await _userManager.FindByEmailAsync( model.Email );

            if(usr == null)
            {
                _logger.LogWarning("User not found.");

                ModelState.AddModelError(string.Empty, "No se encontró el usuario.");

                return View(model);
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(usr);

            // url to reset password
            var callbackUrl = Url.Action(
                action: "ResetPassword", 
                controller: "Auth",
                values: new {
                    userId = usr.Id,
                    code = code
                }, 
                protocol: HttpContext.Request.Scheme
            );

            await _emailService.SendPasswordResetAsync(usr.Email, callbackUrl);
           
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet("forgot-password-confirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region ResetPassword

        [HttpGet("reset-password")]
        public async Task<ActionResult> ResetPassword([FromQuery] string userId, [FromQuery] string code)
        {
            if (userId.IsNullOrEmpty() || code.IsNullOrEmpty())
            {
                ModelState.AddModelError(string.Empty, "User ID and code are required.");
                return View( new ResetPasswordForm() );
            }

            var usr = await _userManager.FindByIdAsync(userId);

            if (usr == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View( new ResetPasswordForm() );
            }

            var model = new ResetPasswordForm {
                Email = usr.Email,
                Code = code
            };

            return View( model );
        }

        [HttpPost("reset-password")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword([FromForm] ResetPasswordForm model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var usr = await _userManager.FindByEmailAsync(model.Email);

            if(usr == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            var response = await _userManager.ResetPasswordAsync(
                user: usr, 
                token: model.Code, // token for password reset (code)
                newPassword: model.Password
            );

            if(response.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            response.handleIdentityErrors(ModelState);

            return View(model);
        }

        [HttpGet("reset-password-confirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region AccessDenied

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region ExternalAccess

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalAccess(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(
                action: "ExternalAccessCallback", 
                controller: "Auth",
                values: new { ReturnUrl = returnUrl }
            );

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                provider: provider, 
                redirectUrl: redirectUrl
            );

            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalAccessCallback(string returnUrl = null, string error = null){
            
            returnUrl = returnUrl ?? Url.Content("~/");
            
            if(error != null){
                ModelState.AddModelError(string.Empty, error);
                return View(nameof(SignIn), new SignInForm());
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            
            // Check if the external login info is null
            if(info == null){
                return RedirectToAction(nameof(SignIn), new { ReturnUrl = returnUrl });
            }

            // Check if the user already has an account
            var result = await _signInManager.ExternalLoginSignInAsync(
                loginProvider: info.LoginProvider, 
                providerKey: info.ProviderKey, 
                isPersistent: false
            );

            if(result.Succeeded){
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnUrl);
            }else{
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                return View("ConfirmExternalAccess", new ConfirmExternalAccessForm{
                    Email = email,
                    Name = name
                });
            }

        }


        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}