using System;
using Chef.Extensions.Decimal;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class DecimalTest
    {
        [TestMethod]
        public void Test_RoundUp_4_Digits_Round_To_2_Digits()
        {
            var input = 1.1234m;

            var result = input.RoundUp(2);

            result.Should().Be(1.13m);
        }

        [TestMethod]
        public void Test_RoundDown_4_Digits_Round_To_2_Digits()
        {
            var input = 1.1234m;

            var result = input.RoundDown(2);

            result.Should().Be(1.12m);
        }
    }
}
