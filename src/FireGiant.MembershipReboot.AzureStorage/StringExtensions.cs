// Copyright (c) FireGiant.  All Rights Reserved.

using System;
using System.Text;

namespace FireGiant.MembershipReboot.AzureStorage
{
    internal static class StringExtensions
    {
        public static string ToBase64(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static string FromBase64(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            value = value.Replace('-', '+').Replace('_', '/');

            var mod = value.Length % 4;
            if (mod > 0)
            {
                value += new string('=', 4 - mod);
            }

            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes); ;
        }
    }
}
