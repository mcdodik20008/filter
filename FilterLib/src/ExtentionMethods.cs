using System.Reflection;
using System.Reflection.Emit;

namespace FilterLib;

public static class ExtentionMethods
{
    public static PropertyInfo? GetPropertyByName(this PropertyInfo[] propertyInfos, string fieldName)
    {
        if (!propertyInfos.Any())
            return null;
        return propertyInfos.First(x => x.Name.Equals(fieldName));
    }

    public static string GetFirstNameInPath(this string fieldPath)
    {
        return fieldPath.Split(".").First() ?? "";
    }
}