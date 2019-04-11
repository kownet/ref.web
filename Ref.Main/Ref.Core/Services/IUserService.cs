using Microsoft.Extensions.Logging;
using Ref.Core.Extensions;
using Ref.Core.Models;
using Ref.Core.Notifications;
using Ref.Core.Repositories;
using Ref.Core.Responses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ref.Core.Services
{
    public interface IUserService
    {
        Task<UserRegisterResponse> Register(string email);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IUserRepository _userRepository;
        private readonly IPushOverNotification _pushOverNotification;
        private readonly IEmailNotification _emailNotification;

        public UserService(
            ILogger<UserService> logger,
            IUserRepository userRepository,
            IPushOverNotification pushOverNotification,
            IEmailNotification emailNotification)
        {
            _logger = logger;
            _userRepository = userRepository;
            _pushOverNotification = pushOverNotification;
            _emailNotification = emailNotification;
        }

        public async Task<UserRegisterResponse> Register(string email)
        {
            var exist = (await _userRepository.FindByAsync(u => string.Equals(u.Email, email.ToLowerInvariant())))
                .FirstOrDefault();

            if (!(exist is null))
                return UserRegisterResponse.Error("Podany adres email, został już zarejestrowany w systemie");

            try
            {
                var guid = Guid.NewGuid().ToString().ToUpperInvariant();

                var newUserId = await _userRepository.CreateAsync(
                    new User { Email = email, Guid = guid });

                if (newUserId > 0)
                {
                    var emailUI = new EmailUI(guid);

                    var emailToSend = emailUI.Prepare();

                    if(emailUI.CanBeSend)
                    {
                        var emailToUser = _emailNotification.Send(
                            emailToSend.Title,
                            emailToSend.RawMessage,
                            emailToSend.HtmlMessage,
                            new string[] { email });

                        if(emailToUser.IsSuccess)
                        {
                            var msg = $"Utworzono użytkownika: {email} i wysłano wiadomość.";

                            _logger.LogInformation(msg);
                            _pushOverNotification.Send("[PewneMieszkanie.pl] - Sukces", msg);

                            return UserRegisterResponse.Success();
                        }
                        else
                        {
                            _logger.LogInformation(emailToUser.Message);
                            _pushOverNotification.Send("[PewneMieszkanie.pl] - Błąd", emailToUser.Message);

                            return UserRegisterResponse.Error(emailToUser.Message);
                        }
                    }
                    else
                    {
                        return UserRegisterResponse.Error($"Wiadomość do {email} nie może zostać wysłana.");
                    }
                }
                else
                {
                    var msg = $"Błąd przy tworzeniu użytkownika - {email}.";

                    _logger.LogError(msg);

                    return UserRegisterResponse.Error(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.GetFullMessage());

                return UserRegisterResponse.Error(ex.GetFullMessage());
            }
        }
    }
}