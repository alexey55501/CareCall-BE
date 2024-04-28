using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace SupportPlatform.API.Configuration
{
    public static class AuthenticationConfiguration
    {
        public static void Configure(
          IConfiguration Configuration,
          IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])),
                        ValidateIssuerSigningKey = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // если запрос направлен хабу
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hub")))
                            {
                                // получаем токен из строки запроса
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            //.AddGoogle(options =>
            //{
            //    options.ClientId = Configuration["OAuth:Google:ClientId"];
            //    options.ClientSecret = Configuration["OAuth:Google:ClientSecret"];
            //})
            //.AddFacebook(options =>
            //{
            //    options.AppId = Configuration["OAuth:Facebook:AppId"];
            //    options.AppSecret = Configuration["OAuth:Facebook:AppSecret"];
            //});
        }
    }
}
