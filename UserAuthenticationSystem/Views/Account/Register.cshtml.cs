using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UAS.Data;
using UAS.Models;
using UAS.Validators;
using FluentValidation.Results;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UserAuthenticationSystem.Views.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var validator = new RegisterValidator();
            ValidationResult result = validator.Validate(RegisterViewModel);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return Page();
            }

            if (await _context.Users.AnyAsync(u => u.Email == RegisterViewModel.Email))
            {
                ModelState.AddModelError(string.Empty, "Email already exists");
                return Page();
            }

            var user = new User
            {
                FullName = RegisterViewModel.FullName,
                Email = RegisterViewModel.Email,
                PasswordHash = HashPassword(RegisterViewModel.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("Login");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}