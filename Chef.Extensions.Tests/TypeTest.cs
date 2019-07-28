using Chef.Extensions.Type;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chef.Extensions.Tests
{
    [TestClass]
    public class TypeTest
    {
        [TestMethod]
        public void Test_GetActivator()
        {
            var activator1 = typeof(ParameterlessCtor).GetActivator();

            var obj = activator1() as ParameterlessCtor;

            obj.Id = 1;
            obj.Name = "1";

            obj.Should().NotBeNull();
            obj.Id.Should().Be(1);

            var activator2 = ObjectActivatorBuilder.Build<ParameterlessCtor>();

            obj = activator2();

            obj.Id = 2;
            obj.Name = "2";

            obj.Should().NotBeNull();
            obj.Id.Should().Be(2);
        }

        [TestMethod]
        public void Test_GetActivator_With_Parameter_Types()
        {
            var activator1 = typeof(ParameterfulCtor).GetActivator(typeof(int), typeof(string));

            var obj = activator1(1, "1") as ParameterfulCtor;

            obj.Should().NotBeNull();
            obj.Id.Should().Be(1);

            var activator2 = ObjectActivatorBuilder.Build<ParameterfulCtor>(typeof(int), typeof(string));

            obj = activator2(2, "2");

            obj.Should().NotBeNull();
            obj.Id.Should().Be(2);
        }
    }

    internal class ParameterlessCtor
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    internal class ParameterfulCtor
    {
        public ParameterfulCtor(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; }

        public string Name { get; }
    }
}