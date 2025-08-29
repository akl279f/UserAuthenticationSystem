using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserAuthenticationSystem.Views.Account
{
    public class DashboardModel : PageModel
    {
        public string UserEmail { get; set; }

        public IActionResult OnGet()
        {
            UserEmail = HttpContext.Session.GetString("User");
            if (UserEmail == null)
                return RedirectToPage("Login");

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("Login");
        }
    }
}