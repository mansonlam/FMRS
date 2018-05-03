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
using System.Globalization;

namespace FMRS.Areas.Donation.Controllers
{
    [Authorize]
    [Area("Donation")]
    [Route("[controller]/[action]")]
    public class DonationRecNExpReconController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;
        private IDonationRecNExpReconService DonationRecNExpReconService;

        public DonationRecNExpReconController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IDonationRecNExpReconService _donationRecNExpReconService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            DonationRecNExpReconService = _donationRecNExpReconService;
        }

        public IActionResult Index(string inst_code, int recon_type)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year")); //2017;
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date");//"20180101"; 
            var value_date2 = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2");//"20171201";
            var cal_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Cal_date"); //"20180221"; 
            var last_month = DateTime.ParseExact(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US"));
            var input_period = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Input_Period");
            int cnt = DonationRecNExpReconService.GetTrendTbCntByValueDate(value_date);
            if (value_date.Substring(0, 6) != cal_date.Substring(0, 6))
            {
                if (cnt > 0)
                {value_date = cal_date.Substring(0, 6) + "01";}
            }
            if (cnt > 0)
            {
                last_month = last_month.AddMonths(-1);
            }
            ViewBag.LastMonth = last_month;
            ViewBag.FinancialYear = financial_year;
            ViewBag.HospDesc = MenuService.GetFlashRptHospGpDesc(inst_code);
            var inc_exp = recon_type == 1 ? "I" : "E";

            DonationReconModel model = new DonationReconModel();
            model.Inst_code = inst_code;
            model.Recon_type = recon_type;
            model.Movement_list = DonationRecNExpReconService.GetErpDonationMovementList(inst_code, value_date, financial_year, recon_type, out decimal total_cost);
            model.Cost = DonationRecNExpReconService.GetDonationMovement(inst_code, value_date, financial_year, recon_type, 0);
            model.Total = total_cost;
            model.DonationList = DonationRecNExpReconService.GetDonationList(inst_code, financial_year, financial_year, value_date2, 2, "", inc_exp,"");
            return View(model);
        }

        public IActionResult UpdateDonationMovement(DonationReconModel model)
        {
            return View();
        }
    }
}