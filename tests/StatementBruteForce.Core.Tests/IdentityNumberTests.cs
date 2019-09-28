using System;
using Xunit;

namespace StatementBruteForce.Core.Tests
{
    public class IdentityNumberTests
    {
        [Theory]
        [InlineData("7802220494084", GenderType.Female)]
        [InlineData("8704305219080", GenderType.Male)]
        [InlineData("8306135187089", GenderType.Male)]
        [InlineData("6006260214086", GenderType.Female)]
        [InlineData("5210100119080", GenderType.Female)]
        [InlineData("4412055047081", GenderType.Male)]
        [InlineData("3812090108080", GenderType.Female)]
        [InlineData("5307180029088", GenderType.Female)]
        [InlineData("7403190804087", GenderType.Female)]
        [InlineData("6302265043087", GenderType.Male)]
        [InlineData("5701285081087", GenderType.Male)]
        [InlineData("4010205028082", GenderType.Male)]
        [InlineData("7912070888085", GenderType.Female)]
        [InlineData("7810060205080", GenderType.Female)]
        [InlineData("8704190138080", GenderType.Female)]
        [InlineData("6203310181080", GenderType.Female)]
        [InlineData("8208305132087", GenderType.Male)]
        [InlineData("9210136150089", GenderType.Male)]
        [InlineData("6101175022086", GenderType.Male)]
        [InlineData("7406275473082", GenderType.Male)]
        public void MatchesExpectedGender(string identityNumber, GenderType gender)
        {
            var result =
                SouthAfricanIdentityNumberUtil.ParseIdentityNumberStringToModel(identityNumber: identityNumber);
            Assert.Equal(expected: gender, actual: result.Gender);
        }

        [Theory]
        [InlineData("650207538708", 3)]
        [InlineData("630412070608", 0)]
        [InlineData("520317505108", 0)]
        [InlineData("930631755508", 6)]
        [InlineData("820431870208", 1)]
        [InlineData("400925523008", 5)]
        [InlineData("570811509008", 9)]
        [InlineData("850631195508", 0)]
        [InlineData("651210504808", 4)]
        [InlineData("800631260708", 7)]
        [InlineData("460713022808", 1)]
        [InlineData("781001109808", 9)]
        [InlineData("750212504308", 1)]
        [InlineData("520115574908", 7)]
        [InlineData("770510535808", 6)]
        [InlineData("860525099108", 8)]
        [InlineData("840715049308", 7)]
        [InlineData("960631985608", 9)]
        [InlineData("540523019808", 3)]
        [InlineData("770631487808", 8)]
        public void MatchesExpectedChecksum(string identityNumberSection, int checksum)
        {
            var result =
                SouthAfricanIdentityNumberUtil.CalculateLuhnChecksumDigit(identityNumberSection: identityNumberSection);

            Assert.Equal(expected: checksum, actual: result);
        }

        [Theory]
        [InlineData("6502075387083", true)]
        [InlineData("6304120706080", true)]
        [InlineData("5203175051080", true)]
        [InlineData("9306317555086", false)]
        [InlineData("8204318702081", false)]
        [InlineData("4009255230085", true)]
        [InlineData("5708115090089", true)]
        [InlineData("8506311955080", false)]
        [InlineData("6512105048084", true)]
        [InlineData("8006312607087", false)]
        [InlineData("4607130228081", true)]
        [InlineData("7810011098089", true)]
        [InlineData("7502125043081", true)]
        [InlineData("5201155749087", true)]
        [InlineData("7705105358086", true)]
        [InlineData("8605250991088", true)]
        [InlineData("8407150493087", true)]
        [InlineData("9606319856089", false)]
        [InlineData("5405230198083", true)]
        [InlineData("7706314878088", false)]
        public void MatchesExpectedValidation(string identityNumber, bool valid)
        {
            var result = identityNumber.IsValidSouthAfricanIdentityNumber();

            Assert.Equal(expected: valid, actual: result);
        }


        [Theory]
        [InlineData("6004090561081")]
        [InlineData("5709215210080")]
        [InlineData("7207170346088")]
        [InlineData("7412245919089")]
        [InlineData("7606030393083")]
        [InlineData("6005260867083")]
        [InlineData("7407090156084")]
        [InlineData("9202235109082")]
        [InlineData("5108180632081")]
        [InlineData("8810295835080")]
        [InlineData("8802011051084")]
        [InlineData("4312235118085")]
        [InlineData("8010040100084")]
        [InlineData("7209160678082")]
        [InlineData("5406140777081")]
        [InlineData("5802026608087")]
        [InlineData("5001275749086")]
        [InlineData("8203076295080")]
        [InlineData("7805225285084")]
        [InlineData("7903310194087")]
        public void PermutationContainsIdentityNumber(string identityNumber)
        {
            var masked = identityNumber;
            masked = masked.Remove(startIndex: 6, count: 4).Insert(startIndex: 6, value: "****");
            var seed =
                SouthAfricanIdentityNumberUtil.ParseIdentityNumberStringToModel(identityNumber: masked);
            var result = SouthAfricanIdentityNumberUtil.GenerateValidIdentityNumbers(seedModel: seed);

            Assert.Contains(expected: identityNumber, collection: result, comparer: StringComparer.OrdinalIgnoreCase);
        }
    }
}