using SupportPlatform.BLL.Services;
using SupportPlatform.SharedModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportPlatform.API.BLL.Services.Recruitment;

namespace SupportPlatform.API.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static void Configure(
            IConfiguration configuration,
            IServiceCollection services)
        {
            // configure DI for application services

            // BLL Services
            services.AddScoped<UserService>();
            services.AddScoped<BlobService>();
            services.AddScoped<RequestsService>();
        }
    }
}
