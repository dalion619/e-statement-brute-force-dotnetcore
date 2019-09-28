using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StatementBruteForce.Core
{
    /// <summary>
    ///     South African identity number utility for common use cases.
    /// </summary>
    public static class SouthAfricanIdentityNumberUtil
    {
        /// <summary>
        ///     Calculates the 13th checksum digit of an identity number using the Luhn algorithm.
        /// </summary>
        /// <param name="identityNumberSection">First 12 digits of an identity number, defined as YYMMDDSSSSCA.</param>
        /// <returns>
        ///     A checksum digit.
        /// </returns>
        /// See: https://knowles.co.za/generating-south-african-id-numbers/
        public static int CalculateLuhnChecksumDigit(string identityNumberSection)
        {
            if (string.IsNullOrWhiteSpace(value: identityNumberSection))
            {
                throw new ArgumentException(message: $"{nameof(identityNumberSection)} is required.",
                    paramName: nameof(identityNumberSection));
            }

            var total = 0;
            var count = 0;
            for (var i = 0; i < identityNumberSection.Length; i++)
            {
                var multiple = count % 2 + 1;
                count++;
                var temp = multiple * CharUnicodeInfo.GetDigitValue(ch: identityNumberSection[index: i]);
                var subtotal = Math.Floor(d: (double) temp / 10) + temp % 10;
                total += (int) subtotal;
            }

            var checksumDigit = total * 9 % 10;
            return checksumDigit;
        }

        /// <summary>
        ///     Validates a South African identity number.
        /// </summary>
        /// <param name="identityNumber">13 digit South African identity number, defined as YYMMDDSSSSCAZ.</param>
        /// <returns>
        ///     Either either true or false.
        /// </returns>
        public static bool IsValidSouthAfricanIdentityNumber(this string identityNumber)
        {
            if (string.IsNullOrWhiteSpace(value: identityNumber))
            {
                return false;
            }

            if (new string(value: Array.FindAll(array: identityNumber.ToArray(), match: c => char.IsDigit(c: c)))
                    .Length != 13)
            {
                return false;
            }

            DateTime dateOfBirth;
            if (!DateTime.TryParse(
                s: $"19{identityNumber.Substring(startIndex: 0, length: 2)}-{identityNumber.Substring(startIndex: 2, length: 2)}-{identityNumber.Substring(startIndex: 4, length: 2)}",
                provider: CultureInfo.InvariantCulture,
                styles: DateTimeStyles.AllowWhiteSpaces, result: out dateOfBirth))
            {
                return false;
            }

            var identityNumberChecksumDigit =
                CharUnicodeInfo.GetDigitValue(ch: identityNumber[index: identityNumber.Length - 1]);
            var identityNumberSection = identityNumber.Substring(startIndex: 0, length: identityNumber.Length - 1);

            return identityNumberChecksumDigit ==
                   CalculateLuhnChecksumDigit(identityNumberSection: identityNumberSection);
        }

        /// <summary>
        ///     Generates valid identity numbers.
        /// </summary>
        /// <param name="seedModel">
        ///     A <see cref="StatementBruteForce.Core.SouthAfricanIdentityNumberModel" /> object used as seed value for generating
        ///     permutations.
        /// </param>
        /// <param name="genderType">
        ///     An enum option to specify the male or female identity numbers if desired.
        /// </param>
        /// <returns>
        ///     List of valid identity numbers.
        /// </returns>
        public static List<string> GenerateValidIdentityNumbers(SouthAfricanIdentityNumberModel seedModel,
                                                                GenderType? genderType = null)
        {
            var list = new List<string>();

            // If no values are provided, loop through possible values in a nested manner.
            var startMonth = 1;
            var endMonth = 12;
            if (!string.IsNullOrEmpty(value: seedModel.MonthOfBirth))
            {
                var month = Convert.ToInt32(value: seedModel.MonthOfBirth);
                startMonth = month;
                endMonth = month;
            }

            for (var m = startMonth; m <= endMonth; m++)
            {
                var startDay = 1;
                var endDay = DateTime.DaysInMonth(year: seedModel.Year, month: m);
                if (!string.IsNullOrEmpty(value: seedModel.DayOfBirth))
                {
                    var day = Convert.ToInt32(value: seedModel.DayOfBirth);
                    startDay = day;
                    endDay = day;
                }

                for (var d = startDay; d <= endDay; d++)
                {
                    var startGender = 0;
                    var endGender = 9;
                    if (genderType.HasValue)
                    {
                        if (genderType == GenderType.Male)
                        {
                            startGender = 5;
                        }
                        else
                        {
                            endGender = 4;
                        }
                    }

                    if (!string.IsNullOrEmpty(value: seedModel.GenderDigit))
                    {
                        var gender = Convert.ToInt32(value: seedModel.GenderDigit);
                        startGender = gender;
                        endGender = gender;
                    }

                    for (var g = startGender; g <= endGender; g++)
                    {
                        var startSequence = 0;
                        var endSequence = 999;

                        if (!string.IsNullOrEmpty(value: seedModel.GenderSequenceDigit1))
                        {
                            startSequence += Convert.ToInt32(value: seedModel.GenderSequenceDigit1) * 100;
                            endSequence = startSequence + 99;
                        }

                        if (!string.IsNullOrEmpty(value: seedModel.GenderSequenceDigit2))
                        {
                            startSequence += Convert.ToInt32(value: seedModel.GenderSequenceDigit2) * 10;
                        }

                        if (!string.IsNullOrEmpty(value: seedModel.GenderSequenceDigit3))
                        {
                            startSequence += Convert.ToInt32(value: seedModel.GenderSequenceDigit3);
                        }

                        for (var s = startSequence; s <= endSequence; s++)
                        {
                            var startCitizenship = 0;
                            var endCitizenship = 1;
                            if (seedModel.Citizenship.HasValue)
                            {
                                var citizenship = seedModel.Citizenship.Value;
                                if (citizenship == CitizenshipType.SA)
                                {
                                    endCitizenship = (int) CitizenshipType.SA;
                                }
                                else
                                {
                                    startCitizenship = (int) CitizenshipType.Other;
                                }
                            }

                            for (var c = startCitizenship; c <= endCitizenship; c++)
                            {
                                var startObsolete = 8;
                                var endObsolete = 9;
                                if (!string.IsNullOrEmpty(value: seedModel.ObsoleteDigit))
                                {
                                    var obsolete = Convert.ToInt32(value: seedModel.ObsoleteDigit);
                                    startObsolete = obsolete;
                                    endObsolete = obsolete;
                                }

                                for (var o = startObsolete; o <= endObsolete; o++)
                                {
                                    var numberSection = $"{seedModel.YearOfBirth:D2}{m:D2}{d:D2}{g}{s:D3}{c}{o}";
                                    if (!string.IsNullOrEmpty(value: seedModel.ChecksumDigit))
                                    {
                                        numberSection += seedModel.ChecksumDigit;
                                    }
                                    else
                                    {
                                        numberSection +=
                                            CalculateLuhnChecksumDigit(identityNumberSection: numberSection).ToString();
                                    }

                                    if (numberSection.IsValidSouthAfricanIdentityNumber())
                                    {
                                        list.Add(item: numberSection);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        ///     Maps <paramref name="identityNumber" /> to a
        ///     <see cref="StatementBruteForce.Core.SouthAfricanIdentityNumberModel" /> object.
        /// </summary>
        /// <param name="identityNumber">13 digit South African identity number, defined as YYMMDDSSSSCAZ.</param>
        /// <returns>
        ///     A <see cref="StatementBruteForce.Core.SouthAfricanIdentityNumberModel" /> object.
        /// </returns>
        public static SouthAfricanIdentityNumberModel ParseIdentityNumberStringToModel(string identityNumber)
        {
            var chars = identityNumber.ToCharArray();

        #region local functions

            int yy()
            {
                if (char.IsDigit(c: chars[0]) && char.IsDigit(c: chars[1]))
                {
                    return 1900 + 10 * CharUnicodeInfo.GetDigitValue(ch: chars[0]) +
                           CharUnicodeInfo.GetDigitValue(ch: chars[1]);
                }

                return -1;
            }

            int mm()
            {
                if (char.IsDigit(c: chars[2]) && char.IsDigit(c: chars[3]))
                {
                    return 10 * CharUnicodeInfo.GetDigitValue(ch: chars[2]) +
                           CharUnicodeInfo.GetDigitValue(ch: chars[3]);
                }

                return -1;
            }

            int dd()
            {
                if (char.IsDigit(c: chars[4]) && char.IsDigit(c: chars[5]))
                {
                    return 10 * CharUnicodeInfo.GetDigitValue(ch: chars[4]) +
                           CharUnicodeInfo.GetDigitValue(ch: chars[5]);
                }

                return -1;
            }

        #endregion


            var model = new SouthAfricanIdentityNumberModel(yearOfBirth: yy(), monthOfBirth: mm(), dayOfBirth: dd(),
                gender: char.IsDigit(c: chars[6]) ? CharUnicodeInfo.GetDigitValue(ch: chars[6]) : -1,
                genderSequence1: char.IsDigit(c: chars[7]) ? CharUnicodeInfo.GetDigitValue(ch: chars[7]) : -1,
                genderSequence2: char.IsDigit(c: chars[8]) ? CharUnicodeInfo.GetDigitValue(ch: chars[8]) : -1,
                genderSequence3: char.IsDigit(c: chars[9]) ? CharUnicodeInfo.GetDigitValue(ch: chars[9]) : -1,
                citizenship: char.IsDigit(c: chars[10]) ? CharUnicodeInfo.GetDigitValue(ch: chars[10]) : -1,
                obsolete: char.IsDigit(c: chars[11]) ? CharUnicodeInfo.GetDigitValue(ch: chars[11]) : -1,
                checksum: char.IsDigit(c: chars[12]) ? CharUnicodeInfo.GetDigitValue(ch: chars[12]) : -1);
            return model;
        }
    }
}