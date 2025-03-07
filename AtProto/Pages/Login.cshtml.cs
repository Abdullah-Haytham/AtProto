using FishyFlip;
using FishyFlip.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading;

namespace AtProto.Pages
{
    public class LoginModel(ATProtocol at) : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var (session, error) = await at.AuthenticateWithPasswordResultAsync(Input.Username, Input.Password);
            if(session is null)
            {
                Console.WriteLine("Failed to Authenticate");
                return Page();
            }
            IdentityUser identityUser = new IdentityUser
            {
                UserName = session.Handle.ToString(),
                Email = session.Email,
                EmailConfirmed = true
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, session.Handle.ToString()),
                new Claim(ClaimTypes.Email, session.Email!),
                new Claim("DID", session.Did.ToString()),
                new Claim("AccessToken", session.AccessJwt)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "cookies");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            Console.WriteLine("Authenticated.");
            Console.WriteLine($"Session Did: {session.Did}");
            Console.WriteLine($"Session Email: {session.Email}");
            Console.WriteLine($"Session Handle: {session.Handle}");
            Console.WriteLine($"Session Token: {session.AccessJwt}");
            return Page();
        }
    }

    public class LoginInput
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
