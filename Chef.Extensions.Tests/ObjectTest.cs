using System;
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

        [TestMethod]
        public void Test_IsNumeric()
        {
            byte.MaxValue.IsNumeric().Should().BeTrue();
            ((byte?)0x01).IsNumeric().Should().BeTrue();
            sbyte.MaxValue.IsNumeric().Should().BeTrue();
            ((sbyte?)0x02).IsNumeric().Should().BeTrue();
            short.MaxValue.IsNumeric().Should().BeTrue();
            ((short?)1).IsNumeric().Should().BeTrue();
            ushort.MaxValue.IsNumeric().Should().BeTrue();
            ((ushort?)2).IsNumeric().Should().BeTrue();
            int.MaxValue.IsNumeric().Should().BeTrue();
            uint.MaxValue.IsNumeric().Should().BeTrue();
            ((uint?)3).IsNumeric().Should().BeTrue();
            long.MaxValue.IsNumeric().Should().BeTrue();
            ((long?)4).IsNumeric().Should().BeTrue();
            ulong.MaxValue.IsNumeric().Should().BeTrue();
            ((ulong?)5).IsNumeric().Should().BeTrue();
            float.MaxValue.IsNumeric().Should().BeTrue();
            ((float?)1.1).IsNumeric().Should().BeTrue();
            double.MaxValue.IsNumeric().Should().BeTrue();
            ((double?)2.2).IsNumeric().Should().BeTrue();
            decimal.MaxValue.IsNumeric().Should().BeTrue();
            ((decimal?)3.3).IsNumeric().Should().BeTrue();

            ((int?)null).IsNumeric().Should().BeFalse();
            true.IsNumeric().Should().BeFalse();
            string.Empty.IsNumeric().Should().BeFalse();
        }
    }
}