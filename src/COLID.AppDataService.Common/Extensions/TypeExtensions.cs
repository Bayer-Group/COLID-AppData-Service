using System;
using System.ComponentModel;
using System.Reflection;

namespace COLID.AppDataService.Common.Extensions
{
    public static class TypeExtension
    {
        public static string GetDescription(this Type type)
        {
            var attribute = (DescriptionAttribute)type.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute.Description;
        }
    }
}
