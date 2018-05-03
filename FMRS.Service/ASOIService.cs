using FMRS.Common.Resources;
using FMRS.DAL.Repository;
using FMRS.Model.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FMRS.Service
{
    public interface IASOIService
    {
        string GetSubcatCheck(string cat_no);
        string GetNatIncCheck(string cat_no);
        string GetSerProvCheck(string cat_no);
        string GetCatName(string cat_no);
        string GetMaxPeriod();
        string GetPeriod();
        string GetPeriodShortMth();
        List<SelectListItem> GetSubCatByCatNo(string cat_no, string empty_text);
        List<SelectListItem> GetNatureIncomeByCatNo(string cat_no, string empty_text);
        List<SelectListItem> GetServiceProvidedByCatNo(string cat_no, string empty_text);
        List<ASOIResultModel> GetASOIResultList(ASOISearchModel model);
        string GetCatNoById(string list_Id);
        ASOIResultModel GetASOIResult(ASOIModel model);
        string GetPeriodFromERPGL();
        int CheckIfRecordExist(string analytical, string section, string hosp_code, string period, string list_id = "");
        int GenNewRecordId();

    #region ASOI Update
        void InsertAsASOIProgrammesRecord(int id, ASOIResultModel model, string user_id);
        void UpdateAsASOIProgrammesRecord(string list_id, ASOIResultModel model, string user_id);
        void DeleteAsASOIProgrammesRecordById(string id);
        void AsUpdateFromAsGL(string Period, string hosp_code, string analytical, string section);
        #endregion

        #region ASOI Detail
        int CountDetailASOI(string rp_id);
        ASOIResultModel GetModelDetail(ASOIResultModel model, string rp_id);
        List<SubList> GetDetailList(string rp_id);
        #endregion

        #region ASOI Detail Update
        SubList GetDetailASOIById(string id);
        void DeleteAsDetailASOIById(string id);
        int CountTotalDetailASOI();
        void InsertAsDetailASOI(SubList model, string user_name);
        void UpdateAsDetailASOI(SubList model, string user_name);
        #endregion
    }
    public class ASOIService : IASOIService
    {
        private IAsProgSubcatRepository AsProgSubcatRepository;
        private IAsNatureIncomeRepository AsNatureIncomeRepository;
        private IAsServiceProvidedRepository AsServiceProvidedRepository;
        private IAsCatInfoRepository AsCatInfoRepository;
        private IAsASOIProgrammesRepository AsASOIProgrammesRepository;
        private IAsGLRepository AsGLRepository;
        private IAsDetailSAOIRepository AsDetailSAOIRepository;
        public ASOIService(IAsProgSubcatRepository _asProgSubcatRepository, IAsNatureIncomeRepository _asNatureIncomeRepository,
                            IAsServiceProvidedRepository _asServiceProvidedRepository, IAsCatInfoRepository _asCatInfoRepository,
                            IAsASOIProgrammesRepository _asASOIProgrammesRepository, IAsGLRepository _asGLRepository,
                            IAsDetailSAOIRepository _asDetailSAOIRepository)
        {
            AsProgSubcatRepository = _asProgSubcatRepository;
            AsNatureIncomeRepository = _asNatureIncomeRepository;
            AsServiceProvidedRepository = _asServiceProvidedRepository;
            AsCatInfoRepository = _asCatInfoRepository;
            AsASOIProgrammesRepository = _asASOIProgrammesRepository;
            AsGLRepository = _asGLRepository;
            AsDetailSAOIRepository = _asDetailSAOIRepository;
        }


        public string GetSubcatCheck(string cat_no)
        {
            return AsProgSubcatRepository.GetSubcatCheck(cat_no);
        }

        public string GetNatIncCheck(string cat_no)
        {
            return AsNatureIncomeRepository.GetNatIncCheck(cat_no);
        }

        public string GetSerProvCheck(string cat_no)
        {
            return AsServiceProvidedRepository.GetSerProvCheck(cat_no);
        }

        public string GetCatName(string cat_no)
        {
            return AsCatInfoRepository.GetCatName(cat_no);
        }

        public string GetMaxPeriod()
        {
            return AsASOIProgrammesRepository.GetPeriod().ToString("yyyy-MM-dd");
        }

        public string GetPeriod()
        {
            return AsASOIProgrammesRepository.GetPeriod().ToString("dd MMMM yyyy");
        }
        public string GetPeriodShortMth()
        {
            return AsASOIProgrammesRepository.GetPeriod().ToString("yyyy-MM-dd");
        }
        public List<SelectListItem> GetSubCatByCatNo(string cat_no, string empty_text)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = AsProgSubcatRepository.GetSubCatByCatNo(cat_no);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                if (ds.Tables[0].Rows[0]["value"].ToString() == "none")
                {
                    result.Insert(0, new SelectListItem() { Value = "", Text = "(" + Resource.None + ")" });
                }
                else
                {
                    result.Insert(0, new SelectListItem() { Value = "", Text = empty_text });
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(new SelectListItem() { Value = dr["value"].ToString(), Text = dr["description"].ToString() });
                    }
                }
            }
            return result;
        }

        public List<SelectListItem> GetNatureIncomeByCatNo(string cat_no, string empty_text)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = AsNatureIncomeRepository.GetNatureIncomeByCatNo(cat_no);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                if (ds.Tables[0].Rows[0]["value"].ToString() == "")
                {
                    result.Insert(0, new SelectListItem() { Value = "", Text = "(" + Resource.None + ")" });
                }
                else
                {
                    result.Insert(0, new SelectListItem() { Value = "", Text = empty_text });
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(new SelectListItem() { Value = dr["id"].ToString(), Text = dr["value"].ToString() });
                    }
                }
            }
            return result;
        }

        public List<SelectListItem> GetServiceProvidedByCatNo(string cat_no, string empty_text)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = AsServiceProvidedRepository.GetServiceProvidedByCatNo(cat_no);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                if (ds.Tables[0].Rows[0]["value"].ToString() == "")
                {
                    result.Insert(0, new SelectListItem() { Value = "", Text = "(" + Resource.None + ")" });
                }
                else
                {
                    result.Insert(0, new SelectListItem() { Value = "", Text = empty_text });
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(new SelectListItem() { Value = dr["id"].ToString(), Text = dr["value"].ToString() });
                    }
                }
            }
            return result;
        }

        public List<ASOIResultModel> GetASOIResultList(ASOISearchModel model)
        {
            List<ASOIResultModel> result = new List<ASOIResultModel>();
            var ds = AsASOIProgrammesRepository.GetASOIResultList(model.Hosp_code, model.Get_max_period, model.Sch_allcat, model.Cat_no, model.Sch_start_date_begin,
                model.Sch_start_date_until, model.Sch_analytical_start, model.Sch_analytical_end,
                model.Sch_section, model.Sch_program_code, model.Sch_prog_subcat, model.Sch_description_location,
                model.Sch_organizer_department, model.Sch_nature_income, model.Sch_service_provided);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ASOIResultModel asoi = new ASOIResultModel();
                    asoi.Id = dr["id"].ToString();
                    asoi.Cat = dr["cat"].ToString();
                    asoi.Analytical = dr["analytical"].ToString();
                    asoi.Section = dr["section"].ToString();
                    asoi.Program_code = dr["program_code"].ToString();
                    asoi.Prog_sub_cat = dr["prog_sub_cat"].ToString();
                    asoi.Prog_desc = dr["prog_desc"].ToString();
                    asoi.Nature_income = dr["nature_income"].ToString();
                    asoi.Prog_organizer = dr["prog_organizer"].ToString();
                    asoi.Service_provided = dr["service_provided"].ToString();
                    asoi.Start_date = DateTime.Parse(dr["start_date"].ToString()).ToString("dd/MM/yyyy");
                    asoi.No_project = model.Cat_no != "10" ? dr["no_project"].ToString() : "";
                    if (model.Sch_allcat == "all" || model.Subcat_check != "none")
                    {
                        asoi.Prog_sub_cat = AsProgSubcatRepository.GetSubCatByValue(dr["prog_sub_cat"].ToString());
                    }
                    asoi.Nature_income = AsNatureIncomeRepository.GetNatureIncomeById(dr["nature_income"].ToString());
                    asoi.Service_provided = AsServiceProvidedRepository.GetServiceProvidedById(dr["service_provided"].ToString());
                    if (asoi.Start_date.ToString() == "01/01/1900")
                    {
                        asoi.Start_date = "";
                    }
                    result.Add(asoi);
                }
            }

            return result;
        }

        public string GetCatNoById(string list_Id)
        {
            return AsASOIProgrammesRepository.GetCatNoById(list_Id);
        }

        public ASOIResultModel GetASOIResult(ASOIModel model)
        {
            var ds = AsASOIProgrammesRepository.GetASOIResultById(model.List_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                model.Detail.Analytical = ds.Tables[0].Rows[0]["analytical"].ToString();
                model.Detail.Section = ds.Tables[0].Rows[0]["section"].ToString();
                model.Detail.Program_code = ds.Tables[0].Rows[0]["program_code"].ToString();
                model.Detail.No_project = ds.Tables[0].Rows[0]["no_project"].ToString();
                model.Detail.Prog_sub_cat = ds.Tables[0].Rows[0]["prog_sub_cat"].ToString();
                model.Detail.Prog_desc = ds.Tables[0].Rows[0]["prog_desc"].ToString().Replace("\"\"", "&quot;");
                model.Detail.Start_date = DateTime.Parse(ds.Tables[0].Rows[0]["start_date"].ToString()).ToString("dd/MM/yyyy");
                model.Detail.End_date = DateTime.Parse(ds.Tables[0].Rows[0]["end_date"].ToString()).ToString("dd/MM/yyyy");

                model.Detail.Prog_organizer = ds.Tables[0].Rows[0]["prog_organizer"].ToString().Replace("\"\"", "&quot;");
                model.Detail.Service_provided = ds.Tables[0].Rows[0]["service_provided"].ToString();
                model.Detail.Contract_signed = ds.Tables[0].Rows[0]["contract_signed"].ToString();
                model.Detail.Nature_income = ds.Tables[0].Rows[0]["nature_income"].ToString();
                model.Detail.Roll_over = ds.Tables[0].Rows[0]["roll_over"].ToString();
                model.Detail.Ytd_income = Convert.ToDecimal(ds.Tables[0].Rows[0]["ytd_income"].ToString());
                model.Detail.Ytd_pe = Convert.ToDecimal(ds.Tables[0].Rows[0]["ytd_pe"].ToString());
                model.Detail.Ytd_oc = Convert.ToDecimal(ds.Tables[0].Rows[0]["ytd_oc"].ToString());
                model.Detail.Cyp_income = Convert.ToDecimal(ds.Tables[0].Rows[0]["cyp_income"].ToString());
                model.Detail.Cyp_pe = Convert.ToDecimal(ds.Tables[0].Rows[0]["cyp_pe"].ToString());
                model.Detail.Cyp_oc = Convert.ToDecimal(ds.Tables[0].Rows[0]["cyp_oc"].ToString());
                model.Detail.Poa_income = Convert.ToDecimal(ds.Tables[0].Rows[0]["poa_income"].ToString());
                model.Detail.Poa_pe = Convert.ToDecimal(ds.Tables[0].Rows[0]["poa_pe"].ToString());
                model.Detail.Poa_oc = Convert.ToDecimal(ds.Tables[0].Rows[0]["poa_oc"].ToString());
                model.Detail.Remarks = ds.Tables[0].Rows[0]["remarks"].ToString().Replace("\"\"", "&quot;");

                model.Detail.Ytd_sd = model.Detail.Ytd_income + model.Detail.Ytd_pe + model.Detail.Ytd_oc;
                model.Detail.Cyp_sd = model.Detail.Cyp_income + model.Detail.Cyp_pe + model.Detail.Cyp_oc;
                model.Detail.Poa_sd = model.Detail.Poa_income + model.Detail.Poa_pe + model.Detail.Poa_oc;
                model.Detail.Remarks = ds.Tables[0].Rows[0]["remarks"].ToString();

                if (model.Detail.Start_date.ToString() == "01/01/1900")
                    model.Detail.Start_date = "";
                if (model.Detail.End_date.ToString() == "01/01/1900")
                    model.Detail.End_date = "";


            }
            return model.Detail;
        }

        public string GetPeriodFromERPGL()
        {
            return AsGLRepository.GetPeriodFromERPGL(); ;
        }

        public int CheckIfRecordExist(string analytical, string section, string hosp_code, string period, string list_id)
        {
            return AsASOIProgrammesRepository.CheckIfRecordExist(analytical, section, hosp_code, period, list_id);
        }

        public int GenNewRecordId()
        {
            int result = 0;
            int cnt = AsASOIProgrammesRepository.CntNumOfRecord();
            if (cnt == 0)
            {
                result = 1;
            }
            else
            {
                result = AsASOIProgrammesRepository.GetLastRecordId() + 1;
            }
            return result;
        }

        #region ASOI Update
        public void InsertAsASOIProgrammesRecord(int id, ASOIResultModel model, string user_id)
        {
            AsASOIProgrammesRepository.InsertAsASOIProgrammesRecord(id, model.Cat, model.Hosp_code, model.Analytical, model.Section, model.Program_code, 
                model.No_project, model.Prog_sub_cat, model.Prog_desc, model.Prog_organizer, model.Service_provided, model.Start_date, model.End_date,
            model.Contract_signed, model.Nature_income, model.Roll_over, model.Ytd_income.ToString(), model.Ytd_pe.ToString(), model.Ytd_oc.ToString(), model.Cyp_income.ToString(),
            model.Cyp_pe.ToString(), model.Cyp_oc.ToString(), model.Poa_income.ToString(), model.Poa_pe.ToString(), model.Poa_oc.ToString(), model.Remarks, model.Period, user_id);
        }

        public void UpdateAsASOIProgrammesRecord(string list_id, ASOIResultModel model, string user_id)
        {
            AsASOIProgrammesRepository.UpdateAsASOIProgrammesRecord(list_id, model.Analytical, model.Section, model.Program_code,
                model.No_project, model.Prog_sub_cat, model.Prog_desc, model.Prog_organizer, model.Service_provided, model.Start_date, model.End_date,
            model.Contract_signed, model.Nature_income, model.Roll_over, model.Ytd_income.ToString(), model.Ytd_pe.ToString(), model.Ytd_oc.ToString(), model.Cyp_income.ToString(),
            model.Cyp_pe.ToString(), model.Cyp_oc.ToString(), model.Poa_income.ToString(), model.Poa_pe.ToString(), model.Poa_oc.ToString(), model.Remarks, user_id);
        }

        public void DeleteAsASOIProgrammesRecordById(string id)
        {
            AsASOIProgrammesRepository.DeleteAsASOIProgrammesRecordById(id);
        }

        public void AsUpdateFromAsGL(string period, string hosp_code, string analytical, string section)
        {
            AsASOIProgrammesRepository.AsUpdateFromAsGL(period, hosp_code, analytical, section);
        }
        #endregion

        #region ASOI Detail
        public int CountDetailASOI(string rp_id)
        {
            return AsDetailSAOIRepository.CountDetailASOI(rp_id);
        }

        public ASOIResultModel GetModelDetail(ASOIResultModel model, string rp_id)
        {
            var ds = AsASOIProgrammesRepository.GetASOIResultById(rp_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                model.Cat = ds.Tables[0].Rows[0]["cat"].ToString();
                model.Hosp_code = ds.Tables[0].Rows[0]["hosp"].ToString();
                model.Analytical = ds.Tables[0].Rows[0]["analytical"].ToString();
                model.Section = ds.Tables[0].Rows[0]["section"].ToString();
            }
            model.Cat_name = AsCatInfoRepository.GetCatName(model.Cat);
            model.Period = GetPeriod();
            return model;
        }

        public List<SubList> GetDetailList(string rp_id)
        {
            List<SubList> result = new List<SubList>();
            var ds = AsDetailSAOIRepository.GetDetailASOIByRpId(rp_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SubList item = new SubList();
                    item.Id = dr["id"].ToString();
                    item.Rp_id = rp_id;
                    item.Prog_desc = dr["prog_desc"].ToString();
                    item.Prog_organizer = dr["prog_organizer"].ToString();
                    item.Start_date = DateTime.Parse(dr["start_date"].ToString()).ToString("dd/MM/yyyy"); 
                    item.End_date = DateTime.Parse(dr["end_date"].ToString()).ToString("dd/MM/yyyy");
                    item.Income = Convert.ToDecimal(dr["income"].ToString());
                    item.Pe = Convert.ToDecimal(dr["pe"].ToString());
                    item.Oc = Convert.ToDecimal(dr["oc"].ToString());
                    //item.Income = dr["income"].ToString();
                    //item.Pe = dr["pe"].ToString();
                    //item.Oc = dr["oc"].ToString();
                    item.Remarks = dr["remarks"].ToString();

                    if (item.Start_date.ToString() == "01/01/1900")
                        item.Start_date = "";
                    if (item.End_date.ToString() == "01/01/1900")
                        item.End_date = "";
                    
                    result.Add(item);
                }
            }
            return result;
        }

        #endregion

        #region ASOI Detail Update
        public SubList GetDetailASOIById(string id)
        {
            SubList result = new SubList();
            var ds = AsDetailSAOIRepository.GetDetailASOIById(id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {

                    result.Id = ds.Tables[0].Rows[0]["id"].ToString();
                    result.Rp_id = ds.Tables[0].Rows[0]["rp_id"].ToString();
                    result.Prog_desc = ds.Tables[0].Rows[0]["prog_desc"].ToString();
                    result.Prog_organizer = ds.Tables[0].Rows[0]["prog_organizer"].ToString();
                    result.Start_date = DateTime.Parse(ds.Tables[0].Rows[0]["start_date"].ToString()).ToString("dd/MM/yyyy");
                    result.End_date = DateTime.Parse(ds.Tables[0].Rows[0]["end_date"].ToString()).ToString("dd/MM/yyyy");
                    result.Income = Convert.ToDecimal(ds.Tables[0].Rows[0]["income"].ToString());
                    result.Pe = Convert.ToDecimal(ds.Tables[0].Rows[0]["pe"].ToString());
                    result.Oc = Convert.ToDecimal(ds.Tables[0].Rows[0]["oc"].ToString());
                    result.Remarks = ds.Tables[0].Rows[0]["remarks"].ToString();

                    if (result.Start_date.ToString() == "01/01/1900")
                        result.Start_date = "";
                    if (result.End_date.ToString() == "01/01/1900")
                        result.End_date = "";

            }
            return result;
        }

        public void DeleteAsDetailASOIById(string id)
        {
            AsDetailSAOIRepository.DeleteDetailASOIById(id);
        }

        public int CountTotalDetailASOI()
        {
            return AsDetailSAOIRepository.CountTotalDetailASOI();
        }

        public void InsertAsDetailASOI(SubList model, string user_name)
        {
            int totalDetailASOI = CountTotalDetailASOI();
            int id = 0;
            if (totalDetailASOI == 0)
            {
                id = 1;
            }
            else
            {
                id = AsDetailSAOIRepository.GetLatestDetailASOIId() + 1;
            }

            AsDetailSAOIRepository.InsertAsDetailASOI(id, model.Rp_id, model.Prog_desc, model.Prog_organizer, model.Start_date, 
                model.End_date, model.Income, model.Pe, model.Oc, model.Remarks, user_name);
        }

        public void UpdateAsDetailASOI(SubList model, string user_name)
        {
            AsDetailSAOIRepository.UpdateAsDetailASOI(Convert.ToInt32(model.Id), model.Rp_id, model.Prog_desc, model.Prog_organizer, model.Start_date,
                model.End_date, model.Income, model.Pe, model.Oc, model.Remarks, user_name);
        }
        #endregion
    }

}

