using System;

namespace Nmea0183
{
  internal static class MessageFormatting
  {
    public static T ParseOneLetterEnumByValue<T>(string s)
    {
      if (string.IsNullOrWhiteSpace(s))
        return default(T);
      return (T) Enum.Parse(typeof (T), ((int) s[0]).ToString());
    }

    /// <summary>
    ///   Formats enum value as letter
    /// </summary>
    /// <returns></returns>
    public static string F(object enumvalue)
    {
      return 0 == (int) enumvalue ? string.Empty : Convert.ToChar(enumvalue).ToString();
    }

    public static T ParseEnum<T>(string s)
    {
      if (string.IsNullOrWhiteSpace(s))
        return default(T);
      return (T) Enum.Parse(typeof (T), s);
    }
  }
}