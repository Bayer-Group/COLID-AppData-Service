using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Common.Extensions
{
    public static class EntityExtensions
    {
        public static bool IsEmpty<T>(this IEntity<T> entity)
        {
            return entity == null;
        }

        public static bool IsNotEmpty<T>(this IEntity<T> entity)
        {
            return entity != null;
        }

        public static bool IsEmpty(this IEntity entity)
        {
            return entity == null;
        }

        public static bool IsNotEmpty(this IEntity entity)
        {
            return entity != null;
        }
    }
}
