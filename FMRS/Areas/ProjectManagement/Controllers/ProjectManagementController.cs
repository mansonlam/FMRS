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

namespace FMRS.Areas.ProjectManagement.Controllers
{
    [Authorize]
    [Area("ProjectManagement")]
    [Route("[controller]/[action]")]
    public class ProjectManagementController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public ProjectManagementController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
        }

        public IActionResult Index()
        {
            Session.SetString("current_sys", "M");
            ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M"), false);
            ViewBag.ClusterList = MenuService.GetClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M"));
            ViewBag.ClusterExHAHOList = MenuService.GetClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M"), false);

            return View();
        }
    }
}