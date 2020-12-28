using System;
using System.ComponentModel;
using Chef.Extensions.Double;
using Chef.Extensions.Enum;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    public enum KindOfBrokerStatus
    {
        [System.ComponentModel.Description("連線中")]
        Connecting = 0,
        [System.ComponentModel.Description("已連線")]
        Connected = 1,
        [System.ComponentModel.Description("連線失敗")]
        ConnectionFailed = 2,
        Disconnected = 3,
        [System.ComponentModel.Description("登入成功")]
        LoginSucceed = 4,
        [System.ComponentModel.Description("登入失敗")]
        LoginFailed = 5
    }

    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void Test_ToFriendlyString()
        {
            KindOfBrokerStatus.Connecting.ToFriendlyString().Should().Be("連線中");
            KindOfBrokerStatus.Disconnected.ToFriendlyString().Should().Be("Disconnected");
            KindOfBrokerStatus.Connecting.ToFriendlyString().Should().Be("連線中");
            KindOfBrokerStatus.Connected.ToFriendlyString().Should().Be("已連線");
        }
    }
}
