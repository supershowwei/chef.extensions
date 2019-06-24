using System.Xml.Linq;

namespace Chef.Extensions.XElement
{
    public static class Extension
    {
        public static bool TryGetAttribute(this System.Xml.Linq.XElement me, XName name, out XAttribute attribute)
        {
            attribute = me.Attribute(name);

            return attribute != null;
        }
    }
}