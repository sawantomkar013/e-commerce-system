using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;
using System.Runtime.Serialization;

namespace OrderService.Domain.DataAccess;

public class EnumMemberValueConverter<TEnum> : ValueConverter<TEnum, string>
        where TEnum : struct, Enum
{
    public EnumMemberValueConverter()
        : base(
            v => GetEnumMemberValue(v),
            v => GetEnumFromValue(v))
    { }

    private static string GetEnumMemberValue(TEnum value)
    {
        return typeof(TEnum)
            .GetMember(value.ToString())
            .First()
            .GetCustomAttribute<EnumMemberAttribute>()?.Value
            ?? value.ToString();
    }

    private static TEnum GetEnumFromValue(string value)
    {
        foreach (var field in typeof(TEnum).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute))
                is EnumMemberAttribute attr && attr.Value == value)
            {
                return (TEnum)field.GetValue(null)!;
            }
            else if (field.Name == value)
            {
                return (TEnum)field.GetValue(null)!;
            }
        }
        throw new ArgumentException($"Unknown value: {value}");
    }
}