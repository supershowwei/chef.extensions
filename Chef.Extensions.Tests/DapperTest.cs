using System;
using System.Collections.Generic;
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
        public void Test_ToSearchCondition_use_Equals()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id.Equals(1);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[Id] = {=Id_0}");
            parameters["Id_0"].Should().Be(1);
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
        public void Test_ToSearchCondition_support_Equals()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id.Equals(1) && x.FirstName.Equals("GoodJob");

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
        public void Test_ToSearchCondition_use_String_Contains()
        {
            var keyword = "777";

            Expression<Func<Member, bool>> predicate = x => x.LastName.Contains("888") || x.FirstName.Contains(keyword);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([last_name] LIKE '%' + @LastName_0 + '%') OR ([first_name] LIKE '%' + @FirstName_0 + '%')");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("777");
            parameters["LastName_0"].Should().Be("888");
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_String_StartsWith()
        {
            var keyword = "666";

            Expression<Func<Member, bool>> predicate = x => x.LastName.StartsWith("777") || x.FirstName.StartsWith(keyword);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([last_name] LIKE @LastName_0 + '%') OR ([first_name] LIKE @FirstName_0 + '%')");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("666");
            parameters["LastName_0"].Should().Be("777");
        }

        [TestMethod]
        public void Test_ToSearchCondition_use_String_EndsWith()
        {
            var keyword = "555";

            Expression<Func<Member, bool>> predicate = x => x.LastName.EndsWith("666") || x.FirstName.EndsWith(keyword);

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("([last_name] LIKE '%' + @LastName_0) OR ([first_name] LIKE '%' + @FirstName_0)");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("555");
            parameters["LastName_0"].Should().Be("666");
        }

        [TestMethod]
        public void Test_ToSearchCondition_Multiply()
        {
            var keyword = "666";

            var predicates = new List<Expression<Func<Member, bool>>>
                             {
                                 x => x.LastName.EndsWith("777") || x.FirstName.EndsWith(keyword),
                                 x => x.LastName.StartsWith("777") || x.FirstName.StartsWith(keyword)
                             };

            var searchCondition = string.Empty;
            var parameters = new Dictionary<string, object>();

            foreach (var predicate in predicates)
            {
                if (!string.IsNullOrEmpty(searchCondition)) searchCondition += ", ";

                searchCondition += predicate.ToSearchCondition(parameters);
            }

            searchCondition.Should().Be("([last_name] LIKE '%' + @LastName_0) OR ([first_name] LIKE '%' + @FirstName_0), ([last_name] LIKE @LastName_1 + '%') OR ([first_name] LIKE @FirstName_1 + '%')");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("666");
            parameters["LastName_0"].Should().Be("777");
            ((DbString)parameters["FirstName_1"]).Value.Should().Be("666");
            parameters["LastName_1"].Should().Be("777");
        }

        [TestMethod]
        public void Test_ToSearchCondition_Multiply_with_Values()
        {
            var values = new List<Member>
                         {
                             new Member { LastName = "777", FirstName = "888" }, new Member { LastName = "888", FirstName = "999" }
                         };

            var predicates = values
                .Select(v => (Expression<Func<Member, bool>>)(x => x.LastName == v.LastName && x.FirstName == v.FirstName))
                .ToList();

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicates.Aggregate(
                string.Empty,
                (accu, next) =>
                    {
                        if (!string.IsNullOrEmpty(accu)) accu += ", ";

                        return accu + next.ToSearchCondition(parameters);
                    });

            searchCondition.Should().Be("([last_name] = @LastName_0) AND ([first_name] = @FirstName_0), ([last_name] = @LastName_1) AND ([first_name] = @FirstName_1)");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("888");
            parameters["LastName_0"].Should().Be("777");
            ((DbString)parameters["FirstName_1"]).Value.Should().Be("999");
            parameters["LastName_1"].Should().Be("888");
        }

        [TestMethod]
        public void Test_ToSearchCondition_for_Multiply_Update()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id == 0 && x.FirstName == default(string) && x.LastName.Equals(default(string));

            var searchCondition = predicate.ToSearchCondition();

            searchCondition.Should().Be("(([Id] = {=Id}) AND ([first_name] = @FirstName)) AND ([last_name] = @LastName)");
        }

        [TestMethod]
        public void Test_ToSearchCondition_for_Multiply_Update_use_Contains_will_Throw_NullReferenceException()
        {
            Expression<Func<Member, bool>> predicate = x =>
                new[] { 1, 2, 3 }.Contains(x.Id) && x.FirstName == default(string) && x.LastName.Equals(default(string));

            predicate.Invoking(p => p.ToSearchCondition())
                .Should()
                .Throw<NullReferenceException>()
                .WithMessage("'parameters' can not be null.");
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

        [TestMethod]
        public void Test_ToSetStatements_for_Multiply()
        {
            Expression<Func<Member>> setters = () =>
                new Member { Id = default(int), FirstName = default(string), LastName = default(string), Age = default(int) };

            var setStatements = setters.ToSetStatements("kkk");

            setStatements.Should().Be("kkk.[Id] = {=Id}, kkk.[first_name] = @FirstName, kkk.[last_name] = @LastName, kkk.[Age] = {=Age}");
        }

        [TestMethod]
        public void Test_ToColumnList_Simple()
        {
            Expression<Func<Member>> setters = () => new Member { Id = 123, FirstName = "abab", LastName = "baba" };

            var columnList = setters.ToColumnList(out var valueList, out var parameters);

            columnList.Should().Be("[Id], [first_name], [last_name]");
            valueList.Should().Be("{=Id_0}, @FirstName_0, @LastName_0");
            parameters["Id_0"].Should().Be(123);
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("abab");
            parameters["LastName_0"].Should().Be("baba");
        }

        [TestMethod]
        public void Test_ToColumnList_without_Parameters()
        {
            Expression<Func<Member>> setters = () => new Member { Id = 123, FirstName = "abab", LastName = "baba" };

            var columnList = setters.ToColumnList(out var valueList);

            columnList.Should().Be("[Id], [first_name], [last_name]");
            valueList.Should().Be("{=Id}, @FirstName, @LastName");
        }

        [TestMethod]
        public void Test_ToAscendingOrder_Simple()
        {
            Expression<Func<Member, object>> orderBy = x => x.Id;
            Expression<Func<Member, object>> thenBy = x => x.FirstName;

            var orderExpression = orderBy.ToOrderAscending("m") + ", " + thenBy.ToOrderAscending("m");

            Assert.AreEqual("m.[Id] ASC, m.[first_name] ASC", orderExpression);
        }

        [TestMethod]
        public void Test_ToDescendingOrder_Simple()
        {
            Expression<Func<Member, object>> orderBy = x => x.Id;
            Expression<Func<Member, object>> thenBy = x => x.Seniority;

            var orderExpression = orderBy.ToOrderDescending("m") + ", " + thenBy.ToOrderDescending("m");

            Assert.AreEqual("m.[Id] DESC, m.[Seniority] DESC", orderExpression);
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

        public double Seniority { get; set; }

        public int Age { get; set; }
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
