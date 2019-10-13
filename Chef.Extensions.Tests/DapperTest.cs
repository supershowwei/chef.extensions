using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Chef.Extensions.Dapper;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class DapperTest
    {
        [TestMethod]
        public void Test_ToSearchCondition_Simple()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id < 1;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[Id] < {=Id_0}");
            parameters["Id_0"].Should().Be(1);
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_DbString()
        {
            Expression<Func<Member, bool>> predicate = x => x.FirstName == "GoodJob";

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[first_name] = @FirstName_0");
            parameters["FirstName_0"].GetType().Should().Be<DbString>();
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("GoodJob");
            ((DbString)parameters["FirstName_0"]).Length.Should().Be(20);
            ((DbString)parameters["FirstName_0"]).IsAnsi.Should().BeTrue();
            ((DbString)parameters["FirstName_0"]).IsFixedLength.Should().BeFalse();
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_IntMax()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id == int.MaxValue;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[Id] = {=Id_0}");
            parameters["Id_0"].Should().Be(int.MaxValue);
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_EmptyString()
        {
            Expression<Func<Member, bool>> predicate = x => x.FirstName == string.Empty;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[first_name] = @FirstName_0");
            ((DbString)parameters["FirstName_0"]).Value.Should().BeEmpty();
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_Null()
        {
            Expression<Func<Member, bool>> predicate = x => x.FirstName == null;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[first_name] = @FirstName_0");
            ((DbString)parameters["FirstName_0"]).Value.Should().BeNull();
        }

        [TestMethod]
        public void Test_ToSearchCondition_And()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id == 1 && x.FirstName == "GoodJob";

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([Id] = {=Id_0}) AND ([first_name] = @FirstName_0)");
            parameters["Id_0"].Should().Be(1);
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("GoodJob");
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_Object_Property()
        {
            var queryParameter = new QueryParameter { Id = 1, Name = "444" };

            Expression<Func<Member, bool>> predicate = x => x.Id <= queryParameter.Id && x.FirstName == queryParameter.Name;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([Id] <= {=Id_0}) AND ([first_name] = @FirstName_0)");
            parameters["Id_0"].Should().Be(1);
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("444");
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_Complex_Object_Property()
        {
            var queryParameter = new QueryParameter { Id = 2, Name = "555", Address = new Address { Value = "888" } };

            Expression<Func<Member, bool>> predicate = x =>
                x.Id <= queryParameter.Id && x.FirstName == queryParameter.Name || x.LastName == queryParameter.Address.Value;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("(([Id] <= {=Id_0}) AND ([first_name] = @FirstName_0)) OR ([last_name] = @LastName_0)");
            parameters["Id_0"].Should().Be(2);
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("555");
            parameters["LastName_0"].Should().Be("888");
        }

        [TestMethod]
        public void Test_ToSearchCondition_And_Or()
        {
            Expression<Func<Member, bool>> predicate = x => x.LastName == "JobGood" || (x.Id > 1 && x.FirstName == "GoodJob");

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([last_name] = @LastName_0) OR (([Id] > {=Id_0}) AND ([first_name] = @FirstName_0))");
            parameters["Id_0"].Should().Be(1);
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("GoodJob");
            parameters["LastName_0"].Should().Be("JobGood");
        }

        [TestMethod]
        public void Test_ToSearchCondition_And_Or_with_Alias()
        {
            Expression<Func<Member, bool>> predicate = x => x.LastName == "JobGood" || (x.Id > 1 && x.FirstName == "GoodJob");

            var searchCondition = predicate.ToSearchCondition("abc", out var parameters);

            searchCondition.Should().Be("(abc.[last_name] = @LastName_0) OR ((abc.[Id] > {=Id_0}) AND (abc.[first_name] = @FirstName_0))");
            parameters["Id_0"].Should().Be(1);
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("GoodJob");
            parameters["LastName_0"].Should().Be("JobGood");
        }

        [TestMethod]
        public void Test_ToSearchCondition_Or_with_Same_Parameter()
        {
            Expression<Func<Member, bool>> predicate = x => x.FirstName == "111" || x.FirstName == "222";

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([first_name] = @FirstName_0) OR ([first_name] = @FirstName_1)");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("111");
            ((DbString)parameters["FirstName_1"]).Value.Should().Be("222");
        }

        [TestMethod]
        public void Test_ToSearchCondition_Multiple_Predicate()
        {
            Expression<Func<Member, bool>> predicate1 = x => x.LastName == "GoodJob";
            Expression<Func<Member, bool>> predicate2 = x => x.LastName == "JobGood";

            var searchCondition1 = predicate1.ToSearchCondition(out var parameters);
            var searchCondition2 = predicate2.ToSearchCondition(parameters);

            searchCondition1.Should().Be("[last_name] = @LastName_0");
            searchCondition2.Should().Be("[last_name] = @LastName_1");
            parameters["LastName_0"].Should().Be("GoodJob");
            parameters["LastName_1"].Should().Be("JobGood");
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_Contains_with_Initializer()
        {
            Expression<Func<Member, bool>> predicate = x => new[] { "1", "2", "3" }.Contains(x.FirstName);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[first_name] = @FirstName_0 OR [first_name] = @FirstName_1 OR [first_name] = @FirstName_2");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("1");
            ((DbString)parameters["FirstName_1"]).Value.Should().Be("2");
            ((DbString)parameters["FirstName_2"]).Value.Should().Be("3");
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_Contains_with_Variable()
        {
            var arr = new[] { "1", "2", "3" };

            Expression<Func<Member, bool>> predicate = x => arr.Contains(x.FirstName);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[first_name] = @FirstName_0 OR [first_name] = @FirstName_1 OR [first_name] = @FirstName_2");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("1");
            ((DbString)parameters["FirstName_1"]).Value.Should().Be("2");
            ((DbString)parameters["FirstName_2"]).Value.Should().Be("3");
        }

        [TestMethod]
        public void Test_ToSearchCondition_And_with_Contains()
        {
            var arr = new[] { "1", "2", "3" };

            Expression<Func<Member, bool>> predicate = x => arr.Contains(x.FirstName) && new[] { 1, 2, 3 }.Contains(x.Id);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([first_name] = @FirstName_0 OR [first_name] = @FirstName_1 OR [first_name] = @FirstName_2) AND ([Id] = {=Id_0} OR [Id] = {=Id_1} OR [Id] = {=Id_2})");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("1");
            ((DbString)parameters["FirstName_1"]).Value.Should().Be("2");
            ((DbString)parameters["FirstName_2"]).Value.Should().Be("3");
            parameters["Id_0"].Should().Be(1);
            parameters["Id_1"].Should().Be(2);
            parameters["Id_2"].Should().Be(3);
        }

        [TestMethod]
        public void Test_ToSearchCondition_Or_Contains()
        {
            Expression<Func<Member, bool>> predicate = x => x.LastName == "999" || new[] { 1, 2, 3 }.Contains(x.Id);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([last_name] = @LastName_0) OR ([Id] = {=Id_0} OR [Id] = {=Id_1} OR [Id] = {=Id_2})");
            parameters["LastName_0"].Should().Be("999");
            parameters["Id_0"].Should().Be(1);
            parameters["Id_1"].Should().Be(2);
            parameters["Id_2"].Should().Be(3);
        }

        [TestMethod]
        public void Test_ToSelectList_Simple()
        {
            Expression<Func<Member, object>> select = x => new { x.Id, x.FirstName, x.LastName };

            var selectList = select.ToSelectList();

            selectList.Should().Be("[Id], [first_name] AS [FirstName], [last_name] AS [LastName]");
        }

        [TestMethod]
        public void Test_ToSelectList_with_Alias()
        {
            Expression<Func<Member, object>> select = x => new { x.Id, x.FirstName, x.LastName };

            var selectList = select.ToSelectList("att");

            selectList.Should().Be("att.[Id], att.[first_name] AS [FirstName], att.[last_name] AS [LastName]");
        }

        [TestMethod]
        public void Test_ToSetStatements_Simple()
        {
            Expression<Func<Member>> setters = () => new Member { FirstName = "abab", LastName = "baba" };

            var setStatements = setters.ToSetStatements(out var parameters);

            setStatements.Should().Be("[first_name] = @FirstName_0, [last_name] = @LastName_0");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("abab");
            parameters["LastName_0"].Should().Be("baba");
        }

        [TestMethod]
        public void Test_ToSetStatements_with_Alias()
        {
            Expression<Func<Member>> setters = () => new Member { FirstName = "abab", LastName = "baba" };

            var setStatements = setters.ToSetStatements("kkk", out var parameters);

            setStatements.Should().Be("kkk.[first_name] = @FirstName_0, kkk.[last_name] = @LastName_0");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("abab");
            parameters["LastName_0"].Should().Be("baba");
        }
    }

    internal class Member
    {
        public int Id { get; set; }

        [Column("first_name", TypeName = "varchar")]
        [StringLength(20)]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }
    }

    internal class QueryParameter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Address Address { get; set; }
    }

    internal class Address
    {
        public string Value { get; set; }
    }
}
