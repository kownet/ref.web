using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ref.Core.Services;
using Ref.Web.ViewModels;
using System.Threading.Tasks;

namespace Ref.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly IUserService _userService;

        public IndexModel(
            ILogger<IndexModel> logger,
            IUserService userService)
        {
            _logger = logger;
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

            //var result = await _userService.Register(UserViewModel.Email);

            //if (result.Succeed)
            //{
            //    _logger.LogInformation($"User: {UserViewModel.Email} created.");

            //    return RedirectToPage("/ThankYou", "UserCreated");
            //}
            //else
            //{
            //    _logger.LogError($"Error: {result.Message} for user: {UserViewModel.Email}.");

            //    return RedirectToPage("/Error", "UserError", new { message = result.Message });
            //}

            return RedirectToPage("/Index");
        }
    }
}