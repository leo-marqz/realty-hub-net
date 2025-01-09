using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealtyHub.Extensions;
using RealtyHub.Models;
using RealtyHub.Models.Forms.AuthForms;

namespace RealtyHub.Controllers
{
    [Route("[controller]")] 
    public class AuthController : Controller
    {
        // This is a logger that will be used to log information
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger; 
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
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
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

            response.handleIdentityErrors();

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

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _signInManager.PasswordSignInAsync(
                    userName: model.Email, 
                    password: model.Password, 
                    isPersistent: model.RememberMe, 
                    lockoutOnFailure: false
                );

            if(response.Succeeded)
            {
                if(returnUrl.IsNullOrEmpty()){
                    // Redirect to the properties page
                    return RedirectToAction("Index", "Property");
                }
                // Protect from open redirect attacks
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");

            return View(model);
        }

        #endregion

        #region SignOut

        [HttpPost("signout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Auth");
        }

        #endregion

        #region ForgotPassword

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("forgot-password")]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(string model)
        {
            return Ok();
        }

        #endregion

        #region ResetPassword

        [HttpGet("reset-password")]
        public IActionResult ResetPassword([FromQuery] string code)
        {
            if (code == null)
            {
                return BadRequest();
            }

            var model = new ResetPasswordForm
            {
                Code = code
            };
            return View(model);
        }

        [HttpPost("reset-password")]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword([FromForm] ResetPasswordForm model)
        {
            return Ok();
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}