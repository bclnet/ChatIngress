using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace System
{
    public static class EnumHelper<T> where T : struct, Enum
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();
            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            return enumValues;
        }

        public static T Parse(string value) => (T)Enum.Parse(typeof(T), value, true);

        public static IList<string> GetNames(Enum value) => value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();

        public static IList<string> GetDisplayValues(Enum value) => GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();

        static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                if (staticProperty.PropertyType == typeof(ResourceManager))
                    return ((ResourceManager)staticProperty.GetValue(null, null)).GetString(resourceKey);
            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            if (descriptionAttributes[0].ResourceType != null) return LookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);
            if (descriptionAttributes == null) return string.Empty;
            return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}