namespace Products.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using NUnit.Framework;
    using Web;

    [TestFixture]
    public class AntiForgeryTokenOnControllersTests
    {
        [Test]
        public void AllHttpPostControllerActionsShouldHaveValidateAntiForgeryTokenAttribute()
        {
            IEnumerable<Type> allControllerTypes = Assembly.GetAssembly(typeof(MvcApplication)).GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type));
            List<MethodInfo> allControllerActions = allControllerTypes.SelectMany(type => type.GetMethods()).ToList();

            List<MethodInfo> failingActions = allControllerActions
                .Where(method => Attribute.GetCustomAttribute(method, typeof(HttpPostAttribute)) != null)
                .Where(method => Attribute.GetCustomAttribute(method, typeof(ValidateAntiForgeryTokenAttribute)) == null)
                .ToList();

            string message = string.Empty;
            if (failingActions.Any())
            {
                message =
                    failingActions.Count + " failing action" +
                    (failingActions.Count == 1 ? ":\n" : "s:\n") +
                    failingActions.Select(method => method.Name + " in " + method.DeclaringType?.Name)
                        .Aggregate((a, b) => a + ",\n" + b);
            }

            Assert.IsFalse(failingActions.Any(), message);
        }
    }
}
