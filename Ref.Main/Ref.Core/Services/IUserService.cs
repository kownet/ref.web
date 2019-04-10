using Ref.Core.Models;
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
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                    return UserRegisterResponse.Success();
                }
                else
                    return UserRegisterResponse.Error("Błąd przy tworzeniu użytkownika.");
            }
            catch (Exception ex)
            {
                return UserRegisterResponse.Error(ex.Message);
            }
        }
    }
}