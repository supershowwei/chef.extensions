using System;
using System.Linq;
using System.Reflection;

namespace Chef.Extensions.LiteDB
{
    internal class BackingField
    {
        private readonly string name;
        private readonly Type declaringType;
        private FieldInfo field;

        public BackingField(string name, Type declaringType)
        {
            this.name = name;
            this.declaringType = declaringType;
        }

        public static string GenerateKey(string name, Type declaringType)
        {
            return $"<{declaringType.AssemblyQualifiedName}>_<{name}>";
        }

        public void SetValue(object obj, object value)
        {
            if (this.field == null)
            {
                this.field = this.declaringType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Single(f => f.Name.Equals($"<{this.name}>k__BackingField"));
            }

            this.field.SetValue(obj, value);
        }
    }
}