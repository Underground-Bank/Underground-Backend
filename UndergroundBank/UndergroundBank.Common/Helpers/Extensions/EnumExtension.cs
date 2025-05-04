namespace UndergroundBank.Common.Helpers.Extensions;

public static class EnumExtension
{
    public static string ToEnumString<T>(this T enumValue)
        where T : Enum
    {
        return enumValue.ToString();
    }
}

