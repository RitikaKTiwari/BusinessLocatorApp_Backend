using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;

namespace BusinessLocatorApp.Services
{
    public interface IAddressService
    {
        Task<Address> AddAddressForUser(int userId, AddressDto request);
        Task<Address> GetAddressByID(int id);
        Task<Address> UpdateAddress(int id, AddressDto address);
        Task<Address> GetAddressForUser(int userId);
        Task<Address> GetAddressForBusiness(int businessId);
    }
}
