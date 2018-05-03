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
using System.Globalization;

namespace FMRS.Areas.Report.Controllers
{
    [Authorize]
    [Area("Report")]
    [Route("[controller]/[action]")]
    public class ReportController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private IReportService ReportService;
        private IUserService UserService;

        private ISession Session => HttpContextAccessor.HttpContext.Session;
        public ReportController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IReportService _reportService,
                                IUserService _userService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            ReportService = _reportService;
            UserService = _userService;
        }

        public IActionResult Index(string modules, string rpt_group, string rpt_name)
        {
            Session.SetString("current_sys", modules);
            ViewBag.Rpt_group = rpt_group??"";
            ViewBag.Rpt_name = rpt_name??"";
            ViewBag.Modules = modules;
            var current_sys = modules;
            var user_group = GetUserGroup(modules);
            SetMenuDropDownList(user_group);

            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            var privilege_non_pjt_report = "";
            var privilege_asoi_rpt = "";
            var privilege_report = "";
            var ho_access = "";
            var privilege_closing_report = "";

            if (modules == "Y")
            {
                privilege_non_pjt_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Non_Pjt_Report_Y");
                privilege_asoi_rpt = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Asoi_Rpt_Y");
                privilege_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Report_Y");
                ho_access = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Ho_Access_Y");
                privilege_closing_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Closing_Report_Y");
            }
            if (modules == "M")
            {
                privilege_non_pjt_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Non_Pjt_Report_M");
                privilege_asoi_rpt = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Asoi_Rpt_M");
                privilege_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Report_M");
                ho_access = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Ho_Access_M");
                privilege_closing_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Closing_Report_M");
            }
            if (modules == "D")
            {
                privilege_non_pjt_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Non_Pjt_Report_D");
                privilege_asoi_rpt = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Asoi_Rpt_D");
                privilege_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Report_D");
                ho_access = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Ho_Access_D");
                privilege_closing_report = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Closing_Report_D");
            }

            var report_period = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Report_Period"));
            if (login_id == "super") { report_period = 5; }
                      
            var cluster_level = UserService.GetClusterLevel(user_group);

            ViewBag.Cluster_level = cluster_level;
            ViewBag.Privilege_closing_report = privilege_closing_report;

            GetReportFormControl(user_group, user_inst_code, login_id, current_sys);

            List<ReportViewModel> report_detail_list = ReportService.GetReportDetailList(current_sys, user_group, user_inst_code, login_id, privilege_non_pjt_report,
            privilege_asoi_rpt, privilege_report, privilege_closing_report, ho_access, report_period, cluster_level, out int cnt);
            ViewBag.RptGpList = ReportService.GetReportGpList(modules, user_inst_code,cnt, report_detail_list, false);
            ViewBag.RptNameList = ReportService.GetReportNameList(modules, user_inst_code, cnt, report_detail_list, false);
            ViewBag.Report_detail_list = report_detail_list;
            ViewBag.RptHospGpList = ReportService.GetReportHospGpList(user_group, user_inst_code, ho_access, login_id, current_sys, out Dictionary<string, string> consolidate_ind_dict);

            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            var actual_cnt = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Actual_Cnt"));

            ReportViewModel model = new ReportViewModel();
            model.AsAtList = ReportService.GetAsAtList(financial_year, actual_cnt, false);
            model.AsAtMList = ReportService.GetAsAtMList(financial_year, actual_cnt, false);
            model.FlashRptDateList = ReportService.GetFlashRptDateList(financial_year, actual_cnt, false);
            model.FlashRptDateList2 = ReportService.GetFlashRptDateList2(financial_year, actual_cnt, false);
            model.Report_detail_list = report_detail_list;
            model.Cnt = cnt;
            model.Consolidate_ind_dict = consolidate_ind_dict;
            model.FVIndTenderer_list = ReportService.GetFVIndTenderer_list();
            model.Rtp_item_list = ReportService.GetRtpItem_list(user_group, login_id);
            model.Report_group_full_list = ReportService.GetRptGpFullList(current_sys, user_group, user_inst_code, login_id, privilege_non_pjt_report,
            privilege_asoi_rpt, privilege_report, privilege_closing_report, ho_access, report_period, cluster_level);
            return View(model);
        }

        [HttpPost]
        public IActionResult GenerateReport(ReportViewModel model)
        {
            Session.SetString("current_sys", model.Modules);
            ViewBag.Modules = model.Modules;
            var current_sys = model.Modules;
            var user_group = GetUserGroup(model.Modules);
            SetMenuDropDownList(user_group);
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date");
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            var other_wk_agent = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Other_Wk_Agent");
            var p13 = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "p13");
            var year_end = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Year_End");
            var current_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Current_Year"));
            var current_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Current_Date"); 
            var period_end_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Period_End_Date");
            GenReportModel rptModel = ReportService.GetReportParm(model, value_date, login_id, financial_year, other_wk_agent, p13, year_end, user_group, current_year, current_date, period_end_date);

            return View(rptModel);
        }

        private string GetUserGroup(string modules)
        {
            switch (modules)
            {
                case "D":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D");
                case "M":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M");
                case "Y":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_Y");
                default:
                    return "";
            }
        }
        private void SetMenuDropDownList(string user_group)
        {
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(user_group);
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(user_group, false);
            ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(user_group, false);
            ViewBag.ClusterList = MenuService.GetClusterList(user_group);
            ViewBag.ClusterExHAHOList = MenuService.GetClusterList(user_group, false);
        }
        private void GetReportFormControl(string user_group, string user_inst_code, string login_id, string current_sys)
        {
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date");
            ViewBag.UserGroup = user_group;
            ViewBag.UserInstCode = user_inst_code;
            ViewBag.AsAtYList = ReportService.GetAsAtYList(value_date, false);
            ViewBag.BatchNoList = ReportService.GetBatchNoList();
            ViewBag.BudgetClusterHospList = ReportService.GetBudgetClusterHospList(user_inst_code, false);
            ViewBag.CntCbv = ReportService.GetCbvAccessControlByUserNameCount(login_id);
            ViewBag.CntCwrf = ReportService.GetCwrfAccessControlByUserName(login_id);
            ViewBag.DonSuperCatList = ReportService.GetDonSuperCat();
            ViewBag.FundGpList = ReportService.GetFundGpList();
            ViewBag.FVIndList = ReportService.GetFVIndList();
            ViewBag.HospClusterList_forM = ReportService.GetHospClusterList_forM(user_group, false);
            ViewBag.HospClusterList_WithAllHAHO = ReportService.GetHospClusterList_forM(user_group);
            ViewBag.ProjItemIndList = ReportService.GetProjItemIndList();
            ViewBag.RptHospList = ReportService.GetReportHospList(login_id, current_sys);
            ViewBag.Snapshot = ReportService.GetSnapshotYr(false);
            ViewBag.SnapshotDateListByMaxYr = ReportService.GetSnapshotDateListByMaxYr();
            ViewBag.SnapshotMaxYr = ReportService.GetSnapshotMaxYr();
            ViewBag.WorkAgentList = ReportService.GetWorkAgentList();
            
        }
    }
}