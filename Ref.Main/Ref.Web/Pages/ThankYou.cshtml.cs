using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ref.Web.Pages
{
    public class ThankYouModel : PageModel
    {
        public void OnGet()
        {
        }

        public string UserCreated { get; private set; }

        public void OnGetUserCreated()
        {
            UserCreated = "Utworzono konto użytkownika, proszę sprawdzić podany adres email. Dziękujemy!";
        }
    }
}