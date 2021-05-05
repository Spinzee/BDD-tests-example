using Products.Model.TariffChange.Enums;
using Products.Service.TariffChange;
using Products.WebModel.Resources.TariffChange;
using Products.WebModel.ViewModels.TariffChange;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Products.Web.Areas.TariffChange.Controllers
{
    [RouteArea("TariffChange", AreaPrefix = "tariff-change")]
    public class CreatePasswordController : Controller
    {        
        private readonly ICustomerService _customerService;
        private readonly IJourneyDetailsService _journeyDetailsService;

        public CreatePasswordController(
            ICustomerService customerService,
            IJourneyDetailsService journeyDetailsService)
        {
            _customerService = customerService;
            _journeyDetailsService = journeyDetailsService;
        }

        [HttpGet]
        [Route("create-password")]
        public ActionResult CreatePassword()
        {
            if (_customerService.CheckEmailAddressIsNotNull())
            {
                var viewModel = new CreatePasswordViewModel
                {
                    ProgressBarViewModel = GetProgressBarViewModel()
                };

                return View("CreatePassword", viewModel);
            }

            return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("create-password")]
        public ActionResult CreateProfile(CreatePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool emailAddressSet = _customerService.CheckEmailAddressIsNotNull();

                if (emailAddressSet)
                {                    
                    _journeyDetailsService.SavePassword(viewModel.Password);
                    return RedirectToAction("TariffSummary", "Summary");                    
                }

                return RedirectToAction("IdentifyCustomer", "CustomerIdentification");
            }

            viewModel.ProgressBarViewModel = GetProgressBarViewModel();

            return View("CreatePassword", viewModel);
        }

        private ProgressBarViewModel GetProgressBarViewModel()
        {
            return new ProgressBarViewModel
            {
                Sections = new List<ProgressBarSection>
                {
                    new ProgressBarSection
                    {
                        Text = ProgressBar_Resource.UserDetailsSectionHeader,
                        Status = ProgressBarStatus.Done
                    },
                    new ProgressBarSection
                    {
                        Text = ProgressBar_Resource.TariffChoiceSectionHeader,
                        Status = ProgressBarStatus.Done
                    },
                    new ProgressBarSection
                    {
                        Text = ProgressBar_Resource.EmailAddressSectionHeader,
                        Status = ProgressBarStatus.Active,
                        StepsToComplete = 2,
                        CompletedStep = 1
                    },
                    new ProgressBarSection
                    {
                        Text = ProgressBar_Resource.SummarySectionHeader,
                        Status = ProgressBarStatus.Awaiting
                    }
                }
            };
        }
    }
}