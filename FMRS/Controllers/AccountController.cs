using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMRS.Models;
using FMRS.Models.Account;
using FMRS.Service;
using FMRS.Common.DataSource;
using FMRS.Common.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FMRS.Helper;

namespace FMRS.Controllers
{
    public class AccountController : Controller
    {
        private IOptions<AppConfiguration> AppConfiguration { get; set; }
        private IUserService UserService;
        private IMenuService MenuService;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public AccountController(IOptions<AppConfiguration> settings, IUserService _userService,
            IMenuService _menuService, IHttpContextAccessor _httpContextAccessor)
        {
            AppConfiguration = settings;
            UserService = _userService;
            MenuService = _menuService;
            HttpContextAccessor = _httpContextAccessor;
        }

        [AllowAnonymous, HttpGet]
        public ActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Hotline_Home = AppConfiguration.Value.BSD_Url;
            ViewBag.Domain = AppConfiguration.Value.FMRS_idmanDomain.Replace("\\", "");
            return View(new LoginModel { Domain = AppConfiguration.Value.FMRS_idmanDomain.Replace("\\", "") });
        }
        
        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl = "")
        {
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.Hotline_Home = AppConfiguration.Value.BSD_Url;
            ViewBag.Domain = AppConfiguration.Value.FMRS_idmanDomain.Replace("\\","");
            try
            {
                if (ModelState.IsValid)
                {
                    var loginResult = UserService.LoginUser(model.UserName.ToUpper(), model.Password, out List<string> system_access);
                    if (!loginResult)
                    {
                        ModelState.AddModelError("", Resource.ErrLoginFailure);
                        return View(model);
                    }
                    else
                    {
                        var claims = UserService.CreateClaims(model.UserName.ToUpper(), system_access, model.Domain);

                        // Create identity
                        var userIdentity = new ClaimsIdentity(claims, "login");
                        // Create principal
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                        
                        // Sign in
                        await HttpContext.SignInAsync(principal);

                        var MAINPAGE = "";
                        //Set landing page path
                        switch (system_access[0])
                        {
                            case "FMRS_SYS_Y":
                                MAINPAGE = "FinancialClosing";
                                Session.SetString("current_sys", "Y");
                                ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_Y"), false);
                                break;
                            case "FMRS_SYS_M":
                                MAINPAGE = "ProjectManagement";
                                Session.SetString("current_sys", "M");
                                ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M"), false);
                                ViewBag.ClusterList = MenuService.GetClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M"));
                                ViewBag.ClusterExHAHOList = MenuService.GetClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M"), false);
                                break;
                            case "FMRS_SYS_D":
                                MAINPAGE = "Donation";
                                Session.SetString("current_sys", "D");
                                ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
                                ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);
                                break;
                        }
                        
                        if (!string.IsNullOrEmpty(ReturnUrl) && !ReturnUrl.Contains("Logout"))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", MAINPAGE);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ModelState.AddModelError("", Resource.ErrLoginFailure);
            return View(model);            
        }

        public async Task<IActionResult> Logout()
        { 
            //var domain = User.FindFirst("domain").Value??"";

            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Help()
        {
            return View();
        }
    }
}