using Microsoft.AspNetCore.Mvc;
using RealtyHub.Models.Forms;

namespace RealtyHub.Controllers
{
    [Route("[controller]")] 
    public class AuthController : Controller
    {
        // This is a logger that will be used to log information
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger; 
        }

        #region  SignUp

        [HttpGet("signup")] // This is the route for the SignUp page
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost("signup")]
        [ValidateAntiForgeryToken] // This is a security feature to prevent CSRF attacks
        public IActionResult SignUp([FromForm] SignUpForm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return Ok();
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
            return View();
        }

        [HttpPost("signin")]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(string model)
        {
            return Ok();
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
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost("reset-password")]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string model)
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