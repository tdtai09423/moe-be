using System;
using System.Collections.Generic;
using System.Linq;
using MOE_System.Domain.Entities;

namespace MOE_System.Infrastructure.Data.Seeding
{
    public static class ResidentSeedData
    {
        private static readonly string[] FirstNames = new[]
        {
            "John","Mary","David","Linda","Michael","Susan","Daniel","Grace","Andrew","Emily",
            "James","Anna","Peter","Rachel","Thomas","Sophie","Kevin","Laura","Brian","Angela"
        };

        private static readonly string[] LastNames = new[]
        {
            "Tan","Lim","Lee","Ng","Wong","Chong","Goh","Koh","Lau","Ho",
            "Chan","Teo","Ong","Chua","Chin","Low","Pereira","Singh","Kumar","Nguyen"
        };

        private static readonly string[] Sexes = new[] { "M", "F" };
        private static readonly string[] Races = new[] { "Chinese", "Malay", "Indian", "Eurasian", "Other" };
        private static readonly string[] ResidentialStatuses = new[] { "Citizen", "Permanent Resident", "Foreigner" };

        /// <summary>
        /// Generate a list of residents following the simplified NRIC rules described.
        /// </summary>
        /// <param name="count">Number of residents to generate (default 50)</param>
        /// <returns>List of Resident objects</returns>
        public static List<Resident> GetResidents(int count = 50)
        {
            var rnd = new Random();
            var residents = new List<Resident>(count);
            var usedNrics = new HashSet<string>();

            while (residents.Count < count)
            {
                // Random date of birth between 1940-01-01 and 2010-12-31
                var year = rnd.Next(1940, 2011);
                var month = rnd.Next(1, 13);
                var day = rnd.Next(1, DateTime.DaysInMonth(year, month) + 1);
                var dob = new DateOnly(year, month, day);

                // Determine prefix
                char prefix;
                var cutoff = new DateOnly(2000, 1, 1);
                if (dob < cutoff)
                    prefix = 'S';
                else
                    prefix = rnd.NextDouble() < 0.5 ? 'T' : 'M';

                // Build 7-digit number
                string sevenDigits;
                // Always set first two digits to last two digits of birth year to match user's requirement
                var yearSuffix = (year % 100).ToString("D2");
                var remaining = rnd.Next(0, 100000).ToString("D5");
                sevenDigits = yearSuffix + remaining;

                // checksum letter A-Z
                var checksum = (char)('A' + rnd.Next(0, 26));

                var nric = string.Concat(prefix, sevenDigits, checksum);

                // ensure uniqueness
                if (usedNrics.Contains(nric))
                    continue;

                usedNrics.Add(nric);

                // random name
                var first = FirstNames[rnd.Next(FirstNames.Length)];
                var last = LastNames[rnd.Next(LastNames.Length)];
                var fullName = $"{first} {last}";

                // assemble resident
                var resident = new Resident
                {
                    NRIC = nric,
                    PrincipalName = fullName,
                    Sex = Sexes[rnd.Next(Sexes.Length)],
                    Race = Races[rnd.Next(Races.Length)],
                    SecondaryRace = string.Empty,
                    Dialect = string.Empty,
                    DateOfBirth = dob,
                    ResidentialStatus = ResidentialStatuses[rnd.Next(ResidentialStatuses.Length)],
                    Nationality = "Singaporean",
                    Country = "Singapore",
                    MobileNumber = GenerateMobile(rnd),
                    EmailAddress = GenerateEmail(first, last, nric),
                    RegisteredAddress = GenerateAddress(rnd)
                };

                residents.Add(resident);
            }

            return residents;
        }

        private static string GenerateMobile(Random rnd)
        {
            // Singapore mobile numbers: 8 digits, often starting with 8 or 9
            var first = rnd.NextDouble() < 0.5 ? '8' : '9';
            var rest = rnd.Next(0, 10000000).ToString("D7");
            return first + rest;
        }

        private static string GenerateEmail(string first, string last, string nric)
        {
            var local = ($"{first}.{last}".ToLower()).Replace(" ", "");
            var domain = "example.com";
            // include last 4 of NRIC to help uniqueness
            var suffix = nric.Length >= 4 ? nric[^4..] : nric;
            return $"{local}{suffix}@{domain}";
        }

        private static string GenerateAddress(Random rnd)
        {
            var blocks = rnd.Next(1, 300);
            var streetNames = new[] { "Ang Mo Kio Ave", "Boon Lay Way", "Toa Payoh Lor", "Yishun Ave", "Bedok North Rd", "Clementi Rd" };
            var street = streetNames[rnd.Next(streetNames.Length)];
            return $"Blk {blocks}, {street}";
        }
    }
}
