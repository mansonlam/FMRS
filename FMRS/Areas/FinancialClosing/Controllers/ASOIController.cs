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
using FMRS.Common.Resources;
using System.Globalization;

namespace FMRS.Areas.FinancialClosing.Controllers
{
    [Authorize]
    [Area("FinancialClosing")]
    [Route("[controller]/[action]")]
    public class ASOIController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;
        private IASOIService ASOIService;
        public ASOIController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IASOIService _aSOIService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            ASOIService = _aSOIService;
        }

        [HttpGet]
        public IActionResult Index(string cat_no, string hosp_code, string search_corp = "",string sch_analytical_start = "", string sch_analytical_end = "")
        {
            pageSetUp();
            
            sch_analytical_start = sch_analytical_start == "" ? "00000" : sch_analytical_start;
            sch_analytical_end = sch_analytical_end == "" ? "ZZZZZ" : sch_analytical_end;

            ASOISearchModel model = new ASOISearchModel();
            model.Cat_no = cat_no;
            model.Hosp_code = hosp_code;
            model.Sch_analytical_start = sch_analytical_start;
            model.Sch_analytical_end = sch_analytical_end;
            ASOISearchModelValueSetUp(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ASOISearchModel model)
        {
            pageSetUp();

            if (!string.IsNullOrEmpty(model.Sch_cat_no))
            { model.Cat_no = model.Sch_cat_no; }
            if (!string.IsNullOrEmpty(model.Sch_description_location))
                model.Sch_description_location = model.Sch_description_location.Replace("\"\"", "&quot;");
            if (!string.IsNullOrEmpty(model.Sch_organizer_department))
                model.Sch_organizer_department = model.Sch_organizer_department.Replace("\"\"", "&quot;");
            model.Sch_analytical_start = model.Sch_analytical_start == "" ? "00000" : model.Sch_analytical_start;
            model.Sch_analytical_end = model.Sch_analytical_end == "" ? "ZZZZZ" : model.Sch_analytical_end;

            ASOISearchModelValueSetUp(model);
            model.Result_list = ASOIService.GetASOIResultList(model);
            return View(model);
        }

        [HttpGet]
        public IActionResult ASOICatEdit(string hosp_code, string list_id, string sch_cat_no, string sch_analytical_start = "", string sch_analytical_end = "",
            string sch_section = "", string sch_description_location = "", string sch_nature_income = "", string sch_organizer_department = "", string sch_start_date_begin = "",
            string sch_start_date_until = "", string sch_service_provided = "", string sch_program_code = "", string sch_prog_subcat = "", string sch_allcat = "",
            string analytical_edit = "", string section_edit = "", string Duplicate = "")
        {
            pageSetUp();
            ASOIModel model = new ASOIModel();
            model.Sch = new ASOISearchModel();
            model.Detail = new ASOIResultModel();
            model.Sch.Hosp_code = hosp_code;
            model.Detail.Hosp_code = hosp_code;
            model.List_id = list_id;
            model.Analytical_edit = analytical_edit;
            model.Section_edit = section_edit;
            model.Duplicate = Duplicate;
            model.Sch.Sch_cat_no = sch_cat_no;
            model.Sch.Sch_allcat = sch_allcat;
            model.Sch.Sch_analytical_start = sch_analytical_start;
            model.Sch.Sch_analytical_end = sch_analytical_end;
            model.Sch.Sch_section = sch_section;
            if(!string.IsNullOrEmpty(sch_description_location))
                model.Sch.Sch_description_location = sch_description_location.Replace("\"\"", "&quot;"); 
            model.Sch.Sch_nature_income = sch_nature_income;
            if (!string.IsNullOrEmpty(sch_organizer_department))
                model.Sch.Sch_organizer_department = sch_organizer_department.Replace("\"\"", "&quot;"); 
            model.Sch.Sch_service_provided = sch_service_provided;
            model.Sch.Sch_start_date_begin = sch_start_date_begin;
            model.Sch.Sch_start_date_until = sch_start_date_until;
            model.Sch.Sch_program_code = sch_program_code;
            model.Sch.Sch_prog_subcat = sch_prog_subcat;
            model.Sch.Cat_no = ASOIService.GetCatNoById(list_id);
            model.Detail.Cat = model.Sch.Cat_no;
            model.Sch.Cat_name = ASOIService.GetCatName(model.Sch.Cat_no);
            model.Detail.Cat_name = model.Sch.Cat_name;
            model.Sch.Period = ASOIService.GetPeriod();
            model.Detail.Period = model.Sch.Period;
            model.Detail = ASOIService.GetASOIResult(model);

            ViewBag.SubCatByCatNo = ASOIService.GetSubCatByCatNo(model.Detail.Cat, "(" + Resource.PleaseSelect + ")");
            ViewBag.NatureIncomeByCatNo = ASOIService.GetNatureIncomeByCatNo(model.Detail.Cat, "(" + Resource.PleaseSelect + ")");
            ViewBag.ServiceProvidedByCatNo = ASOIService.GetServiceProvidedByCatNo(model.Detail.Cat, "(" + Resource.PleaseSelect + ")");
            ViewBag.PeriodNote = ASOIService.GetPeriodFromERPGL();
            return View(model);
        }

        [HttpGet]
        public IActionResult ASOICatNew(string cat_no, string hosp_code, string analytical = "", string section = "", string program_code = "", 
            string no_project = "", string prog_sub_cat = "", string prog_desc = "", string prog_organizer = "", string service_provided = "", 
            string contract_signed  = "", string nature_income = "", string roll_over = "", string cyp_income = "", string cyp_pe = "", 
            string cyp_oc = "", string poa_income = "", string poa_pe = "", string poa_oc = "", string remarks = "", string start_date = "", 
            string end_date = "", string Duplicate="")
        {
            pageSetUp();
            ViewBag.SubCatByCatNo = ASOIService.GetSubCatByCatNo(cat_no, "(" + Resource.PleaseSelect+")");
            ViewBag.NatureIncomeByCatNo = ASOIService.GetNatureIncomeByCatNo(cat_no, "(" + Resource.PleaseSelect + ")");
            ViewBag.ServiceProvidedByCatNo = ASOIService.GetServiceProvidedByCatNo(cat_no, "(" + Resource.PleaseSelect + ")");
            ASOIModel model = new ASOIModel();
            model.Detail = new ASOIResultModel();
            model.Detail.Cat = cat_no;
            model.Detail.Hosp_code = hosp_code;
            model.Detail.Cat_name = ASOIService.GetCatName(cat_no);
            model.Detail.Period = ASOIService.GetPeriod();
            model.Detail.Analytical = analytical;
            model.Detail.Section = section;
            model.Detail.Program_code = program_code;
            model.Detail.No_project = no_project;
            model.Detail.Prog_sub_cat = prog_sub_cat;
            model.Detail.Prog_desc = prog_desc;
            model.Detail.Prog_organizer = prog_organizer;
            model.Detail.Service_provided = service_provided;
            model.Detail.Contract_signed = contract_signed;
            model.Detail.Nature_income = nature_income;
            model.Detail.Roll_over = roll_over;
            model.Detail.Cyp_income = Convert.ToDecimal(cyp_income);
            model.Detail.Cyp_pe = Convert.ToDecimal(cyp_pe);
            model.Detail.Cyp_oc = Convert.ToDecimal(cyp_oc);
            model.Detail.Poa_income = Convert.ToDecimal(poa_income);
            model.Detail.Poa_pe = Convert.ToDecimal(poa_pe);
            model.Detail.Poa_oc = Convert.ToDecimal(poa_oc);
            model.Detail.Remarks = remarks;
            model.Detail.Start_date = start_date;
            model.Detail.End_date = end_date;
            model.Duplicate = Duplicate;

            return View(model);
        }

        [HttpPost]
        public IActionResult ASOICatUpdate(ASOIModel model)
        {
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            int duplicate_cnt = 0;
            if (model.Update_type == "new")
            {
                //check whether the record is duplicated
                if (model.Detail.Analytical != "ZZZZZ" && model.Detail.Section != "ZZZZZZZ")
                {
                    model.Detail.Period = ASOIService.GetPeriodShortMth();
                    int duplicate = ASOIService.CheckIfRecordExist(model.Detail.Analytical, model.Detail.Section, model.Detail.Hosp_code, model.Detail.Period);
                    duplicate_cnt = duplicate;
                    if (duplicate > 0)
                    {
                        return RedirectToAction("ASOICatNew", new
                        {
                            cat_no = model.Detail.Cat,
                            hosp_code = model.Detail.Hosp_code,
                            analytical = model.Detail.Analytical,
                            section = model.Detail.Section,
                            program_code = model.Detail.Program_code,
                            no_project = model.Detail.No_project,
                            prog_sub_cat = model.Detail.Prog_sub_cat,
                            prog_desc = model.Detail.Prog_desc,
                            prog_organizer = model.Detail.Prog_organizer,
                            service_provided = model.Detail.Service_provided,
                            contract_signed = model.Detail.Contract_signed,
                            nature_income = model.Detail.Nature_income,
                            roll_over = model.Detail.Roll_over,
                            cyp_income = model.Detail.Cyp_income,
                            cyp_pe = model.Detail.Cyp_pe,
                            cyp_oc = model.Detail.Cyp_oc,
                            poa_income = model.Detail.Poa_income,
                            poa_pe = model.Detail.Poa_pe,
                            poa_oc = model.Detail.Poa_oc,
                            remarks = model.Detail.Remarks,
                            start_date = model.Detail.Start_date,
                            end_date = model.Detail.End_date,
                            Duplicate = "true"
                        });
                    }
                }
                var new_id = ASOIService.GenNewRecordId();
                ASOIService.InsertAsASOIProgrammesRecord(new_id, model.Detail, login_id);

            }
            else if (model.Update_type == "edit")
            {
                //check whether the record is duplicated
                if (model.Detail.Analytical != "ZZZZZ" && model.Detail.Section != "ZZZZZZZ")
                {
                    model.Detail.Period = ASOIService.GetPeriodShortMth();
                    int duplicateRecord_Edit = ASOIService.CheckIfRecordExist(model.Detail.Analytical, model.Detail.Section, model.Detail.Hosp_code, model.Detail.Period, model.List_id);
                    duplicate_cnt = duplicateRecord_Edit;
                    if (duplicateRecord_Edit > 0)
                    {
                        return RedirectToAction("ASOICatEdit", new
                        {
                            hosp_code = model.Detail.Hosp_code,
                            list_id = model.List_id,
                            sch_cat_no = model.Detail.Cat,
                            analytical_edit = model.Detail.Analytical,
                            section_edit = model.Detail.Section,
                            Duplicate = "true"
                        });
                    }
                }
                ASOIService.UpdateAsASOIProgrammesRecord(model.List_id, model.Detail, login_id);
            }
            else if (model.Update_type == "delete")
            {
                ASOIService.DeleteAsASOIProgrammesRecordById(model.List_id);
            }

            if(model.Update_type == "new" || model.Update_type == "edit")
            {
                model.Detail.Period = ASOIService.GetPeriodShortMth();
                ASOIService.AsUpdateFromAsGL(model.Detail.Period, model.Detail.Hosp_code, model.Detail.Analytical, model.Detail.Section);
            }

            if (model.Update_type == "edit" && model.Submit_type == "UpdateYTD")
            {
                return RedirectToAction("ASOICatEdit", new
                {
                    hosp_code = model.Detail.Hosp_code,
                    list_id = model.List_id,
                    sch_cat_no = model.Detail.Cat
                });
            }
            else if (model.Update_type == "edit" && model.Submit_type == "SaveNComplete")
            {
                return RedirectToAction("Index", new
                {
                    cat_no = model.Detail.Cat,
                    hosp_code = model.Detail.Hosp_code,
                    //list_id = model.List_id,
                    search_corp = "search"
                });
            }
            else if (model.Update_type == "new" && duplicate_cnt == 0)
            {
                return RedirectToAction("ASOICatEdit", new
                {
                    hosp_code = model.Detail.Hosp_code,
                    list_id = model.List_id,
                    sch_cat_no = model.Detail.Cat
                });
            }
            else if (model.Update_type == "delete")
            {
                return RedirectToAction("Index", new
                {
                    cat_no = model.Detail.Cat,
                    hosp_code = model.Detail.Hosp_code,
                });
            }
            return RedirectToAction("Index", new { cat_no = model.Detail.Cat, hosp_code = model.Detail.Hosp_code });
        }

        [HttpGet]
        public IActionResult ASOICatDetail(string rp_id)
        {
            pageSetUp();
            ASOIModel model = new ASOIModel();
            model.Detail = new ASOIResultModel();
            model.Sch = new ASOISearchModel();
            model.List_id = rp_id;
            int count_rp = ASOIService.CountDetailASOI(rp_id);
            if (count_rp == 0 && ViewBag.PrivilegeAsoiInput == "I")
            {
                return RedirectToAction("ASOICatDetailNew", new { rp_id = rp_id });
            }
            else {
                model.Detail = ASOIService.GetModelDetail(model.Detail, rp_id);
                if (count_rp != 0)
                {
                    model.Detail.Detail_list = ASOIService.GetDetailList(rp_id);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ASOICatDetailNew(string rp_id)
        {
            pageSetUp();
            ASOIModel model = new ASOIModel();
            model.Detail = new ASOIResultModel();
            model.Sch = new ASOISearchModel();
            model.List_id = rp_id;
            model.Detail = ASOIService.GetModelDetail(model.Detail, rp_id);
            int count_rp = ASOIService.CountDetailASOI(rp_id);
            if (count_rp != 0)
            {
                model.Detail.Detail_list = ASOIService.GetDetailList(rp_id);
            }
            //model.Detail.Detail_list.AddRange(new SubList[10]);
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.Detail_list.Add(new SubList());
            model.Detail.CountRp = count_rp;
            return View(model);
        }

        [HttpGet]
        public IActionResult ASOICatDetailEdit(string rp_id)
        {
            pageSetUp();
            ASOIModel model = new ASOIModel();
            model.Detail = new ASOIResultModel();
            model.Sch = new ASOISearchModel();
            model.List_id = rp_id;
            model.Detail = ASOIService.GetModelDetail(model.Detail, rp_id);
            int count_rp = ASOIService.CountDetailASOI(rp_id);
            model.Detail.CountRp = count_rp;
            model.Detail.Detail_list = ASOIService.GetDetailList(rp_id);
            return View(model);
        }

        [HttpPost]
        public IActionResult ASOICatDetailUpdate(ASOIModel model)
        {
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            if (model.Update_type == "new")
            {
                for (int i = model.Detail.CountRp; i < model.Detail.CountRp + 10; i++)
                {
                    if (!string.IsNullOrEmpty(model.Detail.Detail_list[i].Prog_desc) || !string.IsNullOrEmpty(model.Detail.Detail_list[i].Prog_organizer)||
                        !string.IsNullOrEmpty(model.Detail.Detail_list[i].Start_date) || !string.IsNullOrEmpty(model.Detail.Detail_list[i].End_date) ||
                        model.Detail.Detail_list[i].Income != 0 || model.Detail.Detail_list[i].Pe != 0 ||
                        model.Detail.Detail_list[i].Oc != 0 || !string.IsNullOrEmpty(model.Detail.Detail_list[i].Remarks))
                    {
                        string st_dt = model.Detail.Detail_list[i].Start_date;
                        if (!(string.IsNullOrEmpty(st_dt)))
                        {
                            model.Detail.Detail_list[i].Start_date = DateTime.ParseExact(st_dt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }
                        string end_dt = model.Detail.Detail_list[i].End_date;
                        if (!(string.IsNullOrEmpty(end_dt)))
                        {
                            model.Detail.Detail_list[i].End_date = DateTime.ParseExact(end_dt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }
                        model.Detail.Detail_list[i].Prog_desc = string.IsNullOrEmpty(model.Detail.Detail_list[i].Prog_desc) ? "": model.Detail.Detail_list[i].Prog_desc.Replace("'", "''");
                        model.Detail.Detail_list[i].Prog_organizer = string.IsNullOrEmpty(model.Detail.Detail_list[i].Prog_organizer) ? "" : model.Detail.Detail_list[i].Prog_organizer.Replace("'", "''");
                        model.Detail.Detail_list[i].Remarks = string.IsNullOrEmpty(model.Detail.Detail_list[i].Remarks) ? "" : model.Detail.Detail_list[i].Remarks.Replace("'", "''");
                        model.Detail.Detail_list[i].Rp_id = model.List_id;
                        ASOIService.InsertAsDetailASOI(model.Detail.Detail_list[i], login_id);
                    }
                }
            }
            else if (model.Update_type == "edit")
            {
                for (int i = 0; i < model.Detail.CountRp; i++)
                {
                    if (model.Detail.Detail_list[i].Id != "")
                    {
                        string st_dt = model.Detail.Detail_list[i].Start_date;
                        if (st_dt != "")
                        {
                            model.Detail.Detail_list[i].Start_date = DateTime.ParseExact(st_dt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }
                        string end_dt = model.Detail.Detail_list[i].End_date;
                        if (end_dt != "")
                        {
                            model.Detail.Detail_list[i].End_date = DateTime.ParseExact(end_dt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        }
                        model.Detail.Detail_list[i].Prog_desc = string.IsNullOrEmpty(model.Detail.Detail_list[i].Prog_desc) ? "" : model.Detail.Detail_list[i].Prog_desc.Replace("'", "''");
                        model.Detail.Detail_list[i].Prog_organizer = string.IsNullOrEmpty(model.Detail.Detail_list[i].Prog_organizer) ? "" : model.Detail.Detail_list[i].Prog_organizer.Replace("'", "''");
                        model.Detail.Detail_list[i].Remarks = string.IsNullOrEmpty(model.Detail.Detail_list[i].Remarks) ? "" : model.Detail.Detail_list[i].Remarks.Replace("'", "''");

                        ASOIService.UpdateAsDetailASOI(model.Detail.Detail_list[i], login_id);
                    }
                }
            }

            return RedirectToAction("ASOICatDetail", new { rp_id = model.List_id });
        }

        [HttpGet]
        public IActionResult ASOICatDetailDelete(string id)
        {
            var rp_id = ASOIService.GetDetailASOIById(id).Rp_id; 
            ASOIService.DeleteAsDetailASOIById(id);
            return RedirectToAction("ASOICatDetail", new { rp_id = rp_id });
        }

        private void pageSetUp()
        {
            Session.SetString("current_sys", "Y");
            ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_Y"), false);
            ViewBag.PrivilegeAsoiInput = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Asoi_Input");
            
        }

        private ASOISearchModel ASOISearchModelValueSetUp(ASOISearchModel model)
        {
            ViewBag.SubCatByCatNo = ASOIService.GetSubCatByCatNo(model.Cat_no, Resource.Any);
            ViewBag.NatureIncomeByCatNo = ASOIService.GetNatureIncomeByCatNo(model.Cat_no, Resource.Any);
            ViewBag.ServiceProvidedByCatNo = ASOIService.GetServiceProvidedByCatNo(model.Cat_no, Resource.Any);

            model.Subcat_check = ASOIService.GetSubcatCheck(model.Cat_no);
            model.Nat_inc_check = ASOIService.GetNatIncCheck(model.Cat_no);
            model.Ser_prov_check = ASOIService.GetSerProvCheck(model.Cat_no);
            model.Cat_name = ASOIService.GetCatName(model.Cat_no);
            model.Get_max_period = ASOIService.GetMaxPeriod();
            model.Period = ASOIService.GetPeriod();
            return model;
        }
    }
}