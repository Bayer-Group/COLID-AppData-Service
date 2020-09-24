using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Common.Utilities
{
    public static class Guard
    {
        public static void IsNotNull(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), $"{nameof(obj)} cannot be null");
        }

        public static void IsPositiveInteger(int value)
        {
            if (value <= 0)
                throw new ArgumentException(nameof(value), $"{nameof(value)} cannot be 0 or negative");
        }

        public static void IsNotNull(params object[] objs)
        {
            foreach (var obj in objs)
            {
                IsNotNull(obj);
            }
        }

        public static void IsNotNullOrEmpty<T>(ICollection<T> col)
        {
            if (col == null || !col.Any())
                throw new ArgumentNullException(nameof(col), $"{nameof(col)} cannot be null or empty");
        }

        public static void IsNotEmpty(string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException(nameof(str), $"{nameof(str)} cannot be empty");
        }

        public static void IsValidUri(Uri uri)
        {
            IsNotNull(uri);
            if (string.IsNullOrWhiteSpace(uri.ToString()))
                throw new UriFormatException($"The given URI is empty");

            if (!uri.IsAbsoluteUri)
                throw new UriFormatException($"The given URI is not absolute.");
        }

        public static void IsValidGuidOrEmail(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException($"The given argument is empty/null");

            var validGuid = IsValid(IsValidGuid, str);
            var validEmail = IsValid(IsValidEmail, str);

            if (!validGuid && !validEmail)
            {
                throw new FormatException($"Given parameter does not consist a valid email or guid: {str}");
            }
        }

        private static bool IsValid(Action<string> guard, string value)
        {
            try
            {
                guard(value);
                return true;
            }
            catch (System.Exception ex)
            {
                if (ex is FormatException || ex is ArgumentException)
                {
                    return false;
                }
                throw;
            }
        }

        public static void IsValidGuid(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                throw new ArgumentNullException($"The given argument is empty/null");

            Guid.Parse(guid);
        }

        public static void ContainsValidEmails(IEnumerable<string> emails)
        {
            foreach (var mail in emails)
            {
                IsValidEmail(mail);
            }
        }

        // Taken from https://docs.microsoft.com/de-de/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        public static void IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException($"The given argument is empty/null");

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                throw new ArgumentException("Email parsing took too long.", e);
            }

            try
            {
                var matched = Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                if (!matched)
                {
                    throw new ArgumentException("Given parameter does not consist a valid email: " + email);
                }
            }
            catch (RegexMatchTimeoutException)
            {
                throw new ArgumentException("Email parsing took too long.");
            }
        }

        public static void IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                throw new ArgumentNullException(nameof(jsonString), $"{nameof(jsonString)} cannot be empty or whitespaces");
            }

            JToken.Parse(jsonString);
        }
    }
}
