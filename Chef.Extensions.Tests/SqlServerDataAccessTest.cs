using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Chef.Extensions.DbAccess;
using Chef.Extensions.DbAccess.Fluent;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class SqlServerDataAccessTest
    {
        [TestMethod]
        public void Test_QueryOne()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = clubDataAccess.QueryOne(x => x.Id == 25);

            club.Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryOneAsync()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25);

            club.Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryOneAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 25, selector: x => new { x.Name });

            club.Id.Should().Be(0);
            club.Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryOneAsync_with_Selector_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.Where(x => x.Id == 25).Select(x => new { x.Name }).QueryOneAsync();

            club.Id.Should().Be(0);
            club.Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Null()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => x.Intro == null).QueryAsync();

            clubs.Count.Should().BeGreaterOrEqualTo(5);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.QueryAsync(x => new[] { 17, 25 }.Contains(x.Id), selector: x => new { x.Name });

            clubs.Count.Should().Be(2);
            clubs[0].Id.Should().Be(0);
            clubs[1].Id.Should().Be(0);
            clubs[0].Name.Should().Be("吳淑娟");
            clubs[1].Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id)).Select(x => new { x.Name }).QueryAsync();

            clubs.Count.Should().Be(2);
            clubs[0].Id.Should().Be(0);
            clubs[1].Id.Should().Be(0);
            clubs[0].Name.Should().Be("吳淑娟");
            clubs[1].Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_has_DateTime_Now()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => x.RunningTime < DateTime.Now).Select(x => new { x.Id, x.Name }).QueryAsync();

            clubs.Count.Should().Be(1);
            clubs[0].Id.Should().Be(39);
            clubs[0].Name.Should().Be("王真希");
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_and_And()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => x.RunningTime < DateTime.Now)
                            .And(y => y.IsActive == false)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_and_Or()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => x.RunningTime > DateTime.Now)
                            .Or(y => y.Name == "王真希")
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(1);
            clubs[0].Id.Should().Be(39);
            clubs[0].Name.Should().Be("王真希");
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_and_OrderByDescending()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id))
                            .OrderByDescending(x => x.Id)
                            .Select(x => new { x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(2);
            clubs[0].Id.Should().Be(0);
            clubs[1].Id.Should().Be(0);
            clubs[0].Name.Should().Be("鄧偉成");
            clubs[1].Name.Should().Be("吳淑娟");
        }

        [TestMethod]
        public async Task Test_QueryAsync_with_Selector_use_QueryObject_and_OrderByDescending_and_Top()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.Where(x => new[] { 17, 25 }.Contains(x.Id))
                            .OrderByDescending(x => x.Id)
                            .Select(x => new { x.Name })
                            .Top(1)
                            .QueryAsync();

            clubs.Count.Should().Be(1);
            clubs[0].Id.Should().Be(0);
            clubs[0].Name.Should().Be("鄧偉成");
        }

        [TestMethod]
        public async Task Test_QueryAllAsync_with_Selector_use_QueryObject_and_OrderByDescending_and_ThenBy_Top()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubs = await clubDataAccess.OrderByDescending(x => x.IsActive)
                            .ThenBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .Top(1)
                            .QueryAsync();

            clubs.Count.Should().Be(1);
            clubs[0].Id.Should().Be(9);
            clubs[0].Name.Should().Be("吳美惠");
        }

        [TestMethod]
        public async Task Test_CountAsync()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubCount = await clubDataAccess.Where(x => x.Intro == "陳").CountAsync();

            clubCount.Should().Be(3);
        }

        [TestMethod]
        public async Task Test_ExistsAsync()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var isExists = await clubDataAccess.Where(x => x.Id > 0).ExistsAsync();

            isExists.Should().BeTrue();

            isExists = await clubDataAccess.Where(x => x.Id < 0).ExistsAsync();

            isExists.Should().BeFalse();
        }

        [TestMethod]
        public async Task Test_UpdateAsync()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "歐陽邦瑋" + suffix;

            await clubDataAccess.UpdateAsync(x => x.Id.Equals(15), () => new Club { Name = clubName });

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 15);

            club.Id.Should().Be(15);
            club.Name.Should().Be("歐陽邦瑋" + suffix);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_set_Null()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 36, selector: x => new { x.Id, x.Intro });

            club.Id.Should().Be(36);
            club.Intro.Should().Be("連");

            await clubDataAccess.UpdateAsync(x => x.Id.Equals(36), () => new Club { Intro = null });

            club = await clubDataAccess.QueryOneAsync(x => x.Id == 36, selector: x => new { x.Id, x.Intro });

            await clubDataAccess.UpdateAsync(x => x.Id.Equals(36), () => new Club { Intro = "連" });

            club.Id.Should().Be(36);
            club.Intro.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_UpdateAsync_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var clubName = "歐陽邦瑋" + suffix;

            await clubDataAccess.Where(x => x.Id.Equals(15)).Set(() => new Club { Name = clubName }).UpdateAsync();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 15);

            club.Id.Should().Be(15);
            club.Name.Should().Be("歐陽邦瑋" + suffix);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_use_QueryObject_set_Null()
        {
            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            var club = await clubDataAccess.QueryOneAsync(x => x.Id == 36, selector: x => new { x.Id, x.Intro });

            club.Id.Should().Be(36);
            club.Intro.Should().Be("連");

            await clubDataAccess.Where(x => x.Id == 36).Set(() => new Club { Intro = null }).UpdateAsync();

            club = await clubDataAccess.Where(x => x.Id == 36).Select(x => new { x.Id, x.Intro }).QueryOneAsync();

            await clubDataAccess.Where(x => x.Id == 36).Set(() => new Club { Intro = "連" }).UpdateAsync();

            club.Id.Should().Be(36);
            club.Intro.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "歐陽邦瑋" + suffix },
                            new Club { Id = 16, Name = "羅怡君" + suffix },
                            new Club { Id = 19, Name = "楊翊貴" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpdateAsync(x => x.Id == default(int), () => new Club { Name = default(string) }, clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            actual.Single(x => x.Id.Equals(15)).Name.Should().Be("歐陽邦瑋" + suffix);
            actual.Single(x => x.Id.Equals(16)).Name.Should().Be("羅怡君" + suffix);
            actual.Single(x => x.Id.Equals(19)).Name.Should().Be("楊翊貴" + suffix);
        }

        [TestMethod]
        public async Task Test_UpdateAsync_Multiply_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "歐陽邦瑋" + suffix },
                            new Club { Id = 16, Name = "羅怡君" + suffix },
                            new Club { Id = 19, Name = "楊翊貴" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default(int)).Set(() => new Club { Name = default(string) }).UpdateAsync(clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            actual.Single(x => x.Id.Equals(15)).Name.Should().Be("歐陽邦瑋" + suffix);
            actual.Single(x => x.Id.Equals(16)).Name.Should().Be("羅怡君" + suffix);
            actual.Single(x => x.Id.Equals(19)).Name.Should().Be("楊翊貴" + suffix);
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(new Club { Id = clubId, Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub");

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            club.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_use_Setter()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(() => new Club { Id = clubId, Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub");

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            club.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_use_QueryObject()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = clubId, Name = "TestClub" }).InsertAsync();

            var club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            await clubDataAccess.Where(x => x.Id == clubId).DeleteAsync();

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub");

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            club.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_Multiply()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(
                new List<Club>
                {
                    new Club { Id = clubIds[1], Name = "TestClub999", IsActive = true }, new Club { Id = clubIds[0], Name = "TestClub998" }
                });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Id.Should().Be(clubIds[0]);
            clubs[0].IsActive.Should().BeFalse();
            clubs[1].IsActive.Should().BeTrue();
            clubs[1].Name.Should().Be("TestClub999");

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id)).Select(x => new { x.Id, x.Name }).QueryAsync();

            clubs.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_Multiply_use_Setter()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(
                () => new Club { Id = default(int), Name = default(string) },
                new List<Club> { new Club { Id = clubIds[1], Name = "TestClub999" }, new Club { Id = clubIds[0], Name = "TestClub998" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Id.Should().Be(clubIds[0]);
            clubs[1].Name.Should().Be("TestClub999");

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id)).Select(x => new { x.Id, x.Name }).QueryAsync();

            clubs.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Test_InsertAsync_and_DeleteAsync_Multiply_use_Setter_and_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = default(int), Name = default(string) })
                .InsertAsync(new List<Club> { new Club { Id = clubIds[1], Name = "TestClub999" }, new Club { Id = clubIds[0], Name = "TestClub998" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Id.Should().Be(clubIds[0]);
            clubs[1].Name.Should().Be("TestClub999");

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id)).Select(x => new { x.Id, x.Name }).QueryAsync();

            clubs.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Test_UpsertAsync()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpsertAsync(x => x.Id == clubId, () => new Club { Name = "TestClub" });

            var club = await clubDataAccess.Where(x => x.Id == clubId)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryOneAsync();

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub");
            club.IsActive.Should().BeTrue();

            await clubDataAccess.UpsertAsync(
                x => x.Id == clubId && x.IsActive == true,
                () => new Club { Name = "TestClub997", IsActive = false });

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub997");
            club.IsActive.Should().BeFalse();

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            club.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_UpsertAsync_use_QueryObject()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == clubId).Set(() => new Club { Name = "TestClub" }).UpsertAsync();

            var club = await clubDataAccess.Where(x => x.Id == clubId)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryOneAsync();

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub");
            club.IsActive.Should().BeTrue();

            await clubDataAccess.Where(x => x.Id == clubId && x.IsActive == true)
                .Set(() => new Club { Name = "TestClub997", IsActive = false })
                .UpsertAsync();

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club.Id.Should().Be(clubId);
            club.Name.Should().Be("TestClub997");
            club.IsActive.Should().BeFalse();

            club = await clubDataAccess.Where(x => x.Id == clubId)
                       .Select(x => new { x.Id, x.Name, x.IsActive })
                       .QueryOneAsync();

            club.Should().BeNull();
        }

        [TestMethod]
        public async Task Test_UpsertAsync_Multiply()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.UpsertAsync(
                x => x.Id == default(int),
                () => new Club { Name = default(string) },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");

            await clubDataAccess.UpsertAsync(
                x => x.Id == default(int),
                () => new Club { Name = default(string) },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub3");
            clubs[1].Name.Should().Be("TestClub4");
        }

        [TestMethod]
        public async Task Test_UpsertAsync_Multiply_use_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default(int))
                .Set(() => new Club { Name = default(string) })
                .UpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");

            await clubDataAccess.Where(x => x.Id == default(int))
                .Set(() => new Club { Name = default(string) })
                .UpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub3");
            clubs[1].Name.Should().Be("TestClub4");
        }

        [TestMethod]
        public async Task Test_BulkInsert()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkInsertAsync(
                () => new Club { Id = default(int), Name = default(string) },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");
        }

        [TestMethod]
        public async Task Test_BulkInsert_use_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Set(() => new Club { Id = default(int), Name = default(string) })
                .BulkInsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");
        }

        [TestMethod]
        public async Task Test_BulkInsert_use_QueryObject_with_RequiredColumns()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkInsertAsync(
                new List<Club>
                {
                    new Club { Id = clubIds[0], Name = "TestClub1", IsActive = false },
                    new Club { Id = clubIds[1], Name = "TestClub2", IsActive = true }
                });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name, x.IsActive })
                            .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");
            clubs[0].IsActive.Should().BeFalse();
            clubs[1].IsActive.Should().BeTrue();
        }

        [TestMethod]
        public async Task Test_BulkUpdate()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "歐陽邦瑋" + suffix },
                            new Club { Id = 16, Name = "羅怡君" + suffix },
                            new Club { Id = 19, Name = "楊翊貴" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkUpdateAsync(x => x.Id == default(int), () => new Club { Name = default(string) }, clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            actual.Single(x => x.Id.Equals(15)).Name.Should().Be("歐陽邦瑋" + suffix);
            actual.Single(x => x.Id.Equals(16)).Name.Should().Be("羅怡君" + suffix);
            actual.Single(x => x.Id.Equals(19)).Name.Should().Be("楊翊貴" + suffix);
        }

        [TestMethod]
        public async Task Test_BulkUpdate_use_QueryObject()
        {
            var suffix = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000).ToString();

            var clubs = new List<Club>
                        {
                            new Club { Id = 15, Name = "歐陽邦瑋" + suffix },
                            new Club { Id = 16, Name = "羅怡君" + suffix },
                            new Club { Id = 19, Name = "楊翊貴" + suffix }
                        };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id == default(int)).Set(() => new Club { Name = default(string) }).BulkUpdateAsync(clubs);

            var actual = await clubDataAccess.QueryAsync(x => new[] { 15, 16, 19 }.Contains(x.Id), selector: x => new { x.Id, x.Name });

            actual.Single(x => x.Id.Equals(15)).Name.Should().Be("歐陽邦瑋" + suffix);
            actual.Single(x => x.Id.Equals(16)).Name.Should().Be("羅怡君" + suffix);
            actual.Single(x => x.Id.Equals(19)).Name.Should().Be("楊翊貴" + suffix);
        }

        [TestMethod]
        public async Task Test_BulkUpsertAsync()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.BulkUpsertAsync(
                x => x.Id >= 0 && x.Id <= 0,
                () => new Club { Name = default(string) },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");

            await clubDataAccess.BulkUpsertAsync(
                x => x.Id == default(int),
                () => new Club { Name = default(string) },
                new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub3");
            clubs[1].Name.Should().Be("TestClub4");
        }

        [TestMethod]
        public async Task Test_BulkUpsertAsync_use_QueryObject()
        {
            var clubIds = new[]
                          {
                              new Random(Guid.NewGuid().GetHashCode()).Next(100, 500),
                              new Random(Guid.NewGuid().GetHashCode()).Next(500, 1000)
                          };

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.Where(x => x.Id >= default(int) && x.Id <= default(int))
                .Set(() => new Club { Name = default(string) })
                .BulkUpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub1" }, new Club { Id = clubIds[1], Name = "TestClub2" } });

            var clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                            .OrderBy(x => x.Id)
                            .Select(x => new { x.Id, x.Name })
                            .QueryAsync();

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub1");
            clubs[1].Name.Should().Be("TestClub2");

            await clubDataAccess.Where(x => x.Id == default(int))
                .Set(() => new Club { Name = default(string) })
                .BulkUpsertAsync(
                    new List<Club> { new Club { Id = clubIds[0], Name = "TestClub3" }, new Club { Id = clubIds[1], Name = "TestClub4" } });

            clubs = await clubDataAccess.Where(x => clubIds.Contains(x.Id))
                        .OrderBy(x => x.Id)
                        .Select(x => new { x.Id, x.Name })
                        .QueryAsync();

            await clubDataAccess.DeleteAsync(x => clubIds.Contains(x.Id));

            clubs.Count.Should().Be(2);
            clubs[0].Name.Should().Be("TestClub3");
            clubs[1].Name.Should().Be("TestClub4");
        }

        [TestMethod]
        public async Task Test_TransactionScope_Query_and_Update()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(() => new Club { Id = clubId, Name = "TestClub" });

            Club club;
            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

                club.Name += "989";

                await clubDataAccess.Where(x => x.Id == clubId).Set(() => new Club { Name = club.Name }).UpdateAsync();

                tx.Complete();
            }

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club.Name.Should().Be("TestClub989");
        }

        [TestMethod]
        public async Task Test_TransactionScope_Multiple_Query_and_Update()
        {
            var clubId = new Random(Guid.NewGuid().GetHashCode()).Next(100, 1000);

            IDataAccess<Club> clubDataAccess = new ClubDataAccess();

            await clubDataAccess.InsertAsync(() => new Club { Id = clubId, Name = "TestClub" });

            Club club;
            using (var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

                club.Name += "979";

                await clubDataAccess.Where(x => x.Id == default(int))
                    .Set(() => new Club { Name = default(string) })
                    .UpdateAsync(new List<Club> { new Club { Id = clubId, Name = club.Name } });

                tx.Complete();
            }

            club = await clubDataAccess.Where(x => x.Id == clubId).Select(x => new { x.Id, x.Name }).QueryOneAsync();

            await clubDataAccess.DeleteAsync(x => x.Id == clubId);

            club.Name.Should().Be("TestClub979");
        }
    }

    internal class Club
    {
        [Column("ClubID")]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Intro { get; set; }

        public DateTime? RunningTime { get; set; }
    }

    internal class ClubDataAccess : SqlServerDataAccess<Club>
    {
        public ClubDataAccess()
            : base(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Club;Integrated Security=True")
        {
        }

        protected override Expression<Func<Club, object>> DefaultSelector { get; } = x => new { x.Id, x.Name };

        protected override Expression<Func<Club>> RequiredColumns { get; } =
            () => new Club { Id = default(int), Name = default(string), IsActive = default(bool) };

        protected override (string, DataTable) ConvertToTableValuedParameters(IEnumerable<Club> values)
        {
            var dataTable = CreateDataTable();

            foreach (var value in values)
            {
                dataTable.Rows.Add(CreateDataRow(dataTable, value));
            }

            return ("ClubType", dataTable);
        }

        private static DataTable CreateDataTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("ClubID", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("IsActive", typeof(bool));

            return dataTable;
        }

        private static DataRow CreateDataRow(DataTable dataTable, Club value)
        {
            var dataRow = dataTable.NewRow();

            dataRow["ClubID"] = value.Id;
            dataRow["Name"] = value.Name;
            dataRow["IsActive"] = value.IsActive;

            return dataRow;
        }
    }
}
