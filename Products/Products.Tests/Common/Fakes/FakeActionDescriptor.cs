namespace Products.Tests.Common.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class FakeActionDescriptor : ActionDescriptor
    {
        public FakeActionDescriptor(string actionName)
        {
            ActionName = actionName;
        }

        public override string ActionName { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public override ControllerDescriptor ControllerDescriptor { get; }

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public override ParameterDescriptor[] GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}