using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FMRS.Service;
using FMRS.Helper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FMRS.Model.DTO;
using System.Globalization;

namespace FMRS.Areas.Donation.Controllers
{
    [Authorize]
    [Area("Donation")]
    [Route("[controller]/[action]")]
    public class DonationUploadController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private readonly IDonationUploadService DonationUploadService;

        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public DonationUploadController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IDonationUploadService _donationUploadService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            DonationUploadService = _donationUploadService;
        }

        [HttpGet]
        public IActionResult Index(string rowschecked, string remark, string error_list)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            DonationUploadViewModel model = new DonationUploadViewModel();
            model.Inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            model.Rows_Checked = rowschecked ?? "";
            model.Remark = remark ?? "";
            model.Error_list = error_list ?? "";
            
            return View(model);
        }

        [HttpPost]
        public IActionResult CheckDonationUpload(DonationUploadViewModel model)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            DateTime current_date = DateTime.ParseExact(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Current_Date"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US"));
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2");
            var user_group = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D");
            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));

            model.Record_array = DonationUploadService.GetRecordArray(model.Remark);
            var temp_record_list = DonationUploadService.GetRecordList(model.Record_array, current_date, value_date, out int rec_cnt, out int break_line);
            model.Record_list = DonationUploadService.RecordValidation(temp_record_list, user_group, user_inst_code, current_date, value_date, financial_year, out int valid_rec_cnt, out int invalid_rec_cnt);
            model.Rec_cnt = rec_cnt;
            model.Break_line = break_line;
            model.Valid_rec_cnt = valid_rec_cnt;
            model.Invalid_rec_cnt = invalid_rec_cnt;
            if (model.Action == "Upload")
            {
                return UploadDonationUpload(model);
            }
            else
            {
                return View(model);
            }
        }

        
        public IActionResult UploadDonationUpload(DonationUploadViewModel model) 
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            DateTime current_date = DateTime.ParseExact(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Current_Date"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US"));
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2");
            var user_group = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D");
            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));

            var temp_record_list = DonationUploadService.GetRecordList(model.Record_array, current_date, value_date, out int rec_cnt, out int break_line);
            model.Record_list = DonationUploadService.RecordValidation(temp_record_list, user_group, user_inst_code, current_date, value_date, financial_year, out int valid_rec_cnt, out int invalid_rec_cnt);
            model.Valid_rec_cnt = valid_rec_cnt;
            model.Invalid_rec_cnt = invalid_rec_cnt;
            return View("UploadDonationUpload", model);
        }


        
        public IActionResult GetLine(string inst_code, string remark, string record_array, string function_mode, int show_valid_only)
        {
            DateTime current_date = DateTime.ParseExact(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Current_Date"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US"));
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2");
            var user_group = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D");
            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode"); 
            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId"); 
            DonationUploadViewModel model = new DonationUploadViewModel();
            model.Inst_code = inst_code;
            model.Valid_only = show_valid_only;
            model.Remark = remark;
            var temp_record_list = DonationUploadService.GetRecordList(record_array, current_date, value_date, out int rec_cnt, out int break_line);
            model.Record_list = DonationUploadService.RecordValidation(temp_record_list, user_group, user_inst_code, current_date, value_date, financial_year, out int valid_rec_cnt, out int invalid_rec_cnt);
            model.Sql = DonationUploadService.GetLine(model.Record_list, function_mode, show_valid_only, login_id, value_date);
            if(!string.IsNullOrEmpty(model.Sql))
                DonationUploadService.ExecuteUploadSQL(model.Sql);
            return ViewComponent("DonationUploadDetail", new { model = model });
        }
    }
}