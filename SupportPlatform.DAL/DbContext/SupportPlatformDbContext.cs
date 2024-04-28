using SupportPlatform.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using SupportPlatform.API.DAL.Models;

namespace SupportPlatform.DAL.DbContext
{
    public class SupportPlatformDbContext : IdentityDbContext<ApplicationUser, Role, string,
                                                              IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
                                                              IdentityRoleClaim<string>, IdentityUserToken<string>>//IdentityDbContext<ApplicationUser, Role, string>
    {
        public SupportPlatformDbContext(DbContextOptions<SupportPlatformDbContext> options)
            : base(options)
        { }
        public SupportPlatformDbContext()
        { }

        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Request>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }
    }
}
