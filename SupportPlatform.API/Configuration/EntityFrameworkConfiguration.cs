using SupportPlatform.DAL.DbContext;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupportPlatform.API.Configuration
{
    public static class EntityFrameworkConfiguration
    {
        public static void Configure(
           IConfiguration Configuration,
           IServiceCollection services)
        {
            services.AddDbContext<SupportPlatformDbContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure();
                    });

            });
            services.AddIdentity<ApplicationUser, Role>(options =>
            {
                //options.User.AllowedUserNameCharacters = string.Empty;
            })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<SupportPlatformDbContext>()
                .AddDefaultTokenProviders();
        }

        public static async Task InitializeDB(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IApplicationBuilder app)
        {
            UpdateDatabase(app);

            await CreateRoles(serviceProvider);
            await CreateSpecialUsers(serviceProvider, configuration);
        }

        #region DB Initializers
        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

                IdentityResult roleResult;

                var roles = new List<string>() {
                    GlobalConstants.Roles.USER,
                    GlobalConstants.Roles.ADMIN,
                    GlobalConstants.Roles.BANNED_USER
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        roleResult = await roleManager.CreateAsync(new Role(role));
                }
            }
            catch (Exception ex)
            {
                //_logger.LogInformation(ex.Message);
            }
        }
        private static async Task CreateSpecialUsers(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            try
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                #region Admin User

                var adminEmail = configuration["Admin:Login"];
                var user = await userManager.FindByEmailAsync(adminEmail);

                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        Name = "Admin",
                    };
                    var pwd = configuration["Admin:DefaultPassword"];

                    var identityResult = await userManager.CreateAsync(user, pwd);
                    if (!identityResult.Succeeded)
                        throw new Exception("Admin User creation finished with error.\n" +
                            identityResult.Errors.ToList().Select(t => t.Description));

                    user = await userManager.FindByEmailAsync(adminEmail);
                }

                await userManager.AddToRoleAsync(user, GlobalConstants.Roles.ADMIN);

                #endregion
            }
            catch (Exception ex)
            {
                //_logger.LogInformation(ex.Message);
            }
        }
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            try
            {
                using (var serviceScope = app.ApplicationServices
                    .GetRequiredService<IServiceScopeFactory>()
                    .CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<SupportPlatformDbContext>())
                    {
                        context.Database.Migrate();
                    }
                }
            }
            catch { }
        }
        #endregion
    }
}
