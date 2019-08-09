using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Ref.Core.Extensions;
using Ref.Core.Models;
using Ref.Core.Notifications;
using Ref.Core.Services;
using Ref.Web.ViewModels;
using System.Threading.Tasks;

namespace Ref.Web.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ILogger<ContactModel> _logger;
        private readonly IEmailNotification _emailNotification;
        private readonly IUserService _userService;

        public ContactModel(
            ILogger<ContactModel> logger,
            IEmailNotification emailNotification,
            IUserService userService)
        {
            _logger = logger;
            _emailNotification = emailNotification;
            _userService = userService;
        }

        public ContactType ContactType { get; set; }
        public bool IsNotDemoAccess { get; set; }
        public void OnGet(int contacttype)
        {
            ContactType = (ContactType)contacttype;

            IsNotDemoAccess = ContactType != ContactType.Demo;

            ContactViewModel = new ContactViewModel
            {
                Subject = ContactType.GetDescription(),
                ContactType = ContactType,
                Message = string.Empty
            };
        }

        [BindProperty]
        public ContactViewModel ContactViewModel { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ContactViewModel.ContactType == ContactType.Demo)
            {
                var result = await _userService.Register(ContactViewModel.Email);

                if (result.Succeed)
                {
                    _logger.LogInformation($"User: {ContactViewModel.Email} created.");

                    return RedirectToPage("/ThankYou", "UserCreated");
                }
                else
                {
                    _logger.LogError($"Error: {result.Message} for user: {ContactViewModel.Email}.");

                    return RedirectToPage("/Error", "UserError", new { message = result.Message });
                }
            }
            else
            {
                var emailSent = _emailNotification.Send(
                $"[PM] {ContactViewModel.Subject}",
                $"{ContactViewModel.Email} pisze: {ContactViewModel.Message}",
                $"{ContactViewModel.Email} pisze: {ContactViewModel.Message}",
                new string[] { "tkowalczyk.poczta@gmail.com" }
                );

                if (emailSent.IsSuccess)
                {
                    _logger.LogInformation($"{emailSent.Message}");

                    return RedirectToPage("/ThankYou", "MailSent");
                }
                else
                {
                    _logger.LogError($"Error: {emailSent.Message} for user: {ContactViewModel.Email}.");

                    return RedirectToPage("/Error", "UserError", new { message = emailSent.Message });
                }
            }
        }
    }
}