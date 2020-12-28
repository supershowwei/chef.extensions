using System;
using Chef.Extensions.Float;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class FloatTest
    {
        [TestMethod]
        public void Test_RoundUp_with_Digitals_is_2()
        {
            var input = 11.1234f;
            var result = input.RoundUp(2);

            result.Should().Be(11.13f);
        }
        
        [TestMethod]
        public void Test_RoundDown_with_Digitals_is_3()
        {
            var input = 11.1234f;
            var result = input.RoundDown(3);

            result.Should().Be(11.123f);
        }
    }
}
