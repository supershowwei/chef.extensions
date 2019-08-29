using System;
using Chef.Extensions.Double;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class DoubleTest
    {
        [TestMethod]
        public void Test_Gradient_Calculate_11_Change_Rate_With_10()
        {
            var input = 11d;
            var baseValue = 10d;

            var result = input.Gradient(baseValue);

            result.Should().Be(0.1d);
        }
    }
}
