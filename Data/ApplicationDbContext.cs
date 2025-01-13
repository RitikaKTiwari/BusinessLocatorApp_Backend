using BusinessLocatorApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLocatorApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessNotification> BusinessNotifications { get; set; }
        public DbSet<BusinessRequest> BusinessRequests { get; set; }
        public DbSet<BusinessService> BusinessServices { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryRequest> CategoryRequests { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<SubCategoryRequest> SubCategoryRequests { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BusinessRequest relationships
            modelBuilder.Entity<BusinessRequest>()
                .HasOne(br => br.Role)
                .WithMany()
                .HasForeignKey(br => br.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessRequest>()
                .HasOne(br => br.Address)
                .WithMany()
                .HasForeignKey(br => br.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessNotification>()
            .HasOne(bn => bn.Appointment)
            .WithMany()  // If Appointment has no collection of BusinessNotifications
            .HasForeignKey(bn => bn.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

            // CategoryRequest relationships
            modelBuilder.Entity<CategoryRequest>()
                .HasOne(cr => cr.Business)
                .WithMany()
                .HasForeignKey(cr => cr.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // SubCategoryRequest relationships
            modelBuilder.Entity<SubCategoryRequest>()
                .HasOne(scr => scr.Category)
                .WithMany()
                .HasForeignKey(scr => scr.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubCategoryRequest>()
                .HasOne(scr => scr.Business)
                .WithMany()
                .HasForeignKey(scr => scr.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequest>()
               .HasOne(scr => scr.SubCategory)
               .WithMany()
               .HasForeignKey(scr => scr.SubCategoryId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(scr => scr.Business)
                .WithMany()
                .HasForeignKey(scr => scr.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationships for other models
            modelBuilder.Entity<User>()
                .HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Business>()
                .HasOne(b => b.Address)
                .WithOne(a => a.Business)
                .HasForeignKey<Business>(b => b.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Business>()
                .HasOne(b => b.Role)
                .WithMany()
                .HasForeignKey(b => b.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessService>()
                .HasOne(bs => bs.business)
                .WithMany()
                .HasForeignKey(bs => bs.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessService>()
                .HasOne(bs => bs.service)
                .WithMany()
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.user)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.businessService)
                .WithMany()
                .HasForeignKey(a => a.BusinessServicesID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.user)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favourite>()
                .HasOne(f => f.businessService)
                .WithMany()
                .HasForeignKey(f => f.BusinessServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.user)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.businessService)
                .WithMany()
                .HasForeignKey(r => r.BusinessServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Technician>()
                .HasOne(t => t.businessService)
                .WithMany()
                .HasForeignKey(t => t.BusinessServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.subCategory)
                .WithMany()
                .HasForeignKey(s => s.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.category)
                .WithMany()
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
