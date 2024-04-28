using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportPlatform.API.DAL.Models;
using SupportPlatform.BLL.Helpers;
using SupportPlatform.BLL.Services;
using SupportPlatform.DAL.DbContext;
using SupportPlatform.DAL.Models;
using SupportPlatform.SharedModels.Base;
using SupportPlatform.SharedModels.DTO.Filters.Base;
using SupportPlatform.SharedModels.DTO.Filters.Recruitment;
using SupportPlatform.SharedModels.DTO.Filters.Users;
using SupportPlatform.SharedModels.DTO.Pagination;
using SupportPlatform.SharedModels.DTO.Recruitment;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SupportPlatform.API.BLL.Services.Recruitment
{
    public class RequestsService : BaseService
    {
        private readonly IConfiguration _configuration;
        private readonly SupportPlatformDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestsService(
            IConfiguration configuration,
            IServiceScopeFactory factory,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _db = factory.CreateScope().ServiceProvider
                .GetRequiredService<SupportPlatformDbContext>();
            _userManager = userManager;
        }

        public bool CreateOrUpdateRequest(RequestDTO payload)
        {
            var vac = _db.Requests.Where(t => t.Id == payload.Id).FirstOrDefault();
            if (vac == null)
            {
                var v = Mapper.Map<Request>(payload);
                v.CreationDate = DateTime.Now;
                _db.Requests.Add(v);
                _db.SaveChanges();
            }
            else
            {
                Mapper.Map(payload, vac);
                _db.SaveChanges();
            }

            return true;
        }

        public PaginatedResponse<RequestDTO> GetRequests(
            RequestsFilterModel filter, string userEmail)
        {
            var user = _db.Users.Where(t => t.Email == userEmail).FirstOrDefault();
            if (user == null) return null;

            var query = _db.Requests.OrderByDescending(t => t.CreationDate).AsQueryable();

            filter.SearchQuery = filter.SearchQuery.ToLower().Trim();
            query =
                query.Where(t =>
                    (t.Title.ToLower().Contains(filter.SearchQuery) ||
                     t.Description.ToLower().Contains(filter.SearchQuery))
                ).AsQueryable();

            if (filter.Id > 0)
                query = query.Where(t => t.Id == filter.Id).AsQueryable();

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(t => t.CreatorId == filter.UserId).AsQueryable();

            if (filter.Category != null)
                query = query.Where(t => t.Category == filter.Category).AsQueryable();

            query = query.OrderByDescending(t => t.CreationDate).AsQueryable();

            if(!string.IsNullOrEmpty(filter.OrderField))
            {
                //switch(filter.OrderField)
                //{
                //    case "creationDate"
                //}
                //if (filter.OrderBy == SortOrder.DESC)
                //    query = query.OrderByDescending();
            }

            int elementsCount = query.Count();

            if (filter.IsPaginationEnabled)
                query = query.Skip(filter.Skip).Take(filter.Take);

            var result = Mapper.Map<List<RequestDTO>>(query.ToList());

            return
                new PaginatedResponse<RequestDTO>()
                {
                    Items = result,
                    CurrentPage = filter.Page ?? 0,
                    PagesCount = PaginationHelper.PagesCount(filter?.AmountOnPage ?? 1, elementsCount),
                    ItemsCount = elementsCount,
                };
        }
    }
}
