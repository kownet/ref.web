using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ref.Core.Services;
using Ref.Web.ViewModels;
using System.Threading.Tasks;

namespace Ref.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserViewModel UserViewModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _userService.Register(UserViewModel.Email);

            if (result.Succeed)
                return RedirectToPage("/Index");
            else
                return RedirectToPage("/Error", "UserError", new { message = result.Message });
        }
    }
}