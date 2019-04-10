using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ref.Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public void OnGet()
        {
        }

        public string UserError { get; private set; }

        public void OnGetUserError(string message)
        {
            UserError = message;
        }
    }
}