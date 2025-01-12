using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealtyHub.Extensions;
using RealtyHub.Models;
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
                if(returnUrl.IsNullOrEmpty()){
                    return LocalRedirect(returnUrl);
                }

                await _signInManager.SignInAsync(usr, isPersistent: false);

                return RedirectToAction("Index", "Property");
            }

            response.handleIdentityErrors(ModelState);

            return View(model);
        }

        #endregion

        #region ConfirmAccount

        [HttpGet("confirm-acount")]
        public IActionResult ConfirmAccount()
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
        public IActionResult ResetPassword([FromForm] ResetPasswordForm model)
        {
            return Ok();
        }

        #endregion

        #region AccessDenied

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}