namespace System;

public static class StringExtensions
{
    public static string Right(this string value, int length)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length <= length) return value;

        return value.Substring(value.Length - length, length);
    }
}
