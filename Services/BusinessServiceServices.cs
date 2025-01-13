using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLocatorApp.Services
{
    public class BusinessServiceServices : IBusinessServiceServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BusinessServiceServices(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<Image>> GetImagesBusinessService(int businessServiceId)
        {
           return  await _context.Images
                .Where(img => img.BusinessServiceId == businessServiceId) 
                .ToListAsync(); 
        }

        public async Task<List<ListBusinessServiceDto>> GetBusinessServices()
        {
            var businessServices = await _context.BusinessServices
                .Include(bs => bs.business)
                .Include(bs => bs.service)
                .Where(bs => bs.IsActive == true)
                .ToListAsync();

            var businessServiceDtos = new List<ListBusinessServiceDto>();

            foreach (var businessService in businessServices)
            {
                Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

                if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
                {
                    // Step 1: Deserialize the stringified JSON into the correct JSON format
                    var rawDaysOfWeek = businessService.DaysOfWeek;

                    try
                    {
                        // First, deserialize the stringified JSON into a valid JSON string
                        var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);

                        // Step 2: Deserialize the now valid JSON string into the Dictionary<string, DaySchedule>
                        daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle deserialization errors
                        Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
                    }
                }

                // Format the days
                var daysFormatted = new List<string>();
                foreach (var day in daysOfWeek)
                {
                    if (day.Value.isWorkingDay)
                    {
                        daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
                    }
                    else
                    {
                        daysFormatted.Add($"{day.Key}: Closed");
                    }
                }

                var businessServiceDto = new ListBusinessServiceDto
                {
                    Id = businessService.Id,
                    DaysOfWeekFormatted = string.Join(", ", daysFormatted),
                    Price = businessService.Price,
                    IsActive = businessService.IsActive,
                    CreatedAt = businessService.CreatedAt,
                    UpdatedAt = businessService.UpdatedAt,
                    BusinessId = businessService.BusinessId,
                    Business = businessService.business,
                    ServiceId = businessService.ServiceId,
                    Service = businessService.service,
                };

                businessServiceDtos.Add(businessServiceDto);
            }

            return businessServiceDtos;
        }

        public async Task<List<ListBusinessServiceDto>> GetBusinessServicesForBusiness(int businessId)
        {
            var businessServices = await _context.BusinessServices
                .Include(bs => bs.business)
                .Include(bs => bs.service)
                .Where(bs => bs.IsActive == true)
                .Where(bs => bs.BusinessId == businessId)
                .ToListAsync();

            var businessServiceDtos = new List<ListBusinessServiceDto>();

            foreach (var businessService in businessServices)
            {
                Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

                if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
                {
                    // Deserialize and format DaysOfWeek like in the previous method
                    var rawDaysOfWeek = businessService.DaysOfWeek;
                    try
                    {
                        var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);
                        daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the error
                        Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
                    }
                }

                var daysFormatted = new List<string>();
                foreach (var day in daysOfWeek)
                {
                    if (day.Value.isWorkingDay)
                    {
                        daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
                    }
                    else
                    {
                        daysFormatted.Add($"{day.Key}: Closed");
                    }
                }

                var businessServiceDto = new ListBusinessServiceDto
                {
                    Id = businessService.Id,
                    DaysOfWeekFormatted = string.Join(", ", daysFormatted),
                    Price = businessService.Price,
                    IsActive = businessService.IsActive,
                    CreatedAt = businessService.CreatedAt,
                    UpdatedAt = businessService.UpdatedAt,
                    BusinessId = businessService.BusinessId,
                    Business = businessService.business,
                    ServiceId = businessService.ServiceId,
                    Service = businessService.service,
                };

                businessServiceDtos.Add(businessServiceDto);
            }

            return businessServiceDtos;
        }

        public async Task<ListBusinessServiceDto> GetBusinessServiceById(int id)
        {
            var businessService = await _context.BusinessServices
                .Include(bs => bs.business)
                .Include(bs => bs.service)
                .SingleOrDefaultAsync(bs => bs.Id == id && bs.IsActive == true);

            if (businessService == null)
            {
                return null; // Or handle this case if necessary
            }

            Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

            if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
            {
                var rawDaysOfWeek = businessService.DaysOfWeek;
                try
                {
                    var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);
                    daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
                }
                catch (Exception ex)
                {
                    // Handle or log the error
                    Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
                }
            }

            var daysFormatted = new List<string>();
            foreach (var day in daysOfWeek)
            {
                if (day.Value.isWorkingDay)
                {
                    daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
                }
                else
                {
                    daysFormatted.Add($"{day.Key}: Closed");
                }
            }

            var businessServiceDto = new ListBusinessServiceDto
            {
                Id = businessService.Id,
                DaysOfWeekFormatted = string.Join(", ", daysFormatted),
                Price = businessService.Price,
                IsActive = businessService.IsActive,
                CreatedAt = businessService.CreatedAt,
                UpdatedAt = businessService.UpdatedAt,
                BusinessId = businessService.BusinessId,
                Business = businessService.business,
                ServiceId = businessService.ServiceId,
                Service = businessService.service,
            };

            return businessServiceDto;
        }

        public async Task<List<ListBusinessServiceDto>> GetBusinessServiceByServiceId(int serviceId)
        {
            var businessServices = await _context.BusinessServices
                .Include(bs => bs.business)
                .Include(bs => bs.service)
                .Where(bs => bs.IsActive == true)
                .Where(bs => bs.ServiceId == serviceId)
                .ToListAsync();

            var businessServiceDtos = new List<ListBusinessServiceDto>();

            foreach (var businessService in businessServices)
            {
                Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

                if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
                {
                    // Deserialize and format DaysOfWeek like in the previous method
                    var rawDaysOfWeek = businessService.DaysOfWeek;
                    try
                    {
                        var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);
                        daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
                    }
                    catch (Exception ex)
                    {
                        // Handle or log the error
                        Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
                    }
                }

                var daysFormatted = new List<string>();
                foreach (var day in daysOfWeek)
                {
                    if (day.Value.isWorkingDay)
                    {
                        daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
                    }
                    else
                    {
                        daysFormatted.Add($"{day.Key}: Closed");
                    }
                }

                var businessServiceDto = new ListBusinessServiceDto
                {
                    Id = businessService.Id,
                    DaysOfWeekFormatted = string.Join(", ", daysFormatted),
                    Price = businessService.Price,
                    IsActive = businessService.IsActive,
                    CreatedAt = businessService.CreatedAt,
                    UpdatedAt = businessService.UpdatedAt,
                    BusinessId = businessService.BusinessId,
                    Business = businessService.business,
                    ServiceId = businessService.ServiceId,
                    Service = businessService.service,
                };

                businessServiceDtos.Add(businessServiceDto);
            }

            return businessServiceDtos;
        }
        public async Task<BusinessService> PostBusinessService(BusinessServiceDto request)
        {
            // Serialize the DaysOfWeek object to JSON
            var daysOfWeekJson = request.DaysOfWeek != null ?
                Newtonsoft.Json.JsonConvert.SerializeObject(request.DaysOfWeek) : null;

            var newBusinessService = new BusinessService
            {
                BusinessId = request.BusinessId,
                ServiceId = request.ServiceId,
                DaysOfWeek = daysOfWeekJson,
                Price = request.Price,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.BusinessServices.Add(newBusinessService);
            await _context.SaveChangesAsync();

            var newimage = new Image
            {
                ImageUrl = request.ImageUrl,
                BusinessServiceId = newBusinessService.Id
            };

            _context.Images.Add(newimage);
            await _context.SaveChangesAsync();

            return newBusinessService;
        }

        public async Task<BusinessService> PutBusinessService(int businessServiceId, BusinessServiceDto request)
        {
            var businessService = await _context.BusinessServices
                .SingleOrDefaultAsync(bs => bs.Id == businessServiceId && bs.IsActive == true);

            if (businessService == null)
            {
                return null;
            }

            businessService.BusinessId = request.BusinessId;
            businessService.ServiceId = request.ServiceId;
            businessService.DaysOfWeek = request.DaysOfWeek != null ? Newtonsoft.Json.JsonConvert.SerializeObject(request.DaysOfWeek) : businessService.DaysOfWeek;
            businessService.Price = request.Price;
            businessService.IsActive = true;
            businessService.UpdatedAt = DateTime.UtcNow;

            _context.BusinessServices.Update(businessService);
            await _context.SaveChangesAsync();

            return businessService;
        }

        public async Task<BusinessService> DeleteBusinessService(int businessServiceId)
        {
            var businessService = await _context.BusinessServices
                .SingleOrDefaultAsync(bs => bs.Id == businessServiceId && bs.IsActive == true);

            var technicians = await _context.Technicians.Where(technician => technician.BusinessServiceId == businessService.Id).ToListAsync();
            if (businessService == null)
            {
                return null;
            }

            foreach (var technician in technicians)
            {
                technician.IsActive = false;
                technician.UpdatedAt = DateTime.UtcNow;
            }

            businessService.IsActive = false;
            businessService.UpdatedAt = DateTime.UtcNow;

            _context.BusinessServices.Update(businessService);
            await _context.SaveChangesAsync();

            return businessService;
        }

        public async Task<int> GetTotalActiveServicesOfaParticularBusiness(int businessId)
        {
            return await _context.BusinessServices.CountAsync(business => business.BusinessId == businessId && business.IsActive == true);
        }
    }
}
//using BusinessLocatorApp.Data;
//using BusinessLocatorApp.Dto;
//using BusinessLocatorApp.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace BusinessLocatorApp.Services
//{
//    public class BusinessServiceServices : IBusinessServiceServices
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IConfiguration _configuration;

//        public BusinessServiceServices(ApplicationDbContext context, IConfiguration configuration)
//        {
//            _context = context;
//            _configuration = configuration;
//        }

//        public async Task<List<ListBusinessServiceDto>> GetBusinessServices()
//        {
//            var businessServices = await _context.BusinessServices
//                .Include(bs => bs.business)
//                .Include(bs => bs.service)
//                .Where(bs => bs.IsActive == true)
//                .ToListAsync();

//            var businessServiceDtos = new List<ListBusinessServiceDto>();

//            foreach (var businessService in businessServices)
//            {
//                Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

//                if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
//                {
//                    // Step 1: Deserialize the stringified JSON into the correct JSON format
//                    var rawDaysOfWeek = businessService.DaysOfWeek;

//                    try
//                    {
//                        // First, deserialize the stringified JSON into a valid JSON string
//                        var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);

//                        // Step 2: Deserialize the now valid JSON string into the Dictionary<string, DaySchedule>
//                        daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
//                    }
//                    catch (Exception ex)
//                    {
//                        // Log or handle deserialization errors
//                        Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
//                    }
//                }

//                // Format the days
//                var daysFormatted = new List<string>();
//                foreach (var day in daysOfWeek)
//                {
//                    if (day.Value.isWorkingDay)
//                    {
//                        daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
//                    }
//                    else
//                    {
//                        daysFormatted.Add($"{day.Key}: Closed");
//                    }
//                }

//                var businessServiceDto = new ListBusinessServiceDto
//                {
//                    Id = businessService.Id,
//                    DaysOfWeekFormatted = string.Join(", ", daysFormatted),
//                    IsActive = businessService.IsActive,
//                    CreatedAt = businessService.CreatedAt,
//                    UpdatedAt = businessService.UpdatedAt,
//                    BusinessId = businessService.BusinessId,
//                    Business = businessService.business,
//                    ServiceId = businessService.ServiceId,
//                    Service = businessService.service,
//                };

//                businessServiceDtos.Add(businessServiceDto);
//            }

//            return businessServiceDtos;
//        }

//        //public async Task<List<BusinessServiceDto>> GetBusinessServices()
//        //{
//        //    // Fetch active business services from the database
//        //    var businessServices = await _context.BusinessServices
//        //        .Where(bs => bs.IsActive)
//        //        .Include(bs => bs.business) // Include the related Business
//        //        .Include(bs => bs.service)  // Include the related Service
//        //        .ToListAsync();

//        //    // Transform the data into DTO format
//        //    var businessServiceDtos = businessServices.Select(bs => new BusinessServiceDto
//        //    {
//        //        BusinessId = bs.BusinessId,
//        //        ServiceId = bs.ServiceId,
//        //        DaysOfWeek = bs.DaysOfWeek
//        //    }).ToList();

//        //    return businessServiceDtos;
//        //}

//        public async Task<List<ListBusinessServiceDto>> GetBusinessServicesForBusiness(int businessId)
//        {
//            var businessServices = await _context.BusinessServices
//                .Include(bs => bs.business)
//                .Include(bs => bs.service)
//                .Where(bs => bs.IsActive == true)
//                .Where(bs => bs.BusinessId == businessId)
//                .ToListAsync();

//            var businessServiceDtos = new List<ListBusinessServiceDto>();

//            foreach (var businessService in businessServices)
//            {
//                Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

//                if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
//                {
//                    // Deserialize and format DaysOfWeek like in the previous method
//                    var rawDaysOfWeek = businessService.DaysOfWeek;
//                    try
//                    {
//                        var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);
//                        daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
//                    }
//                    catch (Exception ex)
//                    {
//                        // Handle or log the error
//                        Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
//                    }
//                }

//                var daysFormatted = new List<string>();
//                foreach (var day in daysOfWeek)
//                {
//                    if (day.Value.isWorkingDay)
//                    {
//                        daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
//                    }
//                    else
//                    {
//                        daysFormatted.Add($"{day.Key}: Closed");
//                    }
//                }

//                var businessServiceDto = new ListBusinessServiceDto
//                {
//                    Id = businessService.Id,
//                    DaysOfWeekFormatted = string.Join(", ", daysFormatted),
//                    IsActive = businessService.IsActive,
//                    CreatedAt = businessService.CreatedAt,
//                    UpdatedAt = businessService.UpdatedAt,
//                    BusinessId = businessService.BusinessId,
//                    Business = businessService.business,
//                    ServiceId = businessService.ServiceId,
//                    Service = businessService.service,
//                };

//                businessServiceDtos.Add(businessServiceDto);
//            }

//            return businessServiceDtos;
//        }

//        public async Task<ListBusinessServiceDto> GetBusinessServiceById(int businessServiceId)
//        {
//            var businessService = await _context.BusinessServices
//                .Include(bs => bs.business)
//                .Include(bs => bs.service)
//                .SingleOrDefaultAsync(bs => bs.Id == businessServiceId && bs.IsActive == true);

//            if (businessService == null)
//            {
//                return null; // Or handle this case if necessary
//            }

//            Dictionary<string, DaySchedule> daysOfWeek = new Dictionary<string, DaySchedule>();

//            if (!string.IsNullOrEmpty(businessService.DaysOfWeek))
//            {
//                var rawDaysOfWeek = businessService.DaysOfWeek;
//                try
//                {
//                    var innerJson = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(rawDaysOfWeek);
//                    daysOfWeek = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DaySchedule>>(innerJson);
//                }
//                catch (Exception ex)
//                {
//                    // Handle or log the error
//                    Console.WriteLine("Error deserializing DaysOfWeek: " + ex.Message);
//                }
//            }

//            var daysFormatted = new List<string>();
//            foreach (var day in daysOfWeek)
//            {
//                if (day.Value.isWorkingDay)
//                {
//                    daysFormatted.Add($"{day.Key}: {day.Value.OpenTime} - {day.Value.CloseTime}");
//                }
//                else
//                {
//                    daysFormatted.Add($"{day.Key}: Closed");
//                }
//            }

//            var businessServiceDto = new ListBusinessServiceDto
//            {
//                Id = businessService.Id,
//                DaysOfWeekFormatted = string.Join(", ", daysFormatted),
//                IsActive = businessService.IsActive,
//                CreatedAt = businessService.CreatedAt,
//                UpdatedAt = businessService.UpdatedAt,
//                BusinessId = businessService.BusinessId,
//                Business = businessService.business,
//                ServiceId = businessService.ServiceId,
//                Service = businessService.service,
//            };

//            return businessServiceDto;
//        }

//        public async Task<BusinessService> PostBusinessService(BusinessServiceDto request)
//        {
//            // Serialize the DaysOfWeek object to JSON
//            var daysOfWeekJson = request.DaysOfWeek != null ?
//                Newtonsoft.Json.JsonConvert.SerializeObject(request.DaysOfWeek) : null;

//            var newBusinessService = new BusinessService
//            {
//                BusinessId = request.BusinessId,
//                ServiceId = request.ServiceId,
//                DaysOfWeek = daysOfWeekJson, // Store the JSON string
//                //isHalfDay = request.isHalfDay ?? false,
//                //isCloseDay = request.isCloseDay ?? false,
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow,
//                UpdatedAt = DateTime.UtcNow
//            };

//            _context.BusinessServices.Add(newBusinessService);
//            await _context.SaveChangesAsync();

//            return newBusinessService;
//        }


//        public async Task<BusinessService> PutBusinessService(int businessServiceId, BusinessServiceDto request)
//        {
//            var businessService = await _context.BusinessServices
//                .SingleOrDefaultAsync(bs => bs.Id == businessServiceId && bs.IsActive == true);

//            if (businessService == null)
//            {
//                return null;
//            }

//            businessService.BusinessId = request.BusinessId;
//            businessService.ServiceId = request.ServiceId;
//            businessService.DaysOfWeek = request.DaysOfWeek != null ? Newtonsoft.Json.JsonConvert.SerializeObject(request.DaysOfWeek) : businessService.DaysOfWeek;
//            businessService.IsActive = request.IsActive;
//            businessService.UpdatedAt = DateTime.UtcNow;

//            _context.BusinessServices.Update(businessService);
//            await _context.SaveChangesAsync();

//            return businessService;
//        }

//        public async Task<BusinessService> DeleteBusinessService(int businessServiceId)
//        {
//            var businessService = await _context.BusinessServices
//                .SingleOrDefaultAsync(bs => bs.Id == businessServiceId && bs.IsActive == true);

//            if (businessService == null)
//            {
//                return null;
//            }

//            businessService.IsActive = false;
//            businessService.UpdatedAt = DateTime.UtcNow;

//            _context.BusinessServices.Update(businessService);
//            await _context.SaveChangesAsync();

//            return businessService;
//        }

//        // Backend: ASP.NET Core

//        public async Task<List<ListBusinessServiceDto>> GetNearbyBusinesses(double userLat, double userLng, double rangeInKm = 100.00)
//        {
//            double earthRadius = 6371; // Radius of the Earth in km

//            // Fetch all active businesses and their associated addresses
//            var nearbyBusinesses = await _context.BusinessServices
//                .Include(bs => bs.business)
//                    .ThenInclude(b => b.Address)
//                .Include(bs => bs.service)
//                .Where(bs => bs.IsActive == true &&
//                            bs.business.Address.IsPrimary == true)
//                .ToListAsync();

//            // Filter businesses by Haversine formula in memory
//            var filteredBusinesses = nearbyBusinesses.Where(bs =>
//            {
//                var businessLat = bs.business.Address.Latitude;
//                var businessLng = bs.business.Address.Longitude;

//                // Apply the Haversine formula in memory
//                var distance = earthRadius * Math.Acos(
//                    Math.Cos(userLat * Math.PI / 180) *
//                    Math.Cos(businessLat * Math.PI / 180) *
//                    Math.Cos((businessLng - userLng) * Math.PI / 180) +
//                    Math.Sin(userLat * Math.PI / 180) *
//                    Math.Sin(businessLat * Math.PI / 180)
//                );

//                return distance <= rangeInKm;
//            }).ToList();

//            Console.WriteLine($"Found {filteredBusinesses.Count} nearby businesses.");

//            // Map results to DTO
//            var businessServiceDtos = filteredBusinesses.Select(bs => new ListBusinessServiceDto
//            {
//                Id = bs.Id,
//                BusinessId = bs.BusinessId,
//                Business = bs.business,
//                ServiceId = bs.ServiceId,
//                Service = bs.service,
//                IsActive = bs.IsActive
//            }).ToList();

//            return businessServiceDtos;
//        }
//    }
//}
