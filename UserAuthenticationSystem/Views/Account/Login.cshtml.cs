using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UAS.Data;
using UAS.Models;

namespace UserAuthenticationSystem.Views.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel LoginViewModel { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == LoginViewModel.Email);
            if (user == null || !VerifyPassword(LoginViewModel.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return Page();
            }

            HttpContext.Session.SetString("User", user.Email);

            return RedirectToPage("Dashboard");
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToBase64String(bytes);
            return hash == storedHash;
        }
    }
}