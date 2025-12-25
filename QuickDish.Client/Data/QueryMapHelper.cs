using System.Reflection;

namespace QuickDish.Client.Data;

public static class QueryMapHelper
{
    /// <summary>
    /// Convert any object public properties to IDictionary<string, object?> for [QueryMap]
    /// </summary>
    public static IDictionary<string, object?> ToQueryMap(this object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var dict = new Dictionary<string, object?>();

        foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = prop.GetValue(obj, null);
            if (value != null)
            {
                dict[prop.Name] = value;
            }
        }

        return dict;
    }
}