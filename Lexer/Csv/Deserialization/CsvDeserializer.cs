using System.Reflection;
using System.Runtime.Serialization;
using Lexer.Csv.Attributes;

namespace Lexer.Csv.Deserialization;

internal static class CsvDeserializer
{
    private const BindingFlags Binds = BindingFlags.Instance | BindingFlags.Public;

    internal static T[] Deserialize<T>(string[] headers, string[][] csvValues) where T : new()
    {
        T[] ts = new T[csvValues.Length];
        int l = ts.Length;
        for (int i = 0; i < l; i++)
        {
            ts[i] = DeserializeLine<T>(headers, csvValues[i]);
        }

        return ts;
    }

    private static T DeserializeLine<T>(string[] headers, string[] lineValues) where T : new()
    {
        PropertyInfo[] csvProps = GetCsvProps<T>();
        if (csvProps.Length is 0)
            throw new SerializationException(
                $"No properties with {nameof(CsvPropertyNameAttribute)} found on target type `{typeof(T).FullName}`");

        object inst = new T();
        Type instT = inst.GetType();
        Dictionary<string, Type> types = GetCorTypes(csvProps, headers);

        if (types.Count != csvProps.Length)
        {
            string[] missing = GetMissingMembers(headers, csvProps);
            string missingJ = string.Join(", ", missing);
            throw new MissingMemberException(
                $"Target type does not include property for every header value.\nMissing properties for header-values: `{missingJ}`");
        }

        int idx = 0;
        foreach (var type in types)
        {
            object converted = Converter.ConvertToType(type.Value, lineValues[idx]);
            instT.InvokeMember(type.Key, Binds | BindingFlags.SetProperty, Type.DefaultBinder, inst,
                new[] { converted });

            idx++;
        }

        return (T)inst;
    }

    private static string[] GetMissingMembers(string[] headers, PropertyInfo[] props)
    {
        List<string> missing = new();
        Span<string> keyList = GetHeaderKeys(props);
        int l = headers.Length;
        for (int i = 0; i < l; i++)
        {
            if (!keyList.Contains(headers[i]))
                missing.Add(headers[i]);
        }

        return missing.ToArray();
    }

    private static string[] GetHeaderKeys(PropertyInfo[] props)
    {
        List<string> keys = new();
        int l = props.Length;
        for (int i = 0; i < l; i++)
        {
            var cur = props[i];
            var curAttr = cur.GetCustomAttribute<CsvPropertyNameAttribute>()!;
            if (curAttr.Name is not null)
            {
                keys.Add(curAttr.Name);
                continue;
            }

            keys.Add(cur.Name);
        }

        return keys.ToArray();
    }

    private static Dictionary<string, Type> GetCorTypes(PropertyInfo[] props, string[] headers)
    {
        Dictionary<string, Type> dict = new();
        Span<string> headersL = headers;
        int l = props.Length;

        for (int i = 0; i < l; i++)
        {
            var cur = props[i];
            var curAttr = cur.GetCustomAttribute<CsvPropertyNameAttribute>()!;

            int propIdx = headersL.IndexOf(cur.Name);
            if (curAttr.Name is not null)
                propIdx = propIdx is -1 ? headersL.IndexOf(curAttr.Name) : propIdx;


            if (propIdx != -1 && propIdx < props.Length)
                dict.Add(props[propIdx].Name, props[propIdx].PropertyType);
        }

        return dict;
    }

    private static PropertyInfo[] GetCsvProps<T>()
    {
        List<PropertyInfo> props = new();
        Span<PropertyInfo> all = typeof(T).GetProperties(Binds);

        int l = all.Length;

        for (int i = 0; i < l; i++)
        {
            var cur = all[i];
            if (cur.GetCustomAttribute<CsvPropertyNameAttribute>() is not null)
                props.Add(cur);
        }

        return props.ToArray();
    }
}