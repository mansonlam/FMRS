using FMRS.Common.Resources;
using FMRS.Helper;
using FMRS.Model.DTO;
using FMRS.Service;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Linq;
using System.Web;

namespace FMRS.Areas.Donation.Controllers
{
    [Authorize]
    [Area("Donation")]
    [Route("[controller]/[action]")]
    public class DonationRecNExpController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IDonationRecNExpService DonationRecNExpService;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;

        public DonationRecNExpController(IHttpContextAccessor _httpContextAccessor, IDonationRecNExpService _donationRecNExpService,
             IMenuService _menuService)
        {
            HttpContextAccessor = _httpContextAccessor;
            DonationRecNExpService = _donationRecNExpService;
            MenuService = _menuService;
        }

        [HttpGet]
        public IActionResult Index(string inst_code, string donor_name, int show_year)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            if (show_year == 0) show_year = financial_year;
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2"); //"20171101";
            //var value_date =  //test
            var in_donor_name = string.IsNullOrEmpty(donor_name)?"": donor_name.Replace("\"\"", "\"\"\"\"").Replace("'", "''");
            ViewBag.InDonorName = in_donor_name;
            Decimal total_cost = 0;
            ViewBag.CostA = DonationRecNExpService.GetCostA(inst_code, show_year, financial_year, in_donor_name);
            ViewBag.CostB = DonationRecNExpService.GetCostB(inst_code, show_year, financial_year, in_donor_name);
            DonationRecNExpModelCollectionSet model = DonationRecNExpService.GetDonationCollectionSet(inst_code, show_year, financial_year, value_date, in_donor_name);
            model.Net_designated_ytd = model.Net_designated_ytd + ViewBag.CostA;
            model.Net_general_ytd = model.Net_general_ytd + ViewBag.CostB;
            model.Grand_total_don_ytd = model.Net_designated_ytd + model.Net_general_ytd;
            model.Donation_by_year_list = DonationRecNExpService.GetDonationByYear(inst_code, in_donor_name, out total_cost);
            model.Total_cost = total_cost;
            ViewBag.HospDesc = MenuService.GetFlashRptHospGpDesc(inst_code);
            ViewBag.InstCode = inst_code;
            ViewBag.ShowYear = show_year;
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Input(int id, int oid, string action, string type, string inst_code,string section, string fund, string analytical, string don_inc_exp, string odon_inc_exp)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            DonationRecNExpModel model = new DonationRecNExpModel();
            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            model.Id = id;
            model.Inst_code = inst_code;
            model.Hospital = inst_code;
            model.Fund = fund;
            model.Section = section;
            model.Analytical = analytical;
            model.Don_inc_exp = don_inc_exp;
            model.Link_diff_type = "N";
            model.Trust = 0;
            model.Don_type = "2";
            model.Don_YTD = 0;
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2");//"20171101"; 
            var cal_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Cal_date"); //"20171101"; 
            model.Action = string.IsNullOrEmpty(action)?"":action;
            if (model.Action == "link" && don_inc_exp != odon_inc_exp)
            {
                model.Link_diff_type = "Y";
                model.Link_id = id;
                //model.Id = 0;
            }
            if (model.Action == "link" && oid != 0 && don_inc_exp == odon_inc_exp)
                model.Link = "Y";
            if (model.Link == "Y")
                DonationRecNExpService.DonationLinkRecord(id, oid);

            if (model.Id != 0)
            { 
                var detail = DonationRecNExpService.GetDonationDetailById(model.Id);
                if (detail != null)
                {
                    model.Hospital = detail.Hospital;
                    model.Inst_code = model.Hospital;
                    model.Section = detail.Section;
                    model.Donor_type = "D";
                    model.Donor_name = detail.DonorName.Replace("'","''");
                    model.ODonor_id = detail.DonorId;
                    model.Link_ind = detail.LinkId;
                    if (action != "link")
                        model.Don_inc_exp = (detail.DonIncExp??"").Trim();
                    model.Input_at_val = detail.InputAt;
                    model.Don_type = detail.DonType;
                    model.Don_purpose = detail.DonPurpose;
                    model.Don_supercat = detail.DonSupercat;
                    model.Don_cat = detail.DonCat;
                    model.Don_subcat = detail.DonSubcat;
                    model.Don_subsubcat = detail.DonSubsubcat;
                    model.Don_specific = detail.DonSpecific;
                    model.Eqt_desc = detail.EqtDesc;
                    model.Recurrent_con = (detail.RecurrentCon??"").Trim();
                    model.Recurrent_cost = detail.RecurrentCost;
                    model.Don_kind_desc = detail.DonKindDesc;//.Replace("'", "\'");
                    model.Maj_don1 = detail.MajDon1;
                    model.Maj_don2 = detail.MajDon2;
                    model.Maj_don3 = detail.MajDon3;
                    model.Reimb = detail.Reimb;
                }
                else
                    model.Id = 0;
            }
            if (model.Id != 0)
            {
                model.BalForward = DonationRecNExpService.GetBalForward(model.Id, financial_year);
            }
            else
            {
                model.Hospital = inst_code;
                model.Donor_type = "D";
                model.Don_cat = 1;
                model.Don_subcat = 1;
                model.Don_subsubcat = 0;
                model.Recurrent_cost = 0;
            }
            model.Exist_record_cnt = DonationRecNExpService.GetExistListCount(model.Inst_code, model.Fund, model.Analytical, model.Section, financial_year, financial_year, value_date, model.Id);
            model.Link_rec = DonationRecNExpService.GetExistList(model.Inst_code, model.Fund, model.Analytical, model.Section, financial_year, financial_year, value_date, model.Id, model.Don_inc_exp);
            model.CM_per_GL = DonationRecNExpService.GetCMperGL(model.Hospital, model.Fund, model.Section, model.Analytical, value_date, model.Don_inc_exp);
            model.CM_Input = DonationRecNExpService.GetCMInput(model.Hospital, model.Fund, model.Section, model.Analytical, value_date, model.Don_inc_exp, model.Id);
           
            model.CM_record = DonationRecNExpService.GetPreviousRecord(model.Id, financial_year, true, value_date, cal_date);
            model.Previous_record = DonationRecNExpService.GetPreviousRecord(model.Id, financial_year, false, value_date, cal_date);
            model.CM_record_cnt = DonationRecNExpService.GetPreviousRecordCount(model.Id, financial_year, true, value_date, cal_date);
            model.Previous_record_cnt = DonationRecNExpService.GetPreviousRecordCount(model.Id, financial_year, false, value_date, cal_date);
            ViewBag.CM_record_cnt = model.CM_record_cnt;
            ViewBag.Previous_record_cnt = model.Previous_record_cnt;
            if (model.Action == "link" && don_inc_exp != odon_inc_exp)
            {
                ViewBag.CM_record_cnt = 0;
                ViewBag.Previous_record_cnt = 0;
            }

            //Refund
            ViewBag.Refund = DonationRecNExpService.GetRefund(model.Id, financial_year);
            model.Refund = DonationRecNExpService.GetRefund(model.Id, financial_year,"");
            ViewBag.NotEditable = 0;
            
            ViewBag.Reserve_Bal_Alert = DonationRecNExpService.GetBalAlert(model.Hospital, model.Fund, model.Section, model.Analytical, value_date);
            GetDonationRecNExpFormControl(model, financial_year);
            if (model.Link_diff_type == "Y")
                model.Id = 0;
            return View(model);
        }

        [HttpPost]
        public IActionResult Input(DonationRecNExpModel model)
        {
            Session.SetString("current_sys", "D");
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] ?? ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? ViewBag.ErrorMessage;
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"));
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D"), false);

            var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
            var value_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2");//"20171101"; 
            var cal_date = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Cal_date"); //"20171101"; 
            model.Link_diff_type = "N";
            model.Trust = 0;
            model.Don_type = "2";
            model.Don_YTD = 0;
            if (model.Action == "link" && model.Don_inc_exp != model.ODon_inc_exp)
            {
                model.Link_diff_type = "Y";
                model.Link_id = model.Id;
                //model.Id = 0;
            }
            if (model.Action == "link" && model.OId != 0 && model.Don_inc_exp == model.ODon_inc_exp)
                model.Link = "Y";
            if (model.Link == "Y")
                DonationRecNExpService.DonationLinkRecord(model.Id, model.OId);

            if (model.Id != 0)
            {
                var detail = DonationRecNExpService.GetDonationDetailById(model.Id);
                if (detail != null)
                {
                    model.Hospital = detail.Hospital;
                    model.Inst_code = model.Hospital;
                    model.Section = detail.Section;
                    model.Donor_type = "D";
                    model.Donor_name = detail.DonorName.Replace("'", "''");
                    model.ODonor_id = detail.DonorId;
                    model.Link_ind = detail.LinkId;
                    if (model.Action != "link")
                        model.Don_inc_exp = (detail.DonIncExp ?? "").Trim();
                    model.Input_at_val = detail.InputAt;
                    model.Don_type = detail.DonType;
                    model.Don_purpose = detail.DonPurpose;
                    model.Don_supercat = detail.DonSupercat;
                    model.Don_cat = detail.DonCat;
                    model.Don_subcat = detail.DonSubcat;
                    model.Don_subsubcat = detail.DonSubsubcat;
                    model.Don_specific = detail.DonSpecific;
                    model.Eqt_desc = detail.EqtDesc;
                    model.Recurrent_con = (detail.RecurrentCon ?? "").Trim();
                    model.Recurrent_cost = detail.RecurrentCost;
                    model.Don_kind_desc = detail.DonKindDesc;//.Replace("'", "\'");
                    model.Maj_don1 = detail.MajDon1;
                    model.Maj_don2 = detail.MajDon2;
                    model.Maj_don3 = detail.MajDon3;
                    model.Reimb = detail.Reimb;
                }
                else
                    model.Id = 0;
            }
            if (model.Id != 0)
            {
                model.BalForward = DonationRecNExpService.GetBalForward(model.Id, financial_year);
            }
            else
            {
                model.Hospital = model.Inst_code;
                model.Donor_type = "D";
                model.Don_cat = 1;
                model.Don_subcat = 1;
                model.Don_subsubcat = 0;
                model.Recurrent_cost = 0;
            }
            model.Exist_record_cnt = DonationRecNExpService.GetExistListCount(model.Inst_code, model.Fund, model.Analytical, model.Section, financial_year, financial_year, value_date, model.Id);
            model.Link_rec = DonationRecNExpService.GetExistList(model.Inst_code, model.Fund, model.Analytical, model.Section, financial_year, financial_year, value_date, model.Id, model.Don_inc_exp);
            model.CM_per_GL = DonationRecNExpService.GetCMperGL(model.Hospital, model.Fund, model.Section, model.Analytical, value_date, model.Don_inc_exp);
            model.CM_Input = DonationRecNExpService.GetCMInput(model.Hospital, model.Fund, model.Section, model.Analytical, value_date, model.Don_inc_exp, model.Id);

            model.CM_record = DonationRecNExpService.GetPreviousRecord(model.Id, financial_year, true, value_date, cal_date);
            model.Previous_record = DonationRecNExpService.GetPreviousRecord(model.Id, financial_year, false, value_date, cal_date);
            model.CM_record_cnt = DonationRecNExpService.GetPreviousRecordCount(model.Id, financial_year, true, value_date, cal_date);
            model.Previous_record_cnt = DonationRecNExpService.GetPreviousRecordCount(model.Id, financial_year, false, value_date, cal_date);
            ViewBag.CM_record_cnt = model.CM_record_cnt;
            ViewBag.Previous_record_cnt = model.Previous_record_cnt;
            if (model.Action == "link" && "Model.Don_inc_exp" != "Model.Odon_inc_exp")
            {
                model.CM_record_cnt = 0;
                ViewBag.Previous_record_cnt = 0;
            }

            //Refund
            ViewBag.Refund = DonationRecNExpService.GetRefund(model.Id, financial_year);
            model.Refund = DonationRecNExpService.GetRefund(model.Id, financial_year, "");
            ViewBag.NotEditable = 0;

            ViewBag.Reserve_Bal_Alert = DonationRecNExpService.GetBalAlert(model.Hospital, model.Fund, model.Section, model.Analytical, value_date);
            GetDonationRecNExpFormControl(model, financial_year);
            if (model.Link_diff_type == "Y")
                model.Id = 0;
            return View(model);
        }

        [HttpPost]
        public IActionResult InsertUpdate(DonationRecNExpModel model)
        {
            try
            {
                var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
                var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
                var value_date = DateTime.ParseExact(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Value_Date2"), "yyyyMMdd", CultureInfo.GetCultureInfo("en-US")); 
                model.New_program = model.New_program ?? "";
                model.Exist_program = model.Exist_program ?? "";
                model.Don_kind_desc = string.IsNullOrEmpty(model.New_program) ? HttpUtility.HtmlDecode(model.Exist_program).Replace("'", "\'") : model.New_program.Replace("'", "\'");
                model.Don_case1 = string.IsNullOrEmpty(model.Don_case1) ? "N" : model.Don_case1;
                model.Don_case2 = string.IsNullOrEmpty(model.Don_case2) ? "N" : model.Don_case2;
                model.Don_case3 = string.IsNullOrEmpty(model.Don_case3) ? "N" : model.Don_case3;
                model.Reimb = string.IsNullOrEmpty(model.Reimb) ? "N" : model.Reimb;
                model.Trust = model.Fund == "52" ? (Int16)1 : (Int16)0;
                if (model.Don_cat != 3)
                {
                    model.Eqt_desc = null;
                    model.Recurrent_con = "N";
                }
                model.Don_cat = model.Don_cat == 0 ? 12 : model.Don_cat;
                model.Recurrent_cost = model.Recurrent_con == "N" ? 0 : model.Recurrent_cost;
                model.Out_comm = model.Don_inc_exp == "E" ? 0 : model.Out_comm;

                if (model.Delete_function == 1 && model.Id != 0)
                {
                    DonationRecNExpService.DeleteDonation(login_id, model.Id);
                }
                else
                {
                    model.Input_month = Convert.ToInt32(20 + model.Don_date.Substring(7, 2) + model.Don_date.Substring(4, 2) + model.Don_date.Substring(1, 2) + "");
                    if (model.Id == 0 || model.Link_diff_type == "Y" && model.OId == 0)
                    {
                        DonationRecNExpService.InsertNewDonation(model, login_id);
                        if (model.Link_diff_type != "Y")
                            DonationRecNExpService.UpdateDonationReserve(value_date, model.Inst_code, 0, login_id);
                        else
                            DonationRecNExpService.UpdateDonationReserve(value_date, model.Inst_code, model.Id, login_id);
                    }
                    else
                    {
                        if (model.Link_diff_type != "Y")
                        {
                            DonationRecNExpService.DeleteDonationHistory(model.Id, financial_year);
                        }
                        if (model.Id != 0 && model.Link_diff_type == "Y" && model.OId != 0)
                            model.Id = model.OId;
                        DonationRecNExpService.UpdateDonationDetail(model, login_id);
                        DonationRecNExpService.UpdateDonationReserve(value_date, model.Inst_code, model.Id, login_id);
                    }
                }

                if (model.Link_id != null)
                    DonationRecNExpService.UpdateDonationLinkID(model.OId, model.Link_id??0);

                TempData["SuccessMessage"] = Resource.SaveSuccess;
                return RedirectToAction("Index", new { inst_code = model.Inst_code });
            }
            catch (Exception ex)
            {
                    TempData["ErrorMessage"] = Resource.SaveFail + ex;
                    return RedirectToAction("Index", new { inst_code = model.Inst_code });
            }
        }

        [HttpPost]
        public IActionResult UpdateDonationRefund(DonationRecNExpModel model)
        {
            try
            {
                var financial_year = Convert.ToInt32(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Financial_Year"));
                 var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
                DonationRecNExpService.UpdateDonationRefund(model.Id, financial_year, "", model.Refund.Refund_Detail_Cnt, login_id, model.Refund.Refund_Detail);
                TempData["SuccessMessage"] = "Update Donation Success";
                return RedirectToAction("Input", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex;
                return RedirectToAction("Input", model);
            }   
        }

        #region Drop Down List Data 
        [HttpGet]
        public IEnumerable<SelectListItem> GetDonCatBySuperCatId(int don_supercat)
        {
            try
            {
                List<SelectListItem> list = DonationRecNExpService.GetDonCatBySuperCatId(don_supercat, false);
                return list;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resource.SaveFail + ex;
                return new List<SelectListItem>();
            }

        }
        [HttpGet]
        public IEnumerable<SelectListItem> GetDonSubCatByCatId(int don_cat)
        {
            try
            {
                List<SelectListItem> list = DonationRecNExpService.GetDonSubcatByCatId(don_cat, false);
                return list;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resource.SaveFail + ex;
                return new List<SelectListItem>();
            }

        }
        [HttpGet]
        public IEnumerable<SelectListItem> GetDonSubSubCatBySubCatId(int don_subcat)
        {
            try
            {
                List<SelectListItem> list = DonationRecNExpService.GetDonSubsubCatBySubCatId(don_subcat, false);
                return list;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resource.SaveFail + ex;
                return new List<SelectListItem>();
            }

        }
        #endregion

        private void GetDonationRecNExpFormControl(DonationRecNExpModel model, int financial_year)
        {
            ViewBag.DonType = DonationRecNExpService.GetDonTypeList(false);
            ViewBag.DonDonor = DonationRecNExpService.GetDonDonorList();
            ViewBag.DonDesc = DonationRecNExpService.GetDonDescList(model.Hospital, financial_year);
            ViewBag.DonPurpose = DonationRecNExpService.GetDonPurposeList(false);
            ViewBag.DonSuperCat = DonationRecNExpService.GetDonSuperCat();
            ViewBag.DonCat = DonationRecNExpService.GetDonCatBySuperCatId(model.Don_supercat, false);
            ViewBag.DonSubCat = DonationRecNExpService.GetDonSubcatByCatId(model.Don_cat, false);
            ViewBag.DonSubsubCat = DonationRecNExpService.GetDonSubsubCatBySubCatId(model.Don_subcat, false);
        }

    }
}