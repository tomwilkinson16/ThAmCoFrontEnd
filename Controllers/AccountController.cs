using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ThAmCoFrontEnd.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl = "/profile")
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        }

        [Authorize]
        public async Task Logout()
        {
            var authenticationProperties = new
                LogoutAuthenticationPropertiesBuilder()
                    .WithRedirectUri(Url.Action("Index", "Home"))
                    .Build();

            await HttpContext.SignOutAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("register-details")]
        [Authorize]
        public IActionResult RegisterDetails()
        {
            return View();
        }

        [HttpPost("register-details")]
        [Authorize]
        public IActionResult RegisterDetails(string name, string address, string phoneNumber, string email)
        {
            // Save the details to the session
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetString("Address", address);
            HttpContext.Session.SetString("PhoneNumber", phoneNumber);
            HttpContext.Session.SetString("Email", email);

            // Redirect to the profile page after registration
            return RedirectToAction("Profile");
        }

        [HttpGet("profile")]
        [Authorize]
        public Task<IActionResult> Profile()
        {
            // Retrieve data from the session
            var name = HttpContext.Session.GetString("Name") ?? "John Doe";
            var address = HttpContext.Session.GetString("Address") ?? "123 Main Street";
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber") ?? "123-456-7890";
            var email = HttpContext.Session.GetString("Email") ?? "user@example.com";

            // Pass data to the view using ViewBag
            ViewBag.Name = name;
            ViewBag.Address = address;
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.Email = email;
            ViewBag.AccountBalance = 150.75; 

            return Task.FromResult<IActionResult>(View());
        }

        [HttpGet("edit-profile")]
        [Authorize]
        public IActionResult EditProfile()
        {
            // Prepopulate the form with hardcoded data (replace with database fetch in a real app)
            ViewBag.Name = "John Doe";
            ViewBag.Address = "123 Main Street";
            ViewBag.PhoneNumber = "012345678901";

            return View();
        }

        [HttpPost("edit-profile")]
        [Authorize]
        public IActionResult EditProfile(string name, string address, string phoneNumber)
        {
            // Save the updated data to the session
            HttpContext.Session.SetString("Name", name);
            HttpContext.Session.SetString("Address", address);
            HttpContext.Session.SetString("PhoneNumber", phoneNumber);

            // Redirect back to the profile page
            return RedirectToAction("Profile");
        }


        public async Task SignUpAsync(string returnUrl = "/register-details")
        {
            var authenticationProperties = new
                LoginAuthenticationPropertiesBuilder()
                .WithParameter("screen_hint", "signup")
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);
        }
    }
}