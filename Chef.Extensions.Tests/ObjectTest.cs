using Chef.Extensions.Object;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class ObjectTest
    {
        [TestMethod]
        public void Test_To_Change_Long_To_Decimal()
        {
            var input = long.MaxValue;

            var result = input.To<decimal>();

            result.Should().Be(long.MaxValue);
        }

        [TestMethod]
        public void Test_ToNullable_Change_Long_To_Nullable_Decimal()
        {
            var input = long.MaxValue;

            var result = input.ToNullable<decimal>();

            result.Value.Should().Be(long.MaxValue);
        }

        [TestMethod]
        public void Test_ToNullable_Change_Null_To_Nullable_Decimal()
        {
            object input = null;

            var result = input.ToNullable<decimal>();

            result.Should().BeNull();
        }
    }
}