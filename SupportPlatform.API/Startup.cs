using AutoMapper;
using SupportPlatform.API.Configuration;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.DTO;
using SupportPlatform.SharedModels.DTO.User;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Quartz;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq;

namespace SupportPlatform.API
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }
        public static string _EnvironmentName { get; set; }

        public Startup(
            IWebHostEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appSettings.All.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _EnvironmentName = environment.EnvironmentName;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Startup>>();
            services.AddSingleton(typeof(ILogger), logger);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SupportPlatform.API",
                    Version = "v1",
                    Description = "Supporting platform - API Swagger for Web"
                });
                c.AddSecurityDefinition("jwt", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    Name = "Authorization",
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.EnableAnnotations();

                c.OperationFilter<AddResponseHeadersFilter>(); // [SwaggerResponseHeader]

                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>(); // Adds "(Auth)" to the summary so that you can see which endpoints have Authorization
                                                                              // or use the generic method, e.g. c.OperationFilter<AppendAuthorizeToSummaryOperationFilter<MyCustomAttribute>>();

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddCors(cors => cors.AddPolicy("CorsPolicy", (p) =>
            {
                p.AllowAnyHeader();
                p.AllowAnyOrigin();
                p.AllowAnyMethod();
                p.SetPreflightMaxAge(TimeSpan.FromDays(365 * 10));
                //p.SetIsOriginAllowed((host) => true);
                //p.AllowCredentials();
            }));

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            services.AddMemoryCache();

            EntityFrameworkConfiguration.Configure(Configuration, services);

            DependencyInjectionConfiguration.Configure(Configuration, services);

            AuthenticationConfiguration.Configure(Configuration, services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider services)
        {
            // Apply all unapplied migrations & 
            // create all roles & special users if they don't exist
            EntityFrameworkConfiguration.InitializeDB(services, Configuration, app).Wait();

            Mapper.Initialize(cfg =>
            {
                cfg.ValidateInlineMaps = false;

            });

            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SupportPlatform.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors(x => x
            //  .AllowAnyOrigin()
            //  .AllowAnyMethod()
            //  .AllowAnyHeader());
            //.AllowCredentials()

            app.UseCors("CorsPolicy");

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // SignalR Hubs
                //SignalREndpointsConfiguration.Configure(
                //    endpoints,
                //    Configuration);
            });
        }
    }
}
