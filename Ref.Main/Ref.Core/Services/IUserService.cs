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
                    var msg = $"Utworzono użytkownika: {email}";

                    _pushOverNotification.Send("PewneMieszkanie.pl", msg);
                    _logger.LogInformation(msg);

                    return UserRegisterResponse.Success();
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