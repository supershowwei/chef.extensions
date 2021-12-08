﻿using System;
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
        public void Test_To_Convert_Member_To_User()
        {
            var member = new Member { Id = 99 };
            
            var user = member.To<User>();

            user.Id.Should().Be(99);
        }

        [TestMethod]
        public void Test_To_Convert_Member_To_User_Again()
        {
            var member = new Member { Id = 99 };
            
            var user = member.To<User>();

            user.Id.Should().Be(99);
        }

        [TestMethod]
        public void Test_To_Convert_Member_To_Existed_User()
        {
            var member = new Member { Id = 99 };
            var user = new User { Id = 88, Name = "abc" };

            var mappedUser = member.To<User>(user);

            mappedUser.Should().BeSameAs(user);
            mappedUser.Id.Should().Be(88);
            mappedUser.Name.Should().Be("abc");
        }

        [TestMethod]
        public void Test_To_Convert_Member_To_User_use_Custom_Func()
        {
            var member = new Member { Id = 99 };

            var user = member.To(
                obj =>
                    {
                        var m = obj;

                        return new User { Id = m.Id };
                    });

            user.Id.Should().Be(99);
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

        [TestMethod]
        public void Test_Is_And_Or()
        {
            var member = new Member { Id = 1 };

            ((bool)member.Is(x => x.Id == 1)).Should().BeTrue();
            ((bool)member.Is(x => x.Id == 1).And(x => x.Id > 0)).Should().BeTrue();
            ((bool)member.Is(x => x.Id == 1).Or(x => x.Id != 0)).Should().BeTrue();
            ((bool)member.Is(x => x.Id == 1).And(false)).Should().BeFalse();
            ((bool)member.Is(x => x.Id == 1).Or(false)).Should().BeTrue();
        }
    }

    internal class Member
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public double Seniority { get; set; }

        public int Age { get; set; }

        public int SubordinateCount { get; set; }

        public int MaxSubordinateId { get; set; }

        public int SubordinateId { get; set; }

        public Member Subordinate { get; set; }

        public string IgnoredColumn { get; set; }
    }

    internal class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public int SubordinateCount { get; set; }

        public int MaxSubordinateId { get; set; }

        public int SubordinateId { get; set; }

        public User Subordinate { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public int ManagerId { get; set; }

        public User Manager { get; set; }

        public User Self { get; set; }
    }

    internal class Department
    {
        public int DepId { get; set; }

        public string Name { get; set; }
    }
}