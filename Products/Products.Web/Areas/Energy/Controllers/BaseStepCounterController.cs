namespace Products.Web.Areas.Energy.Controllers
{
    using System.Web.Mvc;
    using ControllerHelpers.Energy;
    using Infrastructure;

    public class BaseStepCounterController : BaseController
    {
        private readonly IStepCounterControllerHelper _stepCounterControllerHelper;

        public BaseStepCounterController(IStepCounterControllerHelper stepCounterControllerHelper)
        {
            Guard.Against<System.ArgumentNullException>(stepCounterControllerHelper == null, $"{nameof(stepCounterControllerHelper)} is null");
            _stepCounterControllerHelper = stepCounterControllerHelper;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string actionName = filterContext.ActionDescriptor.ActionName;
            ViewBag.StepCounter = _stepCounterControllerHelper.GetStepCounter(actionName);
            base.OnActionExecuted(filterContext);
        }
    }
}