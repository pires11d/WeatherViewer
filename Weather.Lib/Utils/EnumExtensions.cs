using System.ComponentModel;

namespace System
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static string GetDescription<T>(this T value) where T : struct
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                return null;
            }

            var fi = type.GetField(value.ToString());

            if (fi == null)
            {
                return string.Empty;
            }

            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));

            return attribute?.Description ?? value.ToString();
        }
    }
}
