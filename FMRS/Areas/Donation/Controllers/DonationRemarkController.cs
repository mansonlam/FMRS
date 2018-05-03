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
using FMRS.Model.DTO;

namespace FMRS.Areas.Donation.Controllers
{
    [Authorize]
    [Area("Donation")]
    [Route("[controller]/[action]")]
    public class DonationRemarkController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;
        private IDonationRemarkService DonationRemarkService;

        public DonationRemarkController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IDonationRemarkService _donationRemarkService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            DonationRemarkService = _donationRemarkService;
        }

        public IActionResult Index(string inst_code)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            ViewBag.DonationPeriod = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Donation_Period");
            ViewBag.PrivilegeDonation = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Donation");
            ViewBag.HospDesc = MenuService.GetFlashRptHospGpDesc(inst_code);
            DonationRemarkModel model = new DonationRemarkModel();
            model.Inst_code = inst_code;
            model.Remark = DonationRemarkService.GetRemarkByHosp(inst_code);
            return View(model);
        }

        public IActionResult UploadDonationRemark(DonationRemarkModel model)
        {
            string login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            string remark = model.Remark.Replace("'", "''");
            DonationRemarkService.UpdateDonationRemark(model, login_id);
            return RedirectToAction("Index", "DonationRemark", new { inst_code = model.Inst_code });
        }

    }
}