namespace Products.Tests.Common.Helpers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    public static class TestHelper
    {
        public static string GetResultUrlString(ActionResult result)
        {
            string viewName;

            // ReSharper disable once MergeCastWithTypeCheck
            if (result is RedirectToRouteResult)
            {
#pragma warning disable IDE0020 // Use pattern matching
                var redirect = (RedirectToRouteResult)result;
#pragma warning restore IDE0020 // Use pattern matching
                viewName = redirect.RouteValues["action"].ToString();
            }
            else
            {
                var view = (ViewResult)result;
                viewName = view.ViewName;
            }

            return viewName;
        }

        public static void ValidateViewModel(this Controller controller, object viewModel)
        {
            controller.ModelState.Clear();

            var validationContext = new ValidationContext(viewModel, null, null);
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            foreach (ValidationResult result in validationResults)
            {
                controller.ModelState.AddModelError(result.MemberNames?.FirstOrDefault() ?? string.Empty, result.ErrorMessage);
            }
        }
    }
}
