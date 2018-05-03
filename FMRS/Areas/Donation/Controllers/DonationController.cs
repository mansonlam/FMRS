using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FMRS.Helper;
using FMRS.Service;

namespace FMRS.Areas.Donation.Controllers
{
    [Authorize]
    [Area("Donation")]
    [Route("[controller]/[action]")]
    public class DonationController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public DonationController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
        }

        public IActionResult Index()
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            return View();
        }
    }
}