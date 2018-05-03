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
using FMRS.Model.DTO;

namespace FMRS.Areas.Donation.Controllers
{
    [Authorize]
    [Area("Donation")]
    [Route("[controller]/[action]")]
    public class DonationReserveController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private IDonationReserveService DonationReserveService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public DonationReserveController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IDonationReserveService _donationReserveService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            DonationReserveService = _donationReserveService;
        }

        [HttpGet]
        public IActionResult Index(string inst_code)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);
            ViewBag.HospDesc = MenuService.GetFlashRptHospGpDesc(inst_code);

            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date");
            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            DonationReserveModelCollectionSet model = DonationReserveService.GetDonationReserveModelCollectionSet(value_date, inst_code);
            
            return View(model);
        }

        [HttpPost]
        public IActionResult GetDonationReserveRecord(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int curr_financial_year, DateTime current_date, int oid, string don_inc_exp, string don_desc, string donor_name, short don_cat)
        {
            int record_cnt = 0;
            var list = DonationReserveService.GetExistList2(don_inc_exp, inst_code, fund_code, analytical_code, section_code, financial_year, curr_financial_year, current_date, oid, don_desc, donor_name, don_cat, out record_cnt);
            DonationRecNExpModel model = new DonationRecNExpModel();
            model.Link_rec = list;
            model.Inst_code = inst_code;
            model.Fund = fund_code;
            model.Analytical = analytical_code;
            model.Section = section_code;
            model.OId = oid;
            model.Don_inc_exp = don_inc_exp;
            model.Don_kind_desc = don_desc;
            model.Donor_name = donor_name;
            model.Don_supercat = don_cat;
            return ViewComponent("DonationReserveSubList", new { record_cnt = record_cnt, model = model });
        }

    }
}