using SupportPlatform.SharedModels.DTO.Filters.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SupportPlatform.BLL.Infrastructure.Extensions
{
    /// <summary>
    /// Extensions for filtering get query
    /// </summary>
    public static class FilteringExtension
    {
        /// <summary>
        /// Skip and take logic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<T> SkipAndTake<T>(this IQueryable<T> query, BaseFilterModel filter)
        {
            query = query.Skip(filter.Skip).Take(filter.Take);

            return query;
        }

        public static IQueryable<T> OrderedSkipAndTake<T, TKey>(this IQueryable<T> query,
            Expression<Func<T, TKey>> func, BaseFilterModel filter)
        {
            return query.OrderBy(func).SkipAndTake(filter);
        }

        /// <summary>
        /// Skip and take logic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<T> SkipAndTake<T>(this IEnumerable<T> query, BaseFilterModel filter)
        {
            query = query.Skip(filter.Skip);
            query = query.Take(filter.Take);

            return query;
        }
    }
}
