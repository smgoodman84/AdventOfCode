using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode2020;

namespace AdventOfCode._2020.Day04
{
    public class Day04 : Day
    {
        public Day04() : base(2020, 4, "Day04/input_2020_04.txt", "192", "101")
        {

        }

        private List<Passport> _passports;
        public override void Initialise()
        {
            _passports = LineGrouper.GroupLinesBySeperator(InputLines)
                .Select(CreatePassport)
                .ToList();
        }

        private static Passport CreatePassport(List<string> lines)
        {
            var passport = new Passport();
            var joinedLines = string.Join(' ', lines);
            var fields = joinedLines.Split(' ').Select(f => f.Trim()).ToList();
            foreach (var field in fields)
            {
                var keyValue = field.Split(':').ToArray();
                var key = keyValue[0].Trim();
                var value = keyValue.Length > 1 ? keyValue[1].Trim() : null;
                switch(key)
                {
                    case "byr":
                        passport.BirthYear = int.Parse(value);
                        break;
                    case "iyr":
                        passport.IssueYear = int.Parse(value);
                        break;
                    case "eyr":
                        passport.ExpirationYear = int.Parse(value);
                        break;
                    case "hgt":
                        passport.Height = value;
                        break;
                    case "hcl":
                        passport.HairColour = value;
                        break;
                    case "ecl":
                        passport.EyeColour = value;
                        break;
                    case "pid":
                        passport.PassportId = value;
                        break;
                    case "cid":
                        passport.CountryId = int.Parse(value);
                        break;
                }
            }

            return passport;
        }

        private bool IsValidPassport(Passport passport)
        {
            return passport.BirthYear != null
                && passport.IssueYear != null
                && passport.ExpirationYear != null
                && passport.Height != null
                && passport.HairColour != null
                && passport.EyeColour != null
                && passport.PassportId != null;
        }

        private HashSet<string> _validEyeColours = new HashSet<string>(new List<string>
        {
            "amb", "blu", "brn", "gry", "grn", "hzl", "oth"
        });

        private bool IsProperlyValidPassport(Passport passport)
        {
            if (passport.BirthYear == null)
            {
                return false;
            }

            if (passport.BirthYear < 1920)
            {
                return false;
            }

            if (passport.BirthYear > 2002)
            {
                return false;
            }

            if (passport.IssueYear == null)
            {
                return false;
            }

            if (passport.IssueYear < 2010)
            {
                return false;
            }

            if (passport.IssueYear > 2020)
            {
                return false;
            }

            if (passport.ExpirationYear == null)
            {
                return false;
            }

            if (passport.ExpirationYear < 2020)
            {
                return false;
            }

            if (passport.ExpirationYear > 2030)
            {
                return false;
            }

            if (passport.Height == null)
            {
                return false;
            }

            if (!ValidHeight(passport.Height))
            {
                return false;
            }

            if (passport.HairColour == null)
            {
                return false;
            }

            if (!ValidHairColour(passport.HairColour))
            {
                return false;
            }

            if (passport.EyeColour == null)
            {
                return false;
            }

            if (!_validEyeColours.Contains(passport.EyeColour))
            {
                return false;
            }

            if (passport.PassportId == null)
            {
                return false;
            }

            if (passport.PassportId.Length != 9)
            {
                return false;
            }

            if (!passport.PassportId.All(c => c >= '0' && c <= '9'))
            {
                return false;
            }

            return true;
        }

        private bool ValidHairColour(string hairColour)
        {
            if (hairColour.Length != 7)
            {
                return false;
            }

            if (!hairColour.StartsWith('#'))
            {
                return false;
            }

            var colourCode = hairColour.Substring(1);
            if (!colourCode.All(IsValidHexDigit))
            {
                return false;
            }

            return true;
        }

        private bool IsValidHexDigit(char digit)
        {
            return (digit >= '0' && digit <= '9')
                || (digit >= 'a' && digit <= 'f')
                || (digit >= 'A' && digit <= 'F');
        }

        private bool ValidHeight(string height)
        {
            if (height.EndsWith("cm"))
            {
                var cms = int.Parse(height.Substring(0, height.Length - 2));

                if (cms < 150)
                {
                    return false;
                }

                if (cms > 193)
                {
                    return false;
                }

                return true;
            }

            if (height.EndsWith("in"))
            {
                var ins = int.Parse(height.Substring(0, height.Length - 2));

                if (ins < 59)
                {
                    return false;
                }

                if (ins > 76)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public override string Part1() => _passports.Count(IsValidPassport).ToString();

        public override string Part2() => _passports.Count(IsProperlyValidPassport).ToString();

        private class Passport
        {
            public int? BirthYear { get; set; }
            public int? IssueYear { get; set; }
            public int? ExpirationYear { get; set; }
            public string Height { get; set; }
            public string HairColour { get; set; }
            public string EyeColour { get; set; }
            public string PassportId { get; set; }
            public int? CountryId { get; set; }
        }
    }
}
