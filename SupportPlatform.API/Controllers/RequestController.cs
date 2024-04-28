using AutoMapper;
using SupportPlatform.BLL.Services;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Constants;
using SupportPlatform.SharedModels.DTO.Filters.Base;
using SupportPlatform.SharedModels.DTO.Filters.Users;
using SupportPlatform.SharedModels.DTO.User;
using SupportPlatform.SharedModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using static SupportPlatform.SharedModels.Constants.GlobalConstants;
using SupportPlatform.API.BLL.Services.Recruitment;
using SupportPlatform.SharedModels.DTO.Filters.Recruitment;
using SupportPlatform.SharedModels.DTO.Recruitment;

namespace SupportPlatform.API.Controllers
{
    [ApiController]
    [Authorize(Roles =
            GlobalConstants.Roles.USER + "," +
            GlobalConstants.Roles.ADMIN,
        AuthenticationSchemes = "Bearer")]
    [Route(APIRoutes.V1.Requests.Base)]
    [Produces("application/json")]
    public class RequestController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly RequestsService _requestService;

        public RequestController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            UserService userService,
            RequestsService RequestService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _userService = userService;
            _requestService = RequestService;
        }

        //[HttpPost]
        //[Route(APIRoutes.V1.Requests.GetEntities)]
        //[SwaggerOperation("Get entities")]
        //public async Task<IActionResult> GetEntities(
        //    [FromBody] EntitiesFilterModel filter)
        //{
        //    var users = _userService.GetFilteredUsers(filter);

        //    if (users == null)
        //        return BadRequest();
        //    else
        //        return Ok(users);
        //}

        [HttpPost]
        [Route(APIRoutes.V1.Requests.GetRequests)]
        [SwaggerOperation("Get all Requests")]
        public async Task<IActionResult> GetRequests(
            [FromBody] RequestsFilterModel filter)
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null) 
                return BadRequest("User not found");

            var result = _requestService.GetRequests(filter, user.Email);

            return Ok(result);
        }

        [HttpPut]
        [Route(APIRoutes.V1.Requests.CreateOrUpdateRequest)]
        [SwaggerOperation("Update a Request")]
        public async Task<IActionResult> CreateOrUpdateRequest(
             [FromBody] RequestDTO payload)
        {
            if (_requestService.CreateOrUpdateRequest(payload))
                return Ok();
            else
                return BadRequest();
        }
    }
}
