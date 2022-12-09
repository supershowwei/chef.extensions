﻿using System;
using System.Security.Cryptography;
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
        public void Test_Base64UrlEncode_and_Base64UrlDecode()
        {
            for (var i = 0; i < 100; i++)
            {
                var s = BitConverter.ToString(SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())));

                s.Base64UrlEncode().Base64UrlDecode().Should().Be(s);
            }
        }
    }
}
