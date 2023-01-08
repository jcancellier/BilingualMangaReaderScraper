using System.Collections;
using System.Reflection;
using System.Text;

namespace BilingualMangaScanScraper.Utilities
{
    public static class ObjectExtensions
    {
        public static string ToStringEx(this object obj)
        {
            StringBuilder sb = new StringBuilder();

            Type type = obj.GetType();
            sb.AppendLine($"Type: {type.Name}");
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                    property.PropertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                    property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    sb.AppendLine($"{property.Name}:");
                    IEnumerable list = (IEnumerable)property.GetValue(obj);
                    foreach (var item in list)
                    {
                        sb.AppendLine($"  {item}");
                    }
                }
                else
                {
                    sb.AppendLine($"{property.Name}: {property.GetValue(obj)}");
                }
            }

            return sb.ToString();
        }
    }
}