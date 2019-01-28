using System.Web.Mvc;

namespace Chef.Extensions.ActionFilter
{
    public static class ActionExecutingContextExtension
    {
        public static T GetActionParameter<T>(this ActionExecutingContext me, string name)
        {
            return me.ActionParameters.ContainsKey(name) ? (T)me.ActionParameters[name] : default(T);
        }
    }
}