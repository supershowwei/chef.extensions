using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Chef.Extensions.DbAccess;
using Chef.Extensions.DbAccess.SqlServer.Extensions;
using Dapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    public class ExpressionReplacer : ExpressionVisitor
    {
        private readonly IDictionary<string, ParameterExpression> parameterMap = new Dictionary<string, ParameterExpression>();

        public T Replace<T>(T oldExpr, T newExpr)
            where T : LambdaExpression
        {
            for (var i = 0; i < oldExpr.Parameters.Count; i++)
            {
                this.parameterMap.Add(oldExpr.Parameters[i].Name, newExpr.Parameters[i]);
            }

            return this.Visit(oldExpr) as T;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this.parameterMap[node.Name];
        }
    }

    [TestClass]
    public class SqlServerDataAccess_StatementExtension_Test
    {
        [TestMethod]
        public void Test_ToSearchCondition_use_String_CompareTo()
        {
            var firstName = "abc";

            Expression<Func<Member, bool>> predicate = x => x.FirstName.CompareTo(firstName) > 0 && x.LastName.CompareTo("cba") <= 0;

            var searchCondition = predicate.ToSearchCondition("m", out var parameters);

            searchCondition.Should().Be("([m].[first_name] > @FirstName_0) AND ([m].[last_name] <= @LastName_0)");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be(firstName);
            parameters["LastName_0"].Should().Be("cba");
        }

        [TestMethod]
        public void Test_ToSearchCondition_in_Join_Two_Tables()
        {
            Expression<Func<Member, Video, bool>> predicate = (x, y) => x.Id < 1 && y.Id == 2 && y.PackageId == 1;

            var searchCondition = predicate.ToSearchCondition(new[] { "m", "v" }, out var parameters);

            searchCondition.Should().Be("(([m].[Id] < {=Id_0}) AND ([v].[ID] = {=Id_1})) AND ([v].[PackageID] = {=PackageId_0})");
            parameters["Id_0"].Should().Be(1);
            parameters["Id_1"].Should().Be(2);
            parameters["PackageId_0"].Should().Be(1);
        }

        [TestMethod]
        public void Test_ToSearchCondition_in_Join_Three_Tables()
        {
            Expression<Func<Member, Video, Member, bool>> predicate = (x, y, z) => x.Id < 1 && y.Id == 2 && y.PackageId == 1 && z.Id == 2;

            var searchCondition = predicate.ToSearchCondition(new[] { "m1", "v", "m2" }, out var parameters);

            searchCondition.Should().Be("((([m1].[Id] < {=Id_0}) AND ([v].[ID] = {=Id_1})) AND ([v].[PackageID] = {=PackageId_0})) AND ([m2].[Id] = {=Id_2})");
            parameters["Id_0"].Should().Be(1);
            parameters["Id_1"].Should().Be(2);
            parameters["PackageId_0"].Should().Be(1);
            parameters["Id_1"].Should().Be(2);
        }

        [TestMethod]
        public void Test_ToSearchCondition_Simple()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id < 1;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[Id] < {=Id_0}");
            parameters["Id_0"].Should().Be(1);
        }

        [TestMethod]
        public void Test_ToSearchCondition_Square_Brackets_around_Alias()
        {
            Expression<Func<AbcStu, bool>> predicate = x => x.Id == "abc";

            var searchCondition = predicate.ToSearchCondition("as", out var parameters);

            searchCondition.Should().Be("[as].[Id] = @Id_0");
            parameters["Id_0"].Should().Be("abc");
        }

        [TestMethod]
        public void Test_ToSearchCondition_Has_NotMapped_Column_will_Throw_ArgumentException()
        {
            Expression<Func<Member, bool>> predicate = x => x.IgnoredColumn == "testabc";

            predicate.Invoking(x => x.ToSearchCondition(out var parameters))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Member can not applied [NotMapped].");
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

            searchCondition.Should().Be("[first_name] IS NULL");
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
        public void Test_ToSearchCondition_Merge_Multiple_Conditions()
        {
            Expression<Func<Member, bool>> predicate1 = x => x.Id == 1;
            Expression<Func<Member, bool>> predicate2 = y => y.FirstName == "GoodJob";
            
            var replacer = new ExpressionReplacer();

            predicate2 = replacer.Replace(predicate2, predicate1);

            var predicate = predicate1.Update(Expression.AndAlso(predicate1.Body, predicate2.Body), predicate1.Parameters);

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

            searchCondition.Should().Be("([abc].[last_name] = @LastName_0) OR (([abc].[Id] > {=Id_0}) AND ([abc].[first_name] = @FirstName_0))");
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
        public void Test_ToSearchCondition_for_Multiply_Update_use_CSharp_71_Syntax_Default()
        {
            Expression<Func<Member, bool>> predicate = x => x.Id == 0 && x.FirstName == default && x.LastName.Equals(default);

            var searchCondition = predicate.ToSearchCondition();

            searchCondition.Should().Be("(([Id] = {=Id}) AND ([first_name] = @FirstName)) AND ([last_name] = @LastName)");
        }

        [TestMethod]
        public void Test_ToSearchCondition_for_Multiply_Update_use_Contains_will_Throw_ArgumentException()
        {
            Expression<Func<Member, bool>> predicate = x =>
                new[] { 1, 2, 3 }.Contains(x.Id) && x.FirstName == default(string) && x.LastName.Equals(default(string));

            predicate.Invoking(p => p.ToSearchCondition())
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("'parameters' can not be null.");
        }

        [TestMethod]
        public void Test_ToSearchCondition_for_Nullable_Parameter()
        {
            Expression<Func<OrderViewItem, bool>> predicate = x => x.PackageId == 48;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[PackageId] = {=PackageId_0}");
            parameters["PackageId_0"].Should().Be(48);
        }

        [TestMethod]
        public void Test_ToSearchCondition_for_Nullable_Parameter_and_Value()
        {
            var storedValue = new StoredValue(new OrderViewItem { PackageId = 48 });

            Expression<Func<Video, bool>> predicate = x => x.PackageId == storedValue.OrderViewItem.PackageId;

            var searchCondition = predicate.ToSearchCondition(out var parameters);

            searchCondition.Should().Be("[PackageID] = {=PackageId_0}");
            parameters["PackageId_0"].Should().Be(48);
        }

        [TestMethod]
        public void Test_ToInnerJoin_Simple()
        {
            Expression<Func<Member, Video, bool>> innerJoinPredicate = (x, y) => x.Id == y.Id;

            var innerJoinSearchCodition = innerJoinPredicate.ToInnerJoin<Video>(new[] { "m", "v" }, null, null);

            innerJoinSearchCodition.Should().Be("INNER JOIN Video [v] WITH (NOLOCK) ON [m].[Id] = [v].[ID]");
        }

        [TestMethod]
        public void Test_ToInnerJoin_with_Database_and_Schema()
        {
            Expression<Func<Member, Video, bool>> innerJoinPredicate = (x, y) => x.Id == y.Id;

            var innerJoinSearchCodition = innerJoinPredicate.ToInnerJoin<Video>(new[] { "m", "v" }, "MyDB", "dbo");

            innerJoinSearchCodition.Should().Be("INNER JOIN [MyDB].[dbo].[Video] [v] WITH (NOLOCK) ON [m].[Id] = [v].[ID]");
        }

        [TestMethod]
        public void Test_ToInnerJoin_with_Multiple_Coditions()
        {
            Expression<Func<Member, Video, bool>> innerJoinPredicate = (x, y) => x.Id == y.Id && x.Id == y.PackageId;

            var innerJoinSearchCodition = innerJoinPredicate.ToInnerJoin<Video>(new[] { "m", "v" }, null, null);

            innerJoinSearchCodition.Should().Be("INNER JOIN Video [v] WITH (NOLOCK) ON ([m].[Id] = [v].[ID]) AND ([m].[Id] = [v].[PackageID])");
        }

        [TestMethod]
        public void Test_ToLeftJoin_Simple()
        {
            Expression<Func<Member, Video, bool>> leftJoinPredicate = (x, y) => x.Id == y.Id;

            var leftJoinSearchCodition = leftJoinPredicate.ToLeftJoin<Video>(new[] { "m", "v" }, null, null);

            leftJoinSearchCodition.Should().Be("LEFT JOIN Video [v] WITH (NOLOCK) ON [m].[Id] = [v].[ID]");
        }

        [TestMethod]
        public void Test_ToLeftJoin_with_Multiple_Coditions()
        {
            Expression<Func<Member, Video, bool>> leftJoinPredicate = (x, y) => x.Id == y.Id && x.Id == y.PackageId;

            var leftJoinSearchCodition = leftJoinPredicate.ToLeftJoin<Video>(new[] { "m", "v" }, null, null);

            leftJoinSearchCodition.Should().Be("LEFT JOIN Video [v] WITH (NOLOCK) ON ([m].[Id] = [v].[ID]) AND ([m].[Id] = [v].[PackageID])");
        }

        [TestMethod]
        public void Test_ToGroupingColumns_with_Three_Tables()
        {
            Expression<Func<Member, Department, Member, object>> columnExpr = (x, y, z) => new { x.Id, y.Name };

            var columns = columnExpr.ToGroupingColumns(new[] { "m1", "d1", "m2" });

            columns.Should().Be("[m1].[Id], [d1].[Name]");
        }

        [TestMethod]
        public void Test_ToGroupingSelectList_with_Three_Tables()
        {
            Expression<Func<Grouping<Member, Department, Member>, Member>> groupingSelector = g => new Member
                                                                                                   {
                                                                                                       Id = g.Select((a, b, c) => a.Id),
                                                                                                       FirstName = g.Select((d, e, f) => d.LastName),
                                                                                                       SubordinateCount = g.Count(),
                                                                                                       MaxSubordinateId = g.Max((x, y, z) => z.Id)
                                                                                                   };

            var selector = groupingSelector.ToGroupingSelectList(new[] { "m1", "d1", "m2" });

            selector.Should().Be("[m1].[Id] AS [Id], [m1].[last_name] AS [FirstName], COUNT(*) AS [SubordinateCount], MAX([m2].[Id]) AS [MaxSubordinateId]");
        }

        [TestMethod]
        public void Test_ToSelectList_in_Join_Two_Tables()
        {
            Expression<Func<Member, Video, object>> selector = (x, y) => new { x.Id, VideoId = y.Id, x.FirstName, y.PackageId };

            var selectList = selector.ToJoinSelectList(new[] { "m", "v" }, out var splitOn);

            selectList.Should().Be("[m].[Id], [m].[first_name] AS [FirstName], [v].[ID] AS [Id], [v].[PackageID] AS [PackageId]");
            splitOn.Should().Be("Id");
        }

        [TestMethod]
        public void Test_ToSelectList_in_Join_Three_Tables()
        {
            Expression<Func<Member, Video, Member, object>> selector = (x, y, z) => new { x.Id, VideoId = y.Id, x.FirstName, y.PackageId, ZId = z.Id, ZFirstName = z.FirstName };

            var selectList = selector.ToJoinSelectList(new[] { "m1", "v", "m2" }, out var splitOn);

            selectList.Should().Be("[m1].[Id], [m1].[first_name] AS [FirstName], [v].[ID] AS [Id], [v].[PackageID] AS [PackageId], [m2].[Id], [m2].[first_name] AS [FirstName]");
            splitOn.Should().Be("Id,Id");
        }

        [TestMethod]
        public void Test_ToSelectList_Simple()
        {
            Expression<Func<Member, object>> select = x => new { x.Id, x.FirstName, x.LastName };

            var selectList = select.ToSelectList();

            selectList.Should().Be("[Id], [first_name] AS [FirstName], [last_name] AS [LastName]");
        }

        [TestMethod]
        public void Test_ToSelectList_Has_NotMapped_Column_will_be_not_Selected()
        {
            Expression<Func<Member, object>> select = x => new { x.Id, x.FirstName, x.LastName, x.IgnoredColumn };

            var selectList = select.ToSelectList();

            selectList.Should().Be("[Id], [first_name] AS [FirstName], [last_name] AS [LastName]");
        }

        [TestMethod]
        public void Test_ToSelectList_using_PropertyInfo_Array()
        {
            var requiredColumns = typeof(Member).GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute))).ToArray();

            var columnList = requiredColumns.ToSelectList();

            columnList.Should().Be("[Id], [first_name] AS [FirstName], [last_name] AS [LastName]");
        }

        [TestMethod]
        public void Test_ToSelectList_with_Alias()
        {
            Expression<Func<Member, object>> select = x => new { x.Id, x.FirstName, x.LastName };

            var selectList = select.ToSelectList("att");

            selectList.Should().Be("[att].[Id], [att].[first_name] AS [FirstName], [att].[last_name] AS [LastName]");
        }

        [TestMethod]
        public void Test_ToSelectList_for_Nullable_Parameter_and_Value()
        {
            Expression<Func<Video, object>> select = x => new { x.Id, x.PackageId };

            var selectList = select.ToSelectList();

            selectList.Should().Be("[ID] AS [Id], [PackageID] AS [PackageId]");
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
        public void Test_ToSetStatements_Has_NotMapped_Column_will_Throw_ArgumentException()
        {
            Expression<Func<Member>> setters = () => new Member { FirstName = "abab", LastName = "baba", IgnoredColumn = "testabc" };

            setters.Invoking(x => x.ToSetStatements(out var parameters))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Member can not applied [NotMapped].");
        }

        [TestMethod]
        public void Test_ToSetStatements_with_Alias()
        {
            Expression<Func<Member>> setters = () => new Member { FirstName = "abab", LastName = "baba" };

            var setStatements = setters.ToSetStatements("kkk", out var parameters);

            setStatements.Should().Be("[kkk].[first_name] = @FirstName_0, [kkk].[last_name] = @LastName_0");
            ((DbString)parameters["FirstName_0"]).Value.Should().Be("abab");
            parameters["LastName_0"].Should().Be("baba");
        }

        [TestMethod]
        public void Test_ToSetStatements_for_Multiply()
        {
            Expression<Func<Member>> setters = () =>
                new Member { Id = default(int), FirstName = default(string), LastName = default(string), Age = default(int) };

            var setStatements = setters.ToSetStatements("kkk");

            setStatements.Should().Be("[kkk].[Id] = {=Id}, [kkk].[first_name] = @FirstName, [kkk].[last_name] = @LastName, [kkk].[Age] = {=Age}");
        }

        [TestMethod]
        public void Test_ToSetStatements_for_Nullable_Parameter()
        {
            Expression<Func<Video>> setters = () => new Video { Id = 1, PackageId = 2 };

            var setStatements = setters.ToSetStatements();

            setStatements.Should().Be("[ID] = {=Id}, [PackageID] = {=PackageId}");
        }

        [TestMethod]
        public void Test_ToSetStatements_for_Nullable_Parameter_and_Value()
        {
            Expression<Func<Video>> setters = () => new Video { Id = 1, PackageId = 2 };

            var setStatements = setters.ToSetStatements(out var parameters);

            setStatements.Should().Be("[ID] = {=Id_0}, [PackageID] = {=PackageId_0}");
            parameters["Id_0"].Should().Be(1);
            parameters["PackageId_0"].Should().Be(2);
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
        public void Test_ToColumnList_Has_NotMapped_Column_will_Throw_ArgumentException()
        {
            Expression<Func<Member>> setters = () =>
                new Member { Id = 123, FirstName = "abab", LastName = "baba", IgnoredColumn = "testabc" };

            setters.Invoking(x => x.ToColumnList(out var valueList, out var parameters))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Member can not applied [NotMapped].");
        }

        [TestMethod]
        public void Test_ToColumnList_using_PropertyInfo_Array()
        {
            var requiredColumns = typeof(Member).GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute))).ToArray();

            var columnList = requiredColumns.ToColumnList(out var valueList);

            columnList.Should().Be("[Id], [first_name], [last_name]");
            valueList.Should().Be("{=Id}, @FirstName, @LastName");
        }

        [TestMethod]
        public void Test_ToColumnList_using_Null_PropertyInfo_Array_will_Throw_ArgumentException()
        {
            PropertyInfo[] requiredColumns = null;

            requiredColumns.Invoking(x => x.ToColumnList(out var valueList))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("'me' can not be null or empty.");
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
        public void Test_ToColumnList_for_Nullable_Parameter()
        {
            Expression<Func<Video>> setters = () => new Video { Id = 123, PackageId = 321 };

            var columnList = setters.ToColumnList(out var valueList);

            columnList.Should().Be("[ID], [PackageID]");
            valueList.Should().Be("{=Id}, {=PackageId}");
        }

        [TestMethod]
        public void Test_ToColumnList_for_Nullable_Parameter_and_Value()
        {
            Expression<Func<Video>> setters = () => new Video { Id = 123, PackageId = 321 };

            var columnList = setters.ToColumnList(out var valueList, out var parameters);

            columnList.Should().Be("[ID], [PackageID]");
            valueList.Should().Be("{=Id_0}, {=PackageId_0}");
            parameters["Id_0"].Should().Be(123);
            parameters["PackageId_0"].Should().Be(321);
        }

        [TestMethod]
        public void Test_ToAscendingOrder_in_Join_Two_Tables()
        {
            Expression<Func<Member, Video, object>> orderBy = (x, y) => x.Id;
            Expression<Func<Member, Video, object>> thenBy = (x, y) => y.Id;

            var orderExpression = orderBy.ToOrderAscending(new[] { "m", "v" }) + ", " + thenBy.ToOrderAscending(new[] { "m", "v" });

            orderExpression.Should().Be("[m].[Id] ASC, [v].[ID] ASC");
        }

        [TestMethod]
        public void Test_ToAscendingOrder_Simple()
        {
            Expression<Func<Member, object>> orderBy = x => x.Id;
            Expression<Func<Member, object>> thenBy = x => x.FirstName;

            var orderExpression = orderBy.ToOrderAscending("m") + ", " + thenBy.ToOrderAscending("m");

            orderExpression.Should().Be("[m].[Id] ASC, [m].[first_name] ASC");
        }

        [TestMethod]
        public void Test_ToAscendingOrder_Has_NotMapped_Column_will_Throw_ArgumentException()
        {
            Expression<Func<Member, object>> orderBy = x => x.IgnoredColumn;

            orderBy.Invoking(x => x.ToOrderAscending("m"))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Member can not applied [NotMapped].");
        }

        [TestMethod]
        public void Test_ToDescendingOrder_in_Join_Two_Tables()
        {
            Expression<Func<Member, Video, object>> orderBy = (x, y) => x.Id;
            Expression<Func<Member, Video, object>> thenBy = (x, y) => y.Id;

            var orderExpression = orderBy.ToOrderDescending(new[] { "m", "v" }) + ", " + thenBy.ToOrderDescending(new[] { "m", "v" });

            orderExpression.Should().Be("[m].[Id] DESC, [v].[ID] DESC");
        }

        [TestMethod]
        public void Test_ToDescendingOrder_in_Join_Three_Tables()
        {
            Expression<Func<Member, Video, Member, object>> orderBy = (x, y, z) => x.Id;
            Expression<Func<Member, Video, Member, object>> thenBy = (x, y, z) => y.Id;
            Expression<Func<Member, Video, Member, object>> thenThenBy = (x, y, z) => z.Id;

            var orderExpression = orderBy.ToOrderDescending(new[] { "m1", "v", "m2" }) + ", "
                                  + thenBy.ToOrderDescending(new[] { "m1", "v", "m2" }) + ", "
                                  + thenThenBy.ToOrderDescending(new[] { "m1", "v", "m2" });

            orderExpression.Should().Be("[m1].[Id] DESC, [v].[ID] DESC, [m2].[Id] DESC");
        }

        [TestMethod]
        public void Test_ToDescendingOrder_Simple()
        {
            Expression<Func<Member, object>> orderBy = x => x.Id;
            Expression<Func<Member, object>> thenBy = x => x.Seniority;

            var orderExpression = orderBy.ToOrderDescending("m") + ", " + thenBy.ToOrderDescending("m");

            orderExpression.Should().Be("[m].[Id] DESC, [m].[Seniority] DESC");
        }

        [TestMethod]
        public void Test_ToDescendingOrder_Has_NotMapped_Column_will_Throw_ArgumentException()
        {
            Expression<Func<Member, object>> orderBy = x => x.IgnoredColumn;

            orderBy.Invoking(x => x.ToOrderDescending("m"))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Member can not applied [NotMapped].");
        }
    }

    internal class Member
    {
        [Required]
        public int Id { get; set; }

        [Column("first_name", TypeName = "varchar")]
        [StringLength(20)]
        [Required]
        public string FirstName { get; set; }

        [Column("last_name")]
        [Required]
        public string LastName { get; set; }

        public double Seniority { get; set; }

        public int Age { get; set; }

        public int SubordinateCount { get; set; }

        public int MaxSubordinateId { get; set; }

        public int SubordinateId { get; set; }

        public Member Subordinate { get; set; }

        [NotMapped]
        public string IgnoredColumn { get; set; }
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

    internal class Video
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("PackageID")]
        public int PackageId { get; set; }
    }

    internal class StoredValue
    {
        public StoredValue(OrderViewItem orderViewItem)
        {
            this.OrderViewItem = orderViewItem;
        }

        public OrderViewItem OrderViewItem { get; }
    }

    internal class OrderViewItem
    {
        public int? PackageId { get; set; }
    }

    internal class AbcStu
    {
        public string Id { get; set; }
    }
}
