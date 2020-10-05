using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class LinqExtensions
    {

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query, int PageNumber, int PageSize)
        {

            return await query.Skip((PageNumber - 1) * PageSize)
                        .Take(PageSize).ToListAsync();

        }

        public static List<T> ToList<T>(this IQueryable<T> query, int PageNumber, int PageSize)
        {

            return query.Skip((PageNumber - 1) * PageSize)
                        .Take(PageSize).ToList();

        }

    }
}
