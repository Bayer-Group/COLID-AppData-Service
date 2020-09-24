using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Common.Extensions
{
    public static class EntityExtensions
    {
        public static bool IsEmpty<T>(this Entity<T> entity)
        {
            return entity == null;
        }

        public static bool IsNotEmpty<T>(this Entity<T> entity)
        {
            return entity != null;
        }
    }
}
