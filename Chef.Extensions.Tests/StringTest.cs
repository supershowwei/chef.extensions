using System;
using System.Text;
using Chef.Extensions.String;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void Test_IsNumeric_If_String_Is_Valid_Format()
        {
            "123".IsNumeric().Should().BeTrue();
            "123.123".IsNumeric().Should().BeTrue();
            "123,123".IsNumeric().Should().BeTrue();
            "-123".IsNumeric().Should().BeTrue();
            "-123.123".IsNumeric().Should().BeTrue();
            "-123,123".IsNumeric().Should().BeTrue();
        }

        [TestMethod]
        public void Test_IsNumeric_If_String_Is_Invalid_Format()
        {
            "$123".IsNumeric().Should().BeFalse();
            "A123".IsNumeric().Should().BeFalse();
            "A1(3".IsNumeric().Should().BeFalse();
            "12-34".IsNumeric().Should().BeFalse();
        }

        [TestMethod]
        public void Test_Remove()
        {
            var input = "1[2]345[6[7]]";

            input.Remove("[", "]").Should().Be("1234567");
        }

        [TestMethod]
        public void Test_ToUrlBase64()
        {
            var input = "1234567一二三四五六七";

            var result = input.ToUrlBase64();

            result.Should().Be("MTIzNDU2N-S4gOS6jOS4ieWbm-S6lOWFreS4gw..");
        }

        [TestMethod]
        public void Test_ToUrlBase64_use_Big5_Encoding()
        {
            var input = "1234567一二三四五六七";

            var result = input.ToUrlBase64(Encoding.GetEncoding("Big5"));

            result.Should().Be("MTIzNDU2N6RApEekVKV8pK2ku6RD");
        }

        [TestMethod]
        public void Test_UrlBase64Decode()
        {
            var input = "MTIzNDU2N-S4gOS6jOS4ieWbm-S6lOWFreS4gw..";

            var result = input.UrlBase64Decode();

            result.Should().Be("1234567一二三四五六七");
        }
    }
}
