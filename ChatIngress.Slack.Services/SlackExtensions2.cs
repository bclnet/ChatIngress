using System;

namespace ChatIngress.Slack
{
    /// <summary>
    /// SlackExtensions
    /// </summary>
    internal static class SlackExtensions2
    {
        public static string Left(this string source, int length) => source[..Math.Min(length, source.Length)];
        public static string Right(this string source, int length) => source[^(Math.Min(length, source.Length))..];

        public static DateTime ClampSeconds(this DateTime source) => source.AddMilliseconds(-source.Millisecond);
    }
}
