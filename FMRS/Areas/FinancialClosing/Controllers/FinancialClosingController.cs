using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using FMRS.Service;
using FMRS.Helper;
using System.Security.Claims;

namespace FMRS.Areas.FinancialClosing.Controllers
{
    [Authorize]
    [Area("FinancialClosing")]
    [Route("[controller]/[action]")]
    public class FinancialClosingController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public FinancialClosingController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
        }
        public IActionResult Index()
        {
            Session.SetString("current_sys", "Y");
            ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_Y"), false);
            return View();
        }
    }
}