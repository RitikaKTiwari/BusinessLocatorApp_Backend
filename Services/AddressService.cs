using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Address> AddAddressForUser(int userId, AddressDto request)
        {
            var address = new Address
            {
                Street = request.Street,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsPrimary = request.IsPrimary,
                UserId = userId,
                BusinessId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return address;
        }

        public async Task<Address> GetAddressByID(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<Address> GetAddressForUser(int userId)
        {
            return await _context.Addresses.Where(address => address.UserId == userId && address.IsPrimary==true).FirstOrDefaultAsync();
        }

        public async Task<Address> GetAddressForBusiness(int businessId)
        {
            var address1=await _context.Businesses.Where(business=>business.Id==businessId).FirstOrDefaultAsync();
            return await _context.Addresses.Where(address => address.Id == address1.AddressId && address.IsPrimary == true).FirstOrDefaultAsync();
        }

        public async Task<Address> UpdateAddress(int id, AddressDto request)
        {
            var existingAddress = await _context.Addresses.Where(a => a.Id == id && a.IsPrimary==true).FirstOrDefaultAsync();

            if (existingAddress == null)
            {
                throw new KeyNotFoundException($"Primary address for user with ID {id} not found.");
            }

            existingAddress.Street = request.Street;
            existingAddress.Location = request.Location;
            existingAddress.Latitude = request.Latitude;
            existingAddress.Longitude = request.Longitude;
            existingAddress.IsPrimary = request.IsPrimary;
            existingAddress.UpdatedAt = DateTime.UtcNow;

            _context.Addresses.Update(existingAddress);
            await _context.SaveChangesAsync();

            return existingAddress;
        }
    }
}
