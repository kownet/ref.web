using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ref.Web.Pages
{
    public class ThankYouModel : PageModel
    {
        public void OnGet()
        {
        }

        public string Message { get; private set; }

        public void OnGetUserCreated()
        {
            Message = "Utworzono konto użytkownika, proszę sprawdzić podany adres email. Dziękujemy!";
        }

        public void OnGetMailSent()
        {
            Message = "Wysłano wiadomość, odpowiemy tak szybko jak to możliwe. Dziękujemy!";
        }
    }
}