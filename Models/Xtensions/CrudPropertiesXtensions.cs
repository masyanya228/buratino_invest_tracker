using Buratino.Models.Attributes;

using LiteDB;

using System.Reflection;

namespace Buratino.Models.Xtensions
{
    public static class CrudPropertiesXtensions
    {
        public static IEnumerable<PropertyInfo> GetDefaultList(this Type entityType)
        {
            return entityType.GetProperties()
                .Where(x => x.CanWrite)
                .Where(x => x.Name.ToLower() != "id")
                .Where(x => x.GetAttribute<HidenPropertyAttribute>() == null)
                .ToArray();
        }

        public static IEnumerable<PropertyInfo> GetPropertyList(this Type entityType, params string[] propertyNames)
        {
            propertyNames = propertyNames.Select(x => x.ToLower().Trim()).ToArray();
            var props = entityType.GetProperties();
            return propertyNames.Select(x => props.FirstOrDefault(y => y.Name.ToLower() == x) ?? throw new Exception($"Свойство {x} не найдено"));
        }
    }
}