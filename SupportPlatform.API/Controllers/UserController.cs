using AutoMapper;
using SupportPlatform.API.BLL.Helpers;
using SupportPlatform.BLL.Infrastructure.Helpers.Auth;
using SupportPlatform.BLL.Services;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Constants;
using SupportPlatform.SharedModels.DTO.Auth;
using SupportPlatform.SharedModels.DTO.User;
using SupportPlatform.SharedModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SupportPlatform.SharedModels.Base;
using SupportPlatform.DAL.DbContext;
using SupportPlatform.API.DAL.Models;

namespace SupportPlatform.API.Controllers
{
    /// <summary>
    /// Managing users account
    /// Editing basic user stuff
    /// Adding, updating user's data etc
    /// </summary>

    [Authorize(Roles =
            GlobalConstants.Roles.USER + "," +
            GlobalConstants.Roles.ADMIN,
        AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route(APIRoutes.V1.User.Base)]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly SupportPlatformDbContext _db;

        public UserController(
            UserManager<ApplicationUser> userManager,
            UserService userService,
            IServiceScopeFactory factory,
            IConfiguration configuration,
            SupportPlatformDbContext db)
        {
            _userManager = userManager;
            _userService = userService;
            _configuration = configuration;
            _db = db;
        }

        #region User Management

        [HttpGet]
        [Route(APIRoutes.V1.User.GetInfo)]
        [SwaggerOperation("Returns info about current user")]
        public async Task<IActionResult> GetInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return Ok(
                    new
                    {
                        user.Id,
                        user.Email,
                        user.Name,
                        user.Age,
                        user.PlatformRole,
                        user.ImageBase64,
                        user.Description,
                        user.PhoneNumber,
                        user.IsActivated,
                        Roles = await _userManager.GetRolesAsync(user),
                    }
                );
            }
            else
            {
                return BadRequest(new { Message = "User not found." });
            }
        }

        [HttpGet]
        [Route(APIRoutes.V1.User.GetOtherUserInfo)]
        [SwaggerOperation("Returns info about OTHER user")]
        public async Task<IActionResult> GetOtherUserInfo(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                return Ok(
                    new
                    {
                        user.Id,
                        user.Name,
                        user.Age,
                        user.PlatformRole,
                        user.ImageBase64,
                        user.Description,
                        user.PhoneNumber,
                        user.IsActivated,
                        Roles = await _userManager.GetRolesAsync(user),
                    }
                );
            }
            else
            {
                return BadRequest(new { Message = "User not found." });
            }
        }

        [HttpGet]
        [Route(APIRoutes.V1.User.GetId)]
        [SwaggerOperation("Returns current user Id")]
        public async Task<IActionResult> GetId()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                return Ok(user.Id);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route(APIRoutes.V1.User.Update)]
        [SwaggerOperation("Update current user")]
        public async Task<IActionResult> Update(
            [FromBody] UserUpdateDTO payload)
        {
            var userId = _userManager.GetUserId(User);
            var user = _db.Users.Where(t => t.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var regDate = user.RegisterDate;

                Mapper.Map(payload, user);
                //user.Email = user.UserName = payload.Email;
                user.RegisterDate = regDate;

                _db.SaveChanges();

                var responseDTO = await GenerateToken(user);
                return Ok(responseDTO);
            }
            else
            {
                return BadRequest(new { Message = "User not found." });
            }
        }

        private async Task<AuthenticatedUserResponseDTO> GenerateToken(ApplicationUser user)
        {
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

            return new AuthenticatedUserResponseDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Roles = roles.ToList(),
            };
        }

        #endregion
    }
}
