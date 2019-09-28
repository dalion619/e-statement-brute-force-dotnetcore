using System;

namespace StatementBruteForce.Core
{
    /// <summary>
    ///     Class SouthAfricanIdentityNumberModel builds an object that is the seed input for
    ///     <see
    ///         cref="StatementBruteForce.Core.SouthAfricanIdentityNumberUtil.GenerateValidIdentityNumbers(SouthAfricanIdentityNumberModel,System.Nullable{StatementBruteForce.Core.GenderType})" />
    ///     to generate valid identity numbers.
    /// </summary>
    /// <remarks>
    ///     A South African ID number is a 13-digit number which is defined by the following format: YYMMDDSSSSCAZ.
    /// </remarks>
    /// <list type="bullet">
    ///     <item>
    ///         <description>YYMMDD, these 6 digits are based on your date of birth. 20 February 1992 is displayed as 920220.</description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             SSSS, these 4 digits are used to define your gender. Females are assigned numbers in the range
    ///             0000-4999 and males from 5000-9999.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             C, this digit shows if you're an SA citizen status with 0 denoting that you were born a SA
    ///             citizen and 1 denoting that you're a permanent resident.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             A, this digit was historically used to indicate race but now is obsolete. Usually 8, or 9 but can
    ///             be other values.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Z, the last digit is a checksum digit – used to check that the number sequence is accurate using
    ///             a set formula called the Luhn algorithm.
    ///         </description>
    ///     </item>
    /// </list>
    /// See: https://www.westerncape.gov.za/general-publication/decoding-your-south-african-id-number-0
    public class SouthAfricanIdentityNumberModel
    {
        private readonly int? _checksumDigit;
        private readonly int? _citizenshipDigit;
        private readonly int? _dayOfBirth;
        private readonly int? _genderDigit;
        private readonly int? _genderSequence1;
        private readonly int? _genderSequence2;
        private readonly int? _genderSequence3;
        private readonly int? _monthOfBirth;
        private readonly int? _obsoleteDigit;
        private readonly int _yearOfBirth;

        /// <summary>
        ///     Initialise with known variables if provided or set defaults.
        /// </summary>
        /// <param name="yearOfBirth">YY</param>
        /// <param name="monthOfBirth">MM</param>
        /// <param name="dayOfBirth">DD</param>
        /// <param name="gender">S0</param>
        /// <param name="genderSequence1">S1</param>
        /// <param name="genderSequence2">S2</param>
        /// <param name="genderSequence3">S3</param>
        /// <param name="citizenship">C</param>
        /// <param name="obsolete">A</param>
        /// <param name="checksum">Z</param>
        public SouthAfricanIdentityNumberModel(int yearOfBirth = -1, int monthOfBirth = -1, int dayOfBirth = -1,
                                               int gender = -1, int genderSequence1 = -1,
                                               int genderSequence2 = -1, int genderSequence3 = -1,
                                               int citizenship = -1, int obsolete = -1, int checksum = -1)
        {
            var dt = DateTime.UtcNow;

            _yearOfBirth = yearOfBirth;
            Year = yearOfBirth;
            if (yearOfBirth < 1900 || yearOfBirth > dt.Year)
            {
                _yearOfBirth = dt.Year;
                Year = dt.Year;
            }

            if (monthOfBirth > 0 && monthOfBirth <= 12)
            {
                _monthOfBirth = monthOfBirth;
            }

            if (dayOfBirth > 0 && dayOfBirth <= 31)
            {
                _dayOfBirth = dayOfBirth;
            }

            if (gender != -1)
            {
                _genderDigit = gender;
                Gender = gender < 5 ? GenderType.Female : GenderType.Male;
            }

            if (genderSequence1 != -1)
            {
                _genderSequence1 = genderSequence1;
            }

            if (genderSequence2 != -1)
            {
                _genderSequence2 = genderSequence2;
            }

            if (genderSequence3 != -1)
            {
                _genderSequence3 = genderSequence3;
            }

            if (citizenship != -1)
            {
                _citizenshipDigit = citizenship;
                Citizenship = citizenship == 0 ? CitizenshipType.SA : CitizenshipType.Other;
            }

            if (obsolete != -1)
            {
                _obsoleteDigit = obsolete;
            }

            if (checksum != -1)
            {
                _checksumDigit = checksum;
            }
        }

        public string YearOfBirth => _yearOfBirth.ToString().Substring(startIndex: 2, length: 2);

        public string MonthOfBirth => _monthOfBirth.ToString();

        public string DayOfBirth => _dayOfBirth.ToString();

        public string GenderDigit => _genderDigit.ToString();

        public string GenderSequenceDigit1 => _genderSequence1.ToString();
        public string GenderSequenceDigit2 => _genderSequence2.ToString();
        public string GenderSequenceDigit3 => _genderSequence3.ToString();
        public string ObsoleteDigit => _obsoleteDigit.ToString();
        public string ChecksumDigit => _checksumDigit.ToString();
        public int Year { get; }
        public GenderType? Gender { get; }
        public CitizenshipType? Citizenship { get; }
    }

    public enum GenderType
    {
        Female = 4,
        Male = 9
    }

    public enum CitizenshipType
    {
        SA = 0,
        Other = 1
    }
}