using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult SignUp()
        {
            return View( new SignUpForm() );
        }

        [HttpPost("signup")]
        [ValidateAntiForgeryToken] // This is a security feature to prevent CSRF attacks
        public async Task<IActionResult> SignUp([FromForm] SignUpForm model)
        {
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
        public IActionResult SignIn()
        {
            return View( new SignInForm() );
        }

        [HttpPost("signin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([FromForm] SignInForm model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if(response.Succeeded)
            {
                return RedirectToAction("Index", "Property");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(model);
        }

        #endregion

        #region SignOut

        [HttpGet("signout")]
        public IActionResult SignOut()
        {
            return Ok();
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