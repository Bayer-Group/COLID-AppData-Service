using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;

namespace COLID.AppDataService.Common.Extensions
{
    public static class ICollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool IsNotNullAndEmpty<T>(this ICollection<T> collection)
        {
            return collection != null && collection.Any();
        }
    }
}
