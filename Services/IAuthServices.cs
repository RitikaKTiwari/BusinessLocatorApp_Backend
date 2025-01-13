using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IAuthServices
    {
        Task<AuthResponse> Register(UserRegisterDto request);
        Task<AuthResponse> Login(UserLoginDto request);
        Task<AuthResponse> BusinessLogin(UserLoginDto request);
        Task<AuthResponse> GoogleLogin(string tokenId);

    }
}
