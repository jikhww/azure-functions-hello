namespace Kingdee.CDP.WebApi.SDK
{
    internal static class TypeExt
    {
        public static bool IsSimpleType(this Type type)
        {
            if (!type.IsValueType)
            {
                return type == typeof(string);
            }

            return true;
        }
    }
}
