using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using LinqKit;

namespace FilterLib;

internal static class ExtentionMethods
{
    internal static PropertyInfo? GetPropertyByName(this PropertyInfo[] propertyInfos, string fieldName)
    {
        if (!propertyInfos.Any())
            throw new Exception("Объект не содержит публичных полей для фильтрации");
        return propertyInfos.First(x => x.Name.Equals(fieldName));
    }

    internal static string GetFirstNameInPath(this string fieldPath)
    {
        return fieldPath.Split(".").First() ?? "";
    }
    
    internal static string RemoveFirstObjectInPath(this string fieldPath)
    {
        return fieldPath.Split(".").Skip(1).JoinString('.');
    }

    internal static string JoinString(this IEnumerable<string> source, char separator)
    {
        var builder = new StringBuilder();
        
        foreach (var str in source)
        {
            builder.Append(str);
            builder.Append(separator);
        }

        builder.Remove(builder.Length - 1, 1);
        return builder.ToString();
    }

    internal static T ToConsole<T>(this T starter)
    {
        Console.WriteLine(starter.ToString());
        return starter;
    }
}