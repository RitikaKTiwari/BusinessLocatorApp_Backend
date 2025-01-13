using BusinessLocatorApp.Data;
using BusinessLocatorApp.Dto;
using BusinessLocatorApp.Models;
using BusinessLocatorApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusinessLocatorApp.Services
{
    public class TechnicianService : ITechnicianService
    {
        private readonly ApplicationDbContext _context;

        public TechnicianService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Technician> RegisterTechnicianAsync(TechnicianDto request)
        {
            var technician = new Technician
            {
                FullName = request.FullName,
                Experience = request.Experience,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                BusinessServiceId = request.BusinessServiceId
            };

            _context.Technicians.Add(technician);
            await _context.SaveChangesAsync();

            return technician;
        }

        public async Task<Technician> GetTechnicianByIdAsync(int id)
        {
            return await _context.Technicians
                .Include(t => t.businessService)
                .Where(t => t.IsActive == true)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Technician>> GetTechniciansByBusinessServiceId(int businessServiceId)
        {
            // Fetch all active technicians for the given BusinessServiceId
            return await _context.Technicians
                .Where(t => t.BusinessServiceId == businessServiceId && t.IsActive == true)
                .ToListAsync();
        }

        public async Task<List<ListTechnicianDto>> GetTechniciansByBusinessId(int businessId)
        {
            // Fetch all active technicians for the given BusinessServiceId
            return await _context.Technicians
                .Where(t => t.businessService.BusinessId == businessId && t.IsActive == true)
                .Select(t => new ListTechnicianDto
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Experience = t.Experience,
                    BusinessServiceId = t.BusinessServiceId,
                    servicename = t.businessService.service.Name
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<Technician>> GetAllTechniciansAsync()
        {
            return await _context.Technicians
                 .Include(t => t.businessService)
                 .Where(t => t.IsActive == true)
                 .ToListAsync();
        }

        public async Task<Technician> UpdateTechnicianAsync(int id, TechnicianDto request)
        {
            var technician = await _context.Technicians.FindAsync(id);
            if (technician == null)
            {
                return null;
            }

            technician.FullName = request.FullName;
            technician.Experience = request.Experience;
            technician.IsActive = true;
            technician.UpdatedAt = DateTime.UtcNow;
            technician.BusinessServiceId = request.BusinessServiceId;

            _context.Technicians.Update(technician);
            await _context.SaveChangesAsync();

            return technician;
        }

        public async Task<bool> DeleteTechnicianAsync(int id)
        {
            var technician = await _context.Technicians.FindAsync(id);
            if (technician == null)
            {
                return false;
            }

            technician.IsActive = false;
            technician.UpdatedAt = DateTime.UtcNow;

            _context.Technicians.Update(technician);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<int> GetTotalActiveTechniciansAsync()
        {
            return await _context.Technicians.CountAsync(technician => technician.IsActive);
        }

        public async Task<int> GetTotalActiveTechniciansOfaParticularBusiness(int businessId)
        {
            return await _context.Technicians.CountAsync(technician => technician.businessService.BusinessId == businessId && technician.IsActive == true);
        }
    }
}