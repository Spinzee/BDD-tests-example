using System.Diagnostics;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using WebSite.Mappers;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IProductService _productService;

        public HomeController(IProfileService profileService, IProductService productService)
        {
            _profileService = profileService;
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Calender()
        {
            return View("Calender");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var viewModel = new LoginViewModel();

            return View("Login", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var loginStatus = _profileService.AttemptLogin(viewModel.Email, viewModel.Password);

                if (!loginStatus.IsPasswordValid || !loginStatus.IsEmailValid)
                {
                    return RedirectToAction("Login", viewModel);
                }

                return loginStatus.Status switch
                {
                    AccountStatus.Disabled => RedirectToAction("Disabled"),
                    AccountStatus.AwaitingActivation => RedirectToAction("AwaitingActivation"),
                    AccountStatus.Locked => RedirectToAction("Locked"),
                    AccountStatus.Active => RedirectToAction("Products"),
                    _ => RedirectToAction("Login", viewModel)
                };
            }

            return RedirectToAction("Login", viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Products()
        {
            var viewModel = HomeControllerMapper.GetProductsViewModel(_productService.GetListOfServices());

            return View("Products", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Disabled()
        {
            return View("Disabled");
        }

        public IActionResult AwaitingActivation()
        {
            return View("AwaitingActivation");
        }

        public IActionResult Locked()
        {
            return View("Locked");
        }
    }
}
