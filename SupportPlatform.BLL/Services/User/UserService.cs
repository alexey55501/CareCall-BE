using AutoMapper;
using SupportPlatform.BLL.Helpers;
using SupportPlatform.DAL.DbContext;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Constants;
using SupportPlatform.SharedModels.DTO.Admin;
using SupportPlatform.SharedModels.DTO.Filters.Base;
using SupportPlatform.SharedModels.DTO.Filters.Users;
using SupportPlatform.SharedModels.DTO.Pagination;
using SupportPlatform.SharedModels.DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportPlatform.API.DAL.Models;
using SupportPlatform.SharedModels.Base;

namespace SupportPlatform.BLL.Services
{
    public class UserService : BaseService
    {
        private readonly IConfiguration _configuration;
        private readonly SupportPlatformDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            IConfiguration configuration,
            IServiceScopeFactory factory,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _db = factory.CreateScope().ServiceProvider
                .GetRequiredService<SupportPlatformDbContext>();
            _userManager = userManager;
        }

        // All users except Parser & Admin Accounts
        //private IQueryable<ApplicationUser> GetManageableUsers()
        //{
        //    var q =  _db.Users
        //               .AsNoTracking()
        //                .Include(x => x.UserRoles)
        //                    .ThenInclude(x => x.Role)
        //               .Where(t =>
        //                   !(
        //                       t.UserRoles.Select(t => t.Role.Name)
        //                           .Contains(GlobalConstants.Roles.ADMIN) ||
        //                       t.UserRoles.Select(t => t.Role.Name)
        //                           .Contains(GlobalConstants.Roles.PARSER)
        //                   ));
        //    var b = _db.Users                      .Include(x => x.UserRoles)
        //                    .ThenInclude(x => x.Role).ToList();
        //    return q;
        //}

        #region Users

        public ApplicationUser GetUser(string id) => _db.Users.Where(t => t.Id == id).FirstOrDefault();
        public ApplicationUser GetUserByEmail(string email) => _db.Users.Where(t => t.Email == email).FirstOrDefault();

        //public PaginatedResponse<UserShortInfoDTO> GetFilteredUsers(
        //    EntitiesFilterModel filter)
        //{
        //    var query = _db.Users
        //        .Include(u => u.UserRoles)
        //        .ThenInclude(ur => ur.Role)
        //        .Where(t =>
        //            (t.Email.ToLower().Contains(filter.SearchQuery) ||
        //             t.Name.ToLower().Contains(filter.SearchQuery))
        //        );

        //    query = query.Where(t => t.UserRoles.Where(r => r.Role.Name == "Admin").Count() == 0);


        //    if (filter.IsActivated != null)
        //        query = query.Where(t => t.IsActivated == filter.IsActivated.Value);

        //    if (filter.IsSupportPlatform != null)
        //        query = query.Where(t => t.IsSupportPlatform == filter.IsSupportPlatform.Value);

        //    if (!string.IsNullOrEmpty(filter.Id))
        //        query = query.Where(t => t.Id == filter.Id);

        //    int elementsCount = query.Count();

        //    if (filter.IsPaginationEnabled)
        //        query = query.Skip(filter.Skip).Take(filter.Take);

        //    var result = Mapper.Map<List<UserShortInfoDTO>>(query.ToList());

        //    result.ForEach(t =>
        //    {
        //        t.Skills = query.Where(q => q.Id == t.Id).FirstOrDefault().Requirements.Select(q =>
        //        {
        //            return new
        //            {
        //                q.Id,
        //                q.SkillId,
        //                q.VacancyId,
        //                q.UserId,
        //                q.Experience,
        //                //Name = q.Skill.Name,
        //            };
        //        }).ToList();
        //    });

        //    return
        //        new PaginatedResponse<UserShortInfoDTO>()
        //        {
        //            Items = result,
        //            CurrentPage = filter.Page ?? 0,
        //            PagesCount = PaginationHelper.PagesCount(filter?.AmountOnPage ?? 1, elementsCount),
        //        };
        //}

        //public UserInfoAdminDTO GetUserInfo(string id)
        //{
        //    var user = _db.Users.Find(id);

        //    _db.Entry(user)
        //        .Collection(u => u.UserRoles)
        //        .Query()
        //        .Include(ur => ur.Role)
        //        .Load();

        //    return Mapper.Map<UserInfoAdminDTO>(user);
        //}

        public async Task UpdateLastActivity(string userId, string ipAddress)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastActivityDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();
            }
        }

        public async Task<IdentityResult> DeleteUser(ApplicationUser user)
        {
            if (user != null)
            {
                user.IsDeleted = true;

                var result = await _userManager.UpdateAsync(user);

                return result;
            }

            return new IdentityResult();
        }

        #endregion

        #region Ban user

        public async Task BanUser(string id, string reason)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            await BanUser(user, reason);
        }

        public async Task BanUser(ApplicationUser user, string reason)
        {
            var roles = await _userManager.GetRolesAsync(user);

            user.BanDate = System.DateTime.UtcNow;
            user.BanReason = reason;
            user.RoleBeforeBan = roles.FirstOrDefault();

            using (var transaction = _db.Database.BeginTransaction())
            {
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                await _userManager.RemoveFromRolesAsync(user, roles);
                //await _userManager.RemoveFromRoleAsync(user, GlobalConstants.Roles.BANNED_USER);
                await _userManager.AddToRoleAsync(user, GlobalConstants.Roles.BANNED_USER);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        public async Task UnbanUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            await UnbanUser(user);
        }

        public async Task UnbanUser(ApplicationUser user)
        {
            var roleBeforeBan = user.RoleBeforeBan;

            user.BanDate = null;
            user.BanReason = null;
            user.RoleBeforeBan = null;

            using (var transaction = _db.Database.BeginTransaction())
            {
                await _userManager.RemoveFromRoleAsync(user, GlobalConstants.Roles.BANNED_USER);
                await _userManager.AddToRoleAsync(user, roleBeforeBan);

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                await transaction.CommitAsync().ConfigureAwait(false);
            }
        }

        #endregion
    }
}