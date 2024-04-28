using SupportPlatform.BLL.Infrastructure.Helpers.Auth;
using SupportPlatform.BLL.Services;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Constants;
using SupportPlatform.SharedModels.DTO.Auth;
using SupportPlatform.SharedModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SunApp.API.Controllers
{
    [ApiController]
    [Route(APIRoutes.V1.Auth.Base)]
    [Produces("application/json")]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthorizationController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route(APIRoutes.V1.Auth.Login)]
        [SwaggerOperation("General login endpoint. Returns roles user assigned to",
                Tags = new[] { "Authorization" })]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                if (user.BanDate != null)
                {
                    return BadRequest(new { Message = user.BanReason });
                }

                var roles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.GivenName, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, string.Join(", ", roles)),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                JwtSecurityToken token = null;

                    token = JWT.GenerateToken(
                        _configuration, authClaims);

                return Ok(new AuthenticatedUserResponseDTO()
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    Roles = roles.ToList(),
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route(APIRoutes.V1.Auth.Register)]
        [SwaggerOperation("User registration",
            Tags = new[] { "Authorization" })]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO payload)
        {
            var userExists = await _userManager.FindByEmailAsync(payload.Email);

            if (userExists != null)
                return BadRequest("User with this email already exists.");

            ApplicationUser user = new()
            {
                Email = payload.Email,
                UserName = payload.Email,
                Name = payload.Name.Trim(),
                RegisterDate = DateTime.Now,
                LastActivityDate = DateTime.Now,
                PhoneNumber = payload.PhoneNumber,
                PlatformRole = payload.PlatformRole,
            };

            // Creating a user
            var result = await _userManager.CreateAsync(user, payload.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            // Assigning to a role
            result = await _userManager.AddToRoleAsync(user, GlobalConstants.Roles.USER);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors.FirstOrDefault().Description);

            return Ok("User created successfully.");
        }

        //[HttpPost]
        //[Route(APIRoutes.V1.Auth.ResetPassword)]
        //[SwaggerOperation("Reset Password to account",
        //    Tags = new[] { "Authorization" })]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDTO payload)
        //{
        //    var user = await _userManager.FindByIdAsync(payload.UserId);
        //    if (user == null)
        //        return BadRequest("");

        //    var result = await _userManager.ResetPasswordAsync(user, payload.Token, payload.Password);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors.FirstOrDefault().Description);
        //    }

        //    return Ok("Password changed");
        //}
    }
}
