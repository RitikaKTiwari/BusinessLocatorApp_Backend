using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Services
{
    public class ServiceServices : IServiceServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ServiceServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Get all services
        public async Task<List<ListServiceDto>> GetServices()
        {
            return await _context.Services
            .Include(service => service.subCategory)
            .Where(service => service.IsActive == true)
            .Select(service => new ListServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                SubCategoryName = service.subCategory.Name,
                isActive = service.IsActive
            })
            .ToListAsync();
        }

        public async Task<List<ListServiceDto>> GetAllServices()
        {
            return await _context.Services
            .Include(service => service.subCategory)
            .Select(service => new ListServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                SubCategoryName = service.subCategory.Name,
                isActive = service.IsActive
            })
            .ToListAsync();
        }

        // Get a service by ID
        public async Task<ListServiceDto> GetServiceById(int serviceId)
        {
            var service = await _context.Services
                 .Include(service => service.subCategory)
                 .Where(service => service.Id == serviceId && service.IsActive == true)
                 .Select(service => new ListServiceDto
                 {
                     Id = service.Id,
                     Name = service.Name,
                     Description = service.Description,
                     SubCategoryName = service.subCategory.Name,
                     isActive = service.IsActive
                 })
                .SingleOrDefaultAsync();

            if (service == null)
            {
                throw new Exception("Service not found.");
            }
            return service;
        }

        public async Task<List<Service>> GetServiceBySubCategoryId(int subcategoryid)
        {
            var services = await _context.Services.Where(service => service.SubCategoryId == subcategoryid && service.IsActive == true).ToListAsync();
            return services;
        }

        public async Task<Service> PostService(ServiceDto request)
        {
            var subCategoryExists = await _context.SubCategories.SingleOrDefaultAsync(subcategory => subcategory.Id == request.SubCategoryId && subcategory.IsActive == true);
            if (subCategoryExists == null)
            {
                throw new Exception("Subcategory not found.");
            }

            var existingService = await _context.Services.SingleOrDefaultAsync(service => service.Name == request.Name && service.SubCategoryId == request.SubCategoryId && service.IsActive == true);
            if (existingService != null)
            {
                throw new Exception("Service with the same name already exists in this subcategory.");
            }

            var newService = new Service
            {
                Name = request.Name,
                Description = request.Description,
                SubCategoryId = request.SubCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            return newService;
        }

        public async Task<Service> PutService(int serviceId, ServiceDto request)
        {
            var subCategoryExists = await _context.SubCategories.SingleOrDefaultAsync(subcategory => subcategory.Id == request.SubCategoryId && subcategory.IsActive == true);
            if (subCategoryExists == null)
            {
                throw new Exception("Subcategory not found.");
            }

            var service = await _context.Services.SingleOrDefaultAsync(service => service.Id == serviceId);
            if (service == null)
            {
                throw new Exception("Service not found.");
            }

            var existingService = await _context.Services.SingleOrDefaultAsync(service => service.Name == request.Name && service.SubCategoryId == request.SubCategoryId && service.Id != serviceId);
            if (existingService != null)
            {
                throw new Exception("Service with the same name already exists in this subcategory.");
            }

            service.Name = request.Name;
            service.SubCategoryId = request.SubCategoryId;
            service.Description = request.Description;
            service.IsActive = request.IsActive;
            service.UpdatedAt = DateTime.UtcNow;

            _context.Services.Update(service);
            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<Service> DeleteService(int serviceId)
        {
            var service = await _context.Services.SingleOrDefaultAsync(service => service.Id == serviceId && service.IsActive == true);
            if (service == null)
            {
                throw new Exception("Service not found.");
            }

            service.IsActive = false;
            service.UpdatedAt = DateTime.UtcNow;

            _context.Services.Update(service);
            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<int> GetTotalActiveServicesAsync()
        {
            return await _context.Services.CountAsync(service => service.IsActive);
        }

        public async Task<List<Service>> DeleteServicesBySubCategory(int subcategoryid)
        {
            var services = await _context.Services.Where(service1 => service1.SubCategoryId == subcategoryid && service1.IsActive == true).ToListAsync();

            if (services == null || !services.Any())
            {
                throw new Exception("No active services found for this subcategory.");
            }

            foreach (var service in services)
            {
                service.IsActive = false;
                service.UpdatedAt = DateTime.UtcNow;

                _context.Services.Update(service);
            }

            await _context.SaveChangesAsync();
            return services;
        }

        public async Task<List<ListServiceDto>> GetTopServices(int top = 5)
        {
            var topServices = await _context.BusinessServices
                .Where(businessService => businessService.IsActive)
                .GroupBy(businessService => businessService.ServiceId)
                .Select(group => new
                {
                    ServiceId = group.Key,
                    UsageCount = group.Count()
                })
                .OrderByDescending(service => service.UsageCount)
                .Take(top)
                .Join(_context.Services,
                    topService => topService.ServiceId,
                    service => service.Id,
                    (topService, service) => new ListServiceDto
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Description = service.Description,
                        SubCategoryName = service.subCategory.Name,
                        isActive = service.IsActive
                    })
                .ToListAsync();

            return topServices;
        }
    }
}