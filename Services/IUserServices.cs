using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IUserServices
    {
        Task<AuthResponse> Register(UserRegisterDto request);
        Task<User> UpdateUserAsync(int id, UpdateUserDto user);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<int> GetTotalActiveUsersAsync();
        Task<User> UpdatePasswordAsync(int id, UpdatePasswordRequestDto request);
        Task<bool> UpdateProfilePicAsync(int userId, IFormFile profilePic);
        Task<AuthResponse> ForgotPassword(string email);
        Task<List<TopCustomerDto>> GetTopCustomers();
    }
}