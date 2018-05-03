using FMRS.Common.Resources;
using FMRS.DAL.Repository;
using FMRS.Model.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FMRS.Service
{
    public interface IReportService
    {
        List<SelectListItem> GetReportNameList(string current_system, string user_inst_code, int cnt, List<ReportViewModel> report_detail_list, bool blankRow = true);
        List<SelectListItem> GetReportGpList(string current_system, string user_inst_code, int cnt, List<ReportViewModel> report_detail_list, bool blankRow = true);
        List<SelectListItem> GetFlashRptDateList(int financial_year, int actual_cnt, bool blankRow = true);
        List<SelectListItem> GetFlashRptDateList2(int financial_year, int actual_cnt, bool blankRow = true);
        List<SelectListItem> GetReportHospList(string login_id, string current_sys);
        List<SelectListItem> GetReportHospGpList(string user_group, string user_inst_code, string ho_access, string login_id, string current_sys, out Dictionary<string, string> consolidate_ind_dict);
        List<ReportViewModel> GetReportDetailList(string current_sys, string user_group, string user_inst_code, string login_id, string privilege_non_pjt_report,
            string privilege_asoi_rpt, string privilege_report, string privilege_closing_report, string ho_access, int report_period, string cluster_level, out int cnt);
        List<ReportGroupModel> GetRptGpFullList(string current_sys, string user_group, string user_inst_code, string login_id, string privilege_non_pjt_report,
            string privilege_asoi_rpt, string privilege_report, string privilege_closing_report, string ho_access, int report_period, string cluster_level);
        List<SelectListItem> GetSnapshotYr(bool blankRow = true);
        int GetSnapshotMaxYr();
        List<string> GetSnapshotDateListByMaxYr();
        List<SelectListItem> GetFVIndList(bool allRow = true);
        List<FVIndModel> GetFVIndTenderer_list();
        List<SelectListItem> GetFundGpList(bool allRow = true);
        List<SelectListItem> GetAsAtList(int financial_year, int actual_cnt, bool blankRow = true);
        List<SelectListItem> GetAsAtMList(int financial_year, int actual_cnt, bool blankRow = true);
        List<SelectListItem> GetAsAtYList(string value_date, bool blankRow = true);
        List<SelectListItem> GetDonSuperCat(bool blankRow = true);
        List<SelectListItem> GetHospClusterList_forM(string user_group, bool AllHAHORow = true);
        List<SelectListItem> GetBudgetClusterHospList(string user_inst_code, bool blankRow = true);
        List<SelectListItem> GetWorkAgentList();
        List<SelectListItem> GetBatchNoList();
        List<SelectListItem> GetProjItemIndList();
        int GetCbvAccessControlByUserNameCount(string login_id);
        int GetCwrfAccessControlByUserName(string login_id);
        List<RptItemModel> GetRtpItem_list(string user_group, string login_id);

        #region Generate Report
        int GetParmNo(string rpt_name);
        string GetCurrentPeriodByReportDate(string report_date);
        void InsertReportLog(string login_id, string report_type);
        GenReportModel GetReportParm(ReportViewModel model, string value_date, string login_id, int financial_year, string other_wk_agent, string p13, string year_end, string user_group, int current_year, string current_date, string period_end_date);
        #endregion

    }
    public class ReportService : IReportService
    {
        private IReportGroupDescRepository ReportGroupDescRepository;
        private IReportRepository ReportRepository;
        private IDonSupercatRepository DonSupercatRepository;
        private IUserGroupHospRespository UserGroupHospRespository;
        private IHospitalRepository HospitalRepository;
        private ICwrfAccessControlRepository CwrfAccessControlRepository;
        private IFinancialClosingRepository FinancialClosingRepository;

        public ReportService(IReportGroupDescRepository _reportGroupDescRepository, IReportRepository _reportRepository,
                              IDonSupercatRepository _donSupercatRepository, IUserGroupHospRespository _userGroupHospRespository,
                              IHospitalRepository _hospitalRepository, ICwrfAccessControlRepository _cwrfAccessControlRepository,
                              IFinancialClosingRepository _financialClosingRepository)
        {
            ReportGroupDescRepository = _reportGroupDescRepository;
            ReportRepository = _reportRepository;
            DonSupercatRepository = _donSupercatRepository;
            UserGroupHospRespository = _userGroupHospRespository;
            HospitalRepository = _hospitalRepository;
            CwrfAccessControlRepository = _cwrfAccessControlRepository;
            FinancialClosingRepository = _financialClosingRepository;
        }

        public List<SelectListItem> GetReportNameList(string current_system, string user_inst_code, int cnt, List<ReportViewModel> report_detail_list, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = 0; i < cnt; i++)
            {
                result.Add(new SelectListItem() { Value = report_detail_list[i].Report_Id.ToString(), Text = i + 1 + ": " + report_detail_list[i].Rpt_name });
            }

            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetReportGpList(string current_system, string user_inst_code, int cnt, List<ReportViewModel> report_detail_list, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = ReportGroupDescRepository.GetReportGpList(current_system, user_inst_code, cnt, report_detail_list);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result.Add(new SelectListItem() { Value = ds.Tables[0].Rows[i]["group_id"].ToString(), Text = ds.Tables[0].Rows[i]["group_desc"].ToString() });
                }
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetFlashRptDateList(int financial_year, int actual_cnt, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var Tmp_Mth = 5;
            var tmp_int = 0;
            var temp_last_mth = 0;
            var temp_year = 0;
            var SelStr = false;
            if (actual_cnt == 0)
            {
                result.Add(new SelectListItem() { Value = financial_year + "0401" + SelStr, Text = new DateTime(1, 4, 1).ToString("MMMM", new CultureInfo("en-US")) + " " + financial_year.ToString().Substring(financial_year.ToString().Length - 2) });
            }
            else
            {
                for (int i = 1; i <= actual_cnt; i++)
                {
                    if (Tmp_Mth == 13)
                    {
                        Tmp_Mth = 1;
                        tmp_int = 1;
                    }
                    if (Tmp_Mth == 1)
                    {
                        temp_last_mth = 12;
                        temp_year = financial_year + tmp_int - 1;
                    }
                    else
                    {
                        temp_last_mth = Tmp_Mth - 1;
                        temp_year = financial_year + tmp_int;
                    }

                    if (i == actual_cnt)
                        SelStr = true;

                    result.Add(new SelectListItem() { Value = financial_year + tmp_int + Tmp_Mth.ToString("00") + "01 ", Text = new DateTime(temp_year, temp_last_mth, 1).ToString("MMM yy", new CultureInfo("en-US")), Selected = SelStr });
                    Tmp_Mth = Tmp_Mth + 1;
                    SelStr = false;
                }
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetFlashRptDateList2(int financial_year, int actual_cnt, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var Tmp_Mth = 5;
            var tmp_int = 0;
            var temp_last_mth = 0;
            var temp_year = 0;
            var SelStr = false;
            for (int i = 1; i <= 12; i++) //it can be "for i = 1 to 24" in case client wants to view more future months (Marco, Oct 08)
            {
                if (i == 9)
                {
                    Tmp_Mth = 1;
                    tmp_int = 1;
                }
                if (i == 21)
                {
                    Tmp_Mth = 1;
                    tmp_int = 2;
                }

                if (Tmp_Mth == 1)
                {
                    temp_last_mth = 12;
                    temp_year = financial_year + tmp_int - 1;
                }
                else
                {
                    temp_last_mth = Tmp_Mth - 1;
                    temp_year = financial_year + tmp_int;
                }
                if (i == actual_cnt)
                    SelStr = true;

                result.Add(new SelectListItem() { Value = financial_year + tmp_int + Tmp_Mth.ToString("00") + "01 ", Text = new DateTime(temp_year, temp_last_mth, 1).ToString("MMM yy", new CultureInfo("en-US")), Selected = SelStr });
                Tmp_Mth = Tmp_Mth + 1;
                SelStr = false;
            }

            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }
        public List<SelectListItem> GetReportHospList(string login_id, string current_sys)
        {
            login_id = login_id.Replace("\\", "/").ToLower();
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = ReportRepository.GetReportHospList(login_id, current_sys);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var hosp_code = dr["hospital_code"].ToString().Trim();
                    var hosp_desc = dr["hospital_desc"].ToString();
                    var sub_desc = dr["sub_desc"].ToString();
                    var consolidateion_ind = dr["consolidateion_ind"].ToString();
                    if (consolidateion_ind == "Y" && hosp_code != "ALL" && current_sys != "F")
                        result.Add(new SelectListItem() { Value = hosp_code, Text = hosp_desc + " " + sub_desc });
                }

            }
            return result;
        }

        public List<SelectListItem> GetReportHospGpList(string user_group, string user_inst_code, string ho_access, string login_id, string current_sys, out Dictionary<string, string> consolidate_ind_dict)
        {
            login_id = login_id.Replace("\\", "/").ToLower();
            List<SelectListItem> result = new List<SelectListItem>();
            consolidate_ind_dict = new Dictionary<string, string>();
            var ds = ReportRepository.GetReportHospGpList(user_group, user_inst_code, ho_access, login_id, current_sys);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var hosp_gp = dr["hospital_code"].ToString().Trim();
                    var hosp_desc = dr["hospital_desc"].ToString().Trim();
                    var sub_desc = dr["sub_desc"].ToString().Trim();
                    var consolidateion_ind = dr["consolidateion_ind"].ToString();
                    result.Add(new SelectListItem() { Value = hosp_gp, Text = hosp_desc + " " + sub_desc });
                    if (consolidateion_ind != "N")
                        consolidate_ind_dict.Add("consolidation_ind_" + hosp_gp, consolidateion_ind);
                }
            }
            return result;
        }

        public List<ReportGroupModel> GetRptGpFullList(string current_sys, string user_group, string user_inst_code, string login_id, string privilege_non_pjt_report,
            string privilege_asoi_rpt, string privilege_report, string privilege_closing_report, string ho_access, int report_period, string cluster_level)
        {
            List<ReportGroupModel> result = new List<ReportGroupModel>();
            string old_group_id = "";
            DataSet ds = ReportRepository.GetReportDetailDataSet(current_sys, user_group, user_inst_code, login_id, privilege_non_pjt_report, privilege_asoi_rpt, privilege_report, privilege_closing_report, ho_access, report_period, cluster_level);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var group_id = dr["group_id"].ToString();
                    if (old_group_id != group_id)
                    {
                        old_group_id = group_id;
                        ReportGroupModel model = new ReportGroupModel();
                        model.Group_id = group_id;
                        model.Rpt_cnt = 0;
                        model.Report_id_list = new List<string>();
                        result.Add(model);
                    }
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var report_id = dr["report_id"].ToString();
                    if (report_id != "206" || cluster_level == "Y") { 
                        foreach (ReportGroupModel model in result)
                        {
                            if (dr["group_id"].ToString() == model.Group_id)
                            {
                                model.Report_id_list.Add(report_id);
                                model.Rpt_cnt++;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public List<ReportViewModel> GetReportDetailList(string current_sys, string user_group, string user_inst_code, string login_id, string privilege_non_pjt_report,
            string privilege_asoi_rpt, string privilege_report, string privilege_closing_report, string ho_access, int report_period, string cluster_level, out int cnt)
        {
            cnt = 0;
            List<ReportViewModel> result = new List<ReportViewModel>();
            DataSet ds = ReportRepository.GetReportDetailDataSet(current_sys, user_group, user_inst_code, login_id, privilege_non_pjt_report, privilege_asoi_rpt, privilege_report, privilege_closing_report, ho_access, report_period, cluster_level);

            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ReportViewModel model = new ReportViewModel();
                    model.Report_Id = Convert.ToInt32(dr["report_id"]);
                    model.Annual_costing_ind = dr["annual_costing_ind"].ToString();
                    model.HAHO_ind = dr["HAHO_ind"].ToString();
                    model.Hosp_ind = dr["hosp_ind"].ToString();
                    if ((user_inst_code == "HAHO" && model.HAHO_ind == "Y") || (user_inst_code != "HAHO" && model.Hosp_ind == "Y") || 1 == 1)
                    {
                        if (model.Annual_costing_ind != "Y" || privilege_closing_report == "I" || privilege_closing_report == "R")
                        {
                            if (model.Report_Id != 206 || cluster_level == "Y")
                            {
                                model.Group_id = dr["group_id"].ToString();
                                model.Rpt_name = dr["rpt_name"].ToString();
                                model.Asp_file = dr["asp_file"].ToString();
                                model.Hosp_asp_file = dr["hosp_asp_file"].ToString();
                                model.Show_hosp_ind = dr["show_hosp_ind"].ToString();
                                model.Show_pe_type_ind = dr["show_pe_type_ind"].ToString();
                                model.Show_pe_var_type_ind = dr["show_pe_var_type_ind"].ToString();
                                model.Show_project_item_ind = dr["show_project_item_ind"].ToString();
                                model.Show_staff_type_ind = dr["show_staff_type_ind"].ToString();
                                model.Show_cluster_ind = dr["show_cluster_ind"].ToString();
                                model.Eis_element_ind = dr["eis_element_ind"].ToString();
                                model.Activity_ind = dr["activity_ind"].ToString();
                                model.Consolidation_ind = dr["consolidation_ind"].ToString();
                                model.Next_year_ind = dr["next_year_ind"].ToString();
                                model.Fund_gp_ind = dr["fund_gp_ind"].ToString();
                                model.Res_officer = dr["res_officer"].ToString();
                                model.Period = dr["period"].ToString();
                                model.Exp_type = dr["exp_type"].ToString();
                                model.With_eis_stat = dr["with_eis_stat"].ToString();
                                model.Cbv_rpt_type = dr["cbv_rpt_type"].ToString();
                                model.Cwrf_recur = dr["cwrf_recur"].ToString();
                                model.Cwrf_nonrecur = dr["cwrf_nonrecur"].ToString();
                                model.Budget_cluster = dr["budget_cluster"].ToString();
                                model.Consolidate_individual = dr["consolidate_individual"].ToString();
                                cnt = cnt + 1;
                            }
                        }
                    }
                    result.Add(model);
                }
            }

            return result;
        }
        public List<SelectListItem> GetSnapshotYr(bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var asoi_year_list = ReportRepository.GetSnapshotYr();
            foreach (var asoi_year in asoi_year_list)
            {
                result.Add(new SelectListItem() { Value = asoi_year.ToString(), Text = asoi_year.ToString() });
            }

            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }
        public int GetSnapshotMaxYr()
        {
            return ReportRepository.GetSnapshotMaxYr();
        }

        public List<string> GetSnapshotDateListByMaxYr()
        {
            return ReportRepository.GetSnapshotDateByMaxYr();
        }

        public List<SelectListItem> GetFVIndList(bool allRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = FinancialClosingRepository.GetFVInd();
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new SelectListItem() { Value = dr["id"].ToString(), Text = dr["industry_name"].ToString() });
                }
            }
            if (allRow) { result.Add(new SelectListItem() { Value = "ALL", Text = "All" }); }
            return result;
        }

        public List<FVIndModel> GetFVIndTenderer_list()
        {
            string ind_id = "";
            List<FVIndModel> result = new List<FVIndModel>();
            var ds = FinancialClosingRepository.GetFVIndTenderer();
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (ind_id != dr["industry_id"].ToString())
                    {
                        ind_id = dr["industry_id"].ToString();
                        FVIndModel model = new FVIndModel();
                        model.Fv_ind_id = ind_id;
                        model.Fv_ind_name = dr["industry_name"].ToString();
                        model.Tenderer_list = new List<FVTendererModel>();
                        result.Add(model);
                    }
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    foreach (FVIndModel model in result)
                    {
                        if (dr["industry_id"].ToString() == model.Fv_ind_id)
                        {
                            FVTendererModel tenderer = new FVTendererModel();
                            tenderer.Fv_tenderer_id = dr["id"].ToString();
                            tenderer.Fv_tenderer_name = dr["tenderer_name"].ToString();
                            model.Tenderer_list.Add(tenderer);
                        }
                    }
                }
            }

            return result;
        }

        public List<SelectListItem> GetFundGpList(bool allRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = FinancialClosingRepository.GetFundGp();
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new SelectListItem() { Value = dr["code"].ToString(), Text = dr["code"].ToString() });
                }
            }
            if (allRow) { result.Insert(0, new SelectListItem() { Value = "ALL", Text = Resource.ALLCap }); }
            return result;
        }

        public List<SelectListItem> GetAsAtList(int financial_year, int actual_cnt, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var Tmp_Mth = 4;
            var tmp_int = 0;
            var SelStr = false;

            for (int i = 0; i <= actual_cnt; i++)
            {
                if (Tmp_Mth == 13)
                {
                    Tmp_Mth = 1;
                    tmp_int = 1;
                }
                if (i == actual_cnt)
                    SelStr = true;
                var Tmp_date = new DateTime(financial_year, Tmp_Mth, 1).AddYears(tmp_int).AddDays(-1).ToString("yyyyMMdd", new CultureInfo("en-US"));
                var Tmp_text = new DateTime(financial_year, Tmp_Mth, 1).AddYears(tmp_int).ToString("MMM yy", new CultureInfo("en-US"));
                result.Add(new SelectListItem() { Value = Tmp_date, Text = Tmp_text, Selected = SelStr });
                Tmp_Mth = Tmp_Mth + 1;
                SelStr = false;
            }

            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetAsAtMList(int financial_year, int actual_cnt, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var SelStr = false;
            for (int Tmp_Mth = 1; Tmp_Mth <= 12; Tmp_Mth++)
            {
                if (Tmp_Mth - 4 == actual_cnt || Tmp_Mth + 4 == actual_cnt)
                {
                    SelStr = true;
                }
                if (Tmp_Mth == 2)
                    SelStr = true;
                var Tmp_date = new DateTime(financial_year, Tmp_Mth, 1).AddMonths(1).AddDays(-1).ToString("MMdd", new CultureInfo("en-US"));
                var Tmp_text = new DateTime(financial_year, Tmp_Mth, 1).ToString("MMM", new CultureInfo("en-US"));
                result.Add(new SelectListItem() { Value = Tmp_date, Text = Tmp_text, Selected = SelStr });
                SelStr = false;
            }

            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetAsAtYList(string value_date, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            for (int i = 2002; i <= Convert.ToInt32(value_date.Substring(0, 4)); i++)
            {
                result.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString() });
            }

            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }
        public List<SelectListItem> GetDonSuperCat(bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donSuperCat = DonSupercatRepository.GetAllDonSupercat();
            if (donSuperCat.Count() > 0)
            {
                result = donSuperCat.Select(s => new SelectListItem() { Value = s.SupercatId.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "0", Text = Resource.AllCategory }); }
            return result;
        }

        public List<SelectListItem> GetHospClusterList_forM(string user_group, bool AllHAHORow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = UserGroupHospRespository.GetHospClusterList_forM(user_group);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new SelectListItem() { Value = dr["hospital_code"].ToString(), Text = dr["cluster"].ToString() });
                }
            }
            if (AllHAHORow) {
                result.Insert(0, new SelectListItem() { Value = "ALL", Text = Resource.AllCorp });
                result.Add(new SelectListItem() { Value = "HO", Text = Resource.HAHO });
            }
            return result;
        }

        public List<SelectListItem> GetBudgetClusterHospList(string user_inst_code, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = HospitalRepository.GetBudgetClusterHospList(user_inst_code);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new SelectListItem() { Value = dr["hospital"].ToString(), Text = dr["Cluster"].ToString() });
                }
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetWorkAgentList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = ReportRepository.GetWorkAgent();
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new SelectListItem() { Value = dr["wk_agent"].ToString(), Text = " - " + dr["wk_agent"].ToString() });
                }
            }
            result.Insert(0, new SelectListItem() { Value = "ALL", Text = Resource.ALLCap });
            result.Insert(1, new SelectListItem() { Value = "HA", Text = Resource.HAOnly });
            result.Insert(2, new SelectListItem() { Value = "ALL", Text = Resource.OtherWorkAgent });
            return result;
        }

        public List<SelectListItem> GetBatchNoList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Insert(0, new SelectListItem() { Value = "ALL", Text = Resource.ALLCap });
            var batch_no = ReportRepository.GetBatchNo();
            if (!string.IsNullOrEmpty(batch_no))
            {
                result.Add(new SelectListItem() { Value = batch_no.ToString(), Text = Resource.Batch + " " + batch_no.ToString() });
                result.Add(new SelectListItem() { Value = "-" + batch_no.ToString(), Text = Resource.BatchNo + " < " + batch_no.ToString() });
            }
            return result;
        }

        public List<SelectListItem> GetProjItemIndList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = FinancialClosingRepository.GetProjItemIndList();
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(new SelectListItem() { Value = dr["id"].ToString(), Text = dr["description"].ToString() });
                }
            }
            return result;
        }

        public int GetCbvAccessControlByUserNameCount(string login_id)
        {
            return ReportRepository.GetCbvAccessControlByUserNameCount(login_id);
        }

        public int GetCwrfAccessControlByUserName(string login_id)
        {
            return CwrfAccessControlRepository.GetCwrfAccessControlByUserName(login_id).Count;
        }

        public List<RptItemModel> GetRtpItem_list(string user_group, string login_id)
        {
            string rpt_id = "";
            List<RptItemModel> result = new List<RptItemModel>();
            var ds = ReportRepository.GetRtpItem_list(user_group, login_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (rpt_id != dr["rpt_id"].ToString())
                    {
                        rpt_id = dr["rpt_id"].ToString();
                        RptItemModel model = new RptItemModel();
                        model.Rpt_id = rpt_id;
                        model.Table_rtp_item_list = new List<TableRptItemModel>();
                        result.Add(model);
                    }
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var dr_rpt_id = dr["rpt_id"].ToString();
                    if (dr_rpt_id != "17" && dr_rpt_id != "18" && dr_rpt_id != "286" && dr_rpt_id != "293" && dr_rpt_id != "294")
                    {
                        foreach (RptItemModel model in result)
                        {
                            if (dr["rpt_id"].ToString() == model.Rpt_id)
                            {
                                TableRptItemModel rptItem = new TableRptItemModel();
                                rptItem.Id = dr["id"].ToString();
                                rptItem.Description = dr["description"].ToString();
                                model.Table_rtp_item_list.Add(rptItem);
                            }
                        }
                    }
                    else
                    {
                        int index = 0;
                        string str_old_group_id = "0";
                        var ds2 = ReportRepository.GetProjListRptItem(login_id, dr_rpt_id);
                        foreach (DataRow dr2 in ds2.Tables[0].Rows)
                        {
                            foreach (RptItemModel model in result)
                            {
                                if (dr["rpt_id"].ToString() == model.Rpt_id)
                                { 
                                    if (index == 0) {
                                        if (dr2["group_id"].ToString() == "0")
                                        {
                                            TableRptItemModel rptItem = new TableRptItemModel();
                                            rptItem.Id = "A_0";
                                            rptItem.Description = dr2["group_desc"].ToString();
                                            model.Table_rtp_item_list.Add(rptItem);
                                        }
                                        index++;
                                    }
                                    if (str_old_group_id != dr2["group_id"].ToString())
                                    {
                                        if (dr2["group_desc"].ToString().Trim() != "")
                                        {
                                            TableRptItemModel rptItem = new TableRptItemModel();
                                            rptItem.Id = "G_" + dr2["group_id"].ToString().Trim();
                                            rptItem.Description = dr2["group_desc"].ToString().Trim();
                                            model.Table_rtp_item_list.Add(rptItem);
                                        }
                                        str_old_group_id = dr2["group_id"].ToString();
                                    }
                                    if (!(dr2["id_type"].ToString().Trim() == "I" && dr2["id"].ToString() == "31" ))
                                    {
                                        if (dr2["id_type"].ToString().Trim() == "I")
                                        {
                                            TableRptItemModel rptItem = new TableRptItemModel();
                                            rptItem.Id = "I_" + dr2["id"].ToString().Trim();
                                            rptItem.Description = dr2["item_desc"].ToString().Trim();
                                            model.Table_rtp_item_list.Add(rptItem);
                                        }
                                        else
                                        {
                                            TableRptItemModel rptItem = new TableRptItemModel();
                                            rptItem.Id = "S_" + dr2["id"].ToString().Trim();
                                            rptItem.Description = dr2["subgroup_desc"].ToString().Trim();
                                            model.Table_rtp_item_list.Add(rptItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }



        #region Generate Report
        public int GetParmNo(string rpt_name)
        {
            return ReportRepository.GetParmNo(rpt_name);
        }

        public string GetCurrentPeriodByReportDate(string report_date)
        {
            string result = "";
            var ds = FinancialClosingRepository.GetCurrentPeriodByReportDate(report_date);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result = ds.Tables[0].Rows[0]["period_for"].ToString().Trim();
            }
            return result;
        }

        public void InsertReportLog(string login_id, string report_type)
        {
            ReportRepository.InsertReportLog(login_id, report_type);
        }

        // return GenReportModel
        public GenReportModel GetReportParm(ReportViewModel model, string value_date, string login_id, int financial_year, string other_wk_agent, string p13, string year_end, string user_group, int current_year, string current_date, string period_end_date)
        {
            GenReportModel rptModel = new GenReportModel();
            var report_date = value_date;
            if ((Convert.ToInt32(model.As_at_y) % 4 == 0) && model.As_at_m == "0228") model.As_at_m = "0229";
            rptModel.Report_name = model.Report_name.Replace(" ", "+").Replace("/", "_").Replace("&", "_").Replace(",", "").Replace(">", "_");
            rptModel.Rpt_name = model.Rpt_name;
            rptModel.Report_group = model.Rpt_group;
            rptModel.Rpt_group = model.Rpt_group;
            rptModel.Report_format = model.Export_to_excel == "1" ? "EXCEL" : "PDF";
            rptModel.Parm_no = GetParmNo(model.Rpt_name);
            rptModel.Current_period = GetCurrentPeriodByReportDate(report_date);
            var this_year = Convert.ToInt32(value_date.Substring(0, 4));
            var this_month = Convert.ToInt32(value_date.Substring(4, 2));
            var input_for = "";
            if (rptModel.Current_period == "Interim")
            {
                if (this_month >= 1 && this_month < 3)
                    this_year = this_year - 1;
                input_for = this_year + "0901";
            }
            else
            {
                input_for = this_year + "0301";
            }
            model.Inst_code = model.Rpt_hospital;
            if (model.Inst_code == "PROJ" || model.Inst_code == "ALL" || model.Inst_code == "ALL_XPET" || model.Inst_code == "PROJ_XPET")
                model.Report_type = model.Asp_file;
            else
                model.Report_type = model.Hosp_asp_file;
            InsertReportLog(login_id, model.Report_type);
            var this_date = DateTime.ParseExact(report_date, "yyyyMMdd", CultureInfo.InvariantCulture);
            var last_month = this_date.AddMonths(-1);
            var last_month_c = last_month.ToString("yyyyMMdd");
            var last_month_end = this_date.AddDays(-1);
            var last_month_end_c = last_month_end.ToString("yyyyMMdd");
            var next_day = DateTime.ParseExact(current_date, "yyyyMMdd", CultureInfo.InvariantCulture).AddDays(1); 
            var next_day_c = next_day.ToString("yyyyMMdd");
            DateTime period_end_date_date = DateTime.ParseExact(current_date, "yyyyMMdd", CultureInfo.InvariantCulture);
            rptModel.Report_type = model.Report_type;
            if (model.Report_type.Substring(model.Report_type.Length - 4) != ".rpt")
            {
                if (model.Report_type == "nursing_mo_ah_SARS")
                {
                    model.Report_type = "nursing_mo_ah";
                }
                if (model.Report_type.IndexOf(".") <= 0)
                {
                    model.Report_type = model.Report_type + ".asp";
                    rptModel.Parm_no = -1;
                }
                else
                {
                    rptModel.Parm_value = "";
                }
                if (model.Report_type == "rpt_package_quarterly.asp")
                {
                    last_month_c = DateTime.ParseExact(model.Flash_rpt_date, "yyyyMMdd", CultureInfo.InvariantCulture).AddMonths(-1).ToString("yyyyMMdd");
                }
                //RFS6037
                if (model.Report_type == "expec_expend.asp" || model.Report_type == "Bal_of_exp_expend.asp")
                {
                    rptModel.Parm_value = "prompt0=" + model.Proj_sort_list + "&prompt1 =" + model.Rpt_item_id;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt2=" + model.Rpt_item_id.Substring(model.Rpt_item_id.Length - model.Rpt_item_id.IndexOf("_")).Trim();
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + model.Rpt_item_id.Substring(0, model.Rpt_item_id.IndexOf("_") - 1).Trim() + "&";
                }
                //FR07A15 - 2.4
                if (model.Report_type == "rpt_except_cbv.asp")
                {
                    rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + report_date;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt2=" + model.Proj_type_main_select;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + financial_year + "&alert_date=" + model.Flash_rpt_date2 + "&";
                }
                //FR06421A
                if (model.Report_type == "rpt_cwrf_proforma_a.asp" || model.Report_type == "rpt_cwrf_proforma_b.asp" || model.Report_type == "cwrf_8083mm_summary.asp")
                {
                    if (!string.IsNullOrEmpty(model.Budget_cluster))
                    {
                        rptModel.Parm_value = "prompt0=" + model.Budget_cluster + "&prompt1=" + model.Cwrf_recur_list + "&";
                    }
                    else
                    {
                        rptModel.Parm_value = "prompt0=" + model.Budget_cluster_hosp + "&prompt1=" + model.Cwrf_recur_list + "&";
                    }
                }
                if (model.Report_type == "summary_list_8100mx.asp")
                {
                    rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + report_date;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt2=2&prompt3=" + model.Criteria2 + "&prompt4=" + model.Next_month + "&";
                }
                if (model.Report_type == "summary_list_8100mx_cluster.asp")
                {
                    rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + report_date;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt2=2&prompt3=" + model.Criteria2 + "&prompt4=" + model.Next_month + "&";
                }
                if (model.Report_type == "rpt_cwrf_claim_reibm_to_fhb.asp")
                {
                    rptModel.Parm_value = "prompt0=" + report_date + "&prompt1=ALL&prompt2=0&prompt3=ALL&prompt4=" + model.Rpt_cluster + "&prompt5=" + financial_year + "&";
                }
                if (model.Report_type == "rpt_cwrf_claim_reibm_to_fhb_category.asp")
                {
                    rptModel.Parm_value = "prompt0=" + report_date + "&prompt1=ALL&prompt2=" + model.Closed_only;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt3=ALL&prompt4=" + model.Rpt_cluster + "&prompt5=" + financial_year;
                    rptModel.Parm_value = rptModel.Parm_value + "&prompt6=" + model.Top_10 + "&";
                }
                if (model.Report_type == "donation_movement.asp")
                {
                    int exp_type = model.Exp_type.IndexOf("i") > 0 ? 1 : 2;
                    string as_at = "";
                    if (model.As_at_m.Substring(0, 2) == "12")
                    {
                        as_at = Convert.ToInt32(model.As_at_y) + 1 + "0101";
                    }
                    else if (Convert.ToInt32(model.As_at_m.Substring(0, 2)) < 9)
                    {
                        as_at = model.As_at_y + (Convert.ToInt32(model.As_at_m.Substring(0, 2)) + 1) + "01";
                    }
                    else
                    {
                        as_at = model.As_at_y + (Convert.ToInt32(model.As_at_m.Substring(0, 2)) + 1) + "01";
                    }
                    rptModel.Parm_value = "hosp_gp=" + model.Hosp_cluster + "&current_date=" + as_at + "&financial_year=" + financial_year;
                    rptModel.Parm_value = rptModel.Parm_value + "&recon_type=" + exp_type + "&fund=" + model.Don_fund;
                }
                if (model.Report_type == "donation_detail.asp")
                {
                    rptModel.Parm_value = "hosp_gp=" + model.Hosp_cluster + "&trust=-1" + "&don_purpose=" + model.Don_purpose + "&don_inc_exp=" + model.Exp_type;
                    rptModel.Parm_value = rptModel.Parm_value + "&rpt_date_c=" + model.As_at_y + model.As_at_m + "&financial_year=" + financial_year;
                    rptModel.Parm_value = rptModel.Parm_value + "&range_ind=0" + "&range=0" + "&deleted=" + model.Deleted + "&fund=" + model.Don_fund + "&with_previous=1" + "&don_supercat=" + model.Don_cat;
                }
                if (model.Report_type == "don_pj_rpt.asp")
                {
                    rptModel.Report_name = "Report12";
                    rptModel.Parm_value = "hosp_gp=" + model.Hosp_cluster + "&rpt_date_c=" + model.As_at_y + model.As_at_m.Substring(0, 2) + "01" + "&fund=" + model.Don_fund;
                }
            }
            else
            {
                Random rnd = new Random();
                switch (model.Report_type)
                {
                    case "rpt_cluster_performance.rpt": //FR04719A - new report
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + financial_year + "&prompt2=" + model.Rpt_cluster;
                        rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + model.Rpt_item_id.Substring(model.Rpt_item_id.Length - model.Rpt_item_id.IndexOf("_")).Trim();
                        rptModel.Parm_value = rptModel.Parm_value + "&prompt4=" + model.Rpt_item_id.Substring(0, model.Rpt_item_id.IndexOf("_") - 1).Trim() + "&";
                        break;
                    case "rpt_cbv_budget_cashflow_by_cluster.rpt":
                        rptModel.Parm_value = "rpt_date=" + last_month_c + "&financial_year=" + financial_year;
                        break;
                    case "rpt_cbv_budget_cashflow_by_project.rpt":
                        rptModel.Parm_value = "rpt_date=" + last_month_c + "&financial_year=" + financial_year + "&corp=" + model.Rpt_cluster;
                        break;
                    case "flash.rpt":
                        rptModel.Report_type = "rpt_package_quarterly.asp";
                        rptModel.Parm_no = -1;
                        last_month_c = DateTime.ParseExact(model.Flash_rpt_date, "yyyyMMdd", CultureInfo.InvariantCulture).AddMonths(-1).ToString("yyyyMMdd");
                        break;
                    case "flash_by_cluster.rpt":
                        rptModel.Report_type = "flash.rpt";
                        rptModel.Parm_value = "prompt0=CLUSTER&prompt1=" + last_month_c + "&prompt2=" + report_date + "&prompt3=N&prompt4=" + financial_year + "&";
                        break;
                    case "trend_flash_rpt.rpt":
                    case "trend_flash_rpt_v2.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_consolidation;
                        rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + model.Rpt_next_year + "&prompt4=" + financial_year + "&";
                        break;
                    case "project_list.rpt":
                        if (model.Rpt_item_id == "28" && other_wk_agent == "Y")
                        {
                            rptModel.Report_type = "proj_8100mx.rpt";
                            rptModel.Parm_value = "prompt1=" + report_date + "&prompt2=" + model.Rpt_cluster + "&prompt3=Y&prompt0=" + financial_year + "&";
                        }
                        else
                        {
                            rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_item_id.Substring(model.Rpt_item_id.Length - model.Rpt_item_id.IndexOf("_")).Trim();
                            rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + model.Rpt_item_id.Substring(0, model.Rpt_item_id.IndexOf("_") - 1).Trim() + "&prompt4=" + financial_year;
                            rptModel.Parm_value = rptModel.Parm_value + "&prompt5=M&prompt6=" + model.Proj_sort_list + "&";
                        }
                        break;
                    case "project_list_reimbursement.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=31&prompt3=I&prompt4=" + financial_year + "&prompt5=M&prompt6=" + model.Proj_sort_list + "&";
                        break;
                    case "proj_type_hosp.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_item_id + "&";
                        break;
                    case "proj_type2.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_hospital + "&prompt1=" + last_month_c + "&prompt2=0&prompt3=" + financial_year + "&";
                        break;
                    case "cbv_by_month_proc.rpt":
                        int cm = 0;
                        if (model.Cwrf_recur == "CM")
                        {
                            model.Cwrf_recur = "ALL";
                            cm = 1;
                        }
                        rptModel.Parm_value = "hospital_gp=" + model.Rpt_cluster + "&ReportDate_c=" + last_month_c + "&next_year=" + model.Rpt_next_year;
                        rptModel.Parm_value = rptModel.Parm_value + "&financial_yy=" + financial_year + "&cwrf_recur=" + model.Cwrf_recur + "&one_line=" + model.One_line + "&cm=" + cm;
                        break;
                    case "cbv_eq_list.rpt":
                        cm = model.Cwrf_recur == "CM" ? 1 : 0;
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + model.Rpt_cluster + "&prompt2=" + cm + "&prompt3=" + financial_year + "&";
                        break;
                    case "project_by_month_by_type.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_hospital + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_next_year + "&prompt3=" + model.Rpt_item_id + "&prompt4=" + financial_year + "&";
                        break;
                    case "project_by_month.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_next_year + "&prompt3=" + financial_year;
                        rptModel.Parm_value = rptModel.Parm_value + "&prompt4=" + model.Rpt_item_id.Substring(model.Rpt_item_id.Length - model.Rpt_item_id.IndexOf("_")).Trim() + "&prompt5=" + model.Rpt_item_id.Substring(0, model.Rpt_item_id.IndexOf("_") - 1).Trim();
                        rptModel.Parm_value = rptModel.Parm_value + "&prompt6=Y&prompt7=0&prompt8=ALL&prompt9=M&prompt10=" + model.Proj_sort_list + "&";
                        break;
                    case "project_by_month_reimbursement.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_next_year + "&prompt3=" + financial_year + "&prompt4=31&prompt5=I&prompt6=Y&prompt7=0&prompt8=ALL&prompt9=M&prompt10=" + model.Proj_sort_list + "&";
                        break;
                    case "donation_exp_by_haho.rpt":
                        rptModel.Report_type = "project_by_month.rpt";
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_next_year + "&prompt3=" + financial_year + "&prompt4=14&prompt5=Y&prompt6=0&prompt7=FE&";
                        break;
                    case "journal.rpt":
                        rptModel.Report_type = "pe_adjust.rpt&prompt1=" + model.Rpt_hospital + "&prompt2=" + report_date + "&prompt3=-1&prompt4=M&prompt0=D&";
                        break;
                    case "annual_costing.rpt":
                    case "annual_costing_hosp.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "0401&prompt1=" + model.Rpt_hospital + "&";
                        break;
                    case "annual_costing_by_hosp.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "0401&prompt1=" + model.Rpt_hospital + "&prompt2=" + model.Rpt_consolidation + "&";
                        break;
                    case "annualcostingvarreport.rpt":
                    case "annualcostingvarreport2.rpt":
                    case "annualcostingvarreport3.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "0401&prompt1=" + model.Rpt_hospital + "&prompt2=" + model.Rpt_consolidation + "&prompt3=" + model.Rpt_consolidation + "&";
                        break;
                    case "annualcostingvarreport2_c.rpt":
                        rptModel.Report_type = "annualcostingvarreport2.rpt";
                        rptModel.Parm_value = "prompt0=" + financial_year + "0401&prompt1=ALL&prompt2=Y&";
                        break;
                    case "annualcostingvarreport3_c.rpt":
                        rptModel.Report_type = "annualcostingvarreport3.rpt";
                        rptModel.Parm_value = "prompt0=" + financial_year + "0401&prompt1=ALL&prompt2=Y&";
                        break;
                    case "annual_costing_compare_by_hosp.rpt":
                    case "annualcostingbreakdown.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "0401&prompt1=" + model.Rpt_hospital + "&prompt2=" + model.Rpt_consolidation + "&";
                        break;
                    case "trend_tb_rpt.rpt":
                        if (model.Modules == "Y" && DateTime.Now.Month > 4 && DateTime.Now.Month < 8 && DateTime.Now.Year == financial_year + 1)
                        { financial_year = financial_year + 1; }
                        if (model.Rpt_next_year == "1")
                        {
                            report_date = financial_year + "0401";
                            financial_year = financial_year - 1;
                        }
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + model.Rpt_consolidation + "&prompt2=" + model.Fund_gp + "&prompt3=" + model.Fund_gp_ind + "&prompt4=" + report_date + "&prompt5=" + financial_year + "&";
                        break;
                    case "trend_tb_flash_rpt.rpt":
                        if (model.Modules == "Y" && DateTime.Now.Month < 8 && DateTime.Now.Year == financial_year + 1)
                        { financial_year = financial_year + 1; }
                        if (model.Rpt_next_year == "1")
                        {
                            report_date = financial_year + "0401";
                            financial_year = financial_year - 1;
                        }
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + model.Rpt_hospital + "&prompt2=" + model.Rpt_consolidation + "&prompt3=" + report_date + "&prompt4=" + model.Rpt_format + "&";
                        break;
                    case "trend_recon_rpt.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&";
                        break;
                    case "rpt_interhosp_recon.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_hospital + "&";
                        break;
                    case "others_breakdown.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_hospital + "&prompt1=" + model.Rpt_consolidation + "&prompt2=" + value_date + "&";
                        break;
                    case "Recon_Cash_Accr.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_hospital + "&";
                        break;
                    case "sch_m_rpt.rpt":
                    case "sch_m1_rpt.rpt":
                    case "sch_s_rpt.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&input_for_c=" + input_for + "&consolidation_ind=" + model.Rpt_consolidation;
                        break;
                    case "far_journal.rpt":
                        rptModel.Parm_value = "prompt0=" + input_for + "&prompt1=" + model.Rpt_hospital + "&prompt2=Y&";
                        break;
                    case "sch_cap_reserve.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + input_for + "&prompt2=" + model.Rpt_consolidation + "&";
                        break;
                    case "sch_o_cost_rpt.rpt":
                    case "sch_o_depr_rpt.rpt":
                    case "sch_n_cost_rpt.rpt":
                    case "sch_n_depr_rpt.rpt":
                    case "sch_n_loss_rpt.rpt":
                    case "sch_o_loss_rpt.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + input_for + "&prompt2=" + model.Rpt_consolidation + "&";
                        break;
                    case "fc_ie.rpt":
                    case "fc_bs.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + model.Rpt_consolidation + "&prompt2=" + model.Period + "&prompt3=" + model.Exclude + "&prompt4=" + model.Basis + "&";
                        break;
                    case "analysis_oc.rpt":
                    case "analysis_income.rpt":
                    case "analysis_pe.rpt":
                        if (model.Report_type == "analysis_oc.rpt") rptModel.Report_type = "analysis_rpt.rpt&prompt1=O";
                        else if (model.Report_type == "analysis_income.rpt") rptModel.Report_type = "analysis_rpt.rpt&prompt1=I";
                        else if (model.Report_type == "analysis_pe.rpt") rptModel.Report_type = "analysis_rpt.rpt&prompt1=P";
                        rptModel.Parm_value = "prompt0=" + report_date + "&prompt2=0&prompt3=" + financial_year + "&";
                        break;
                    case "project_report.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_item_id + "&prompt1=" + model.Rpt_cluster + "&prompt2=" + value_date + "&prompt3=N&prompt4=" + login_id.Replace("/", "\\") + " & prompt5 = " + financial_year + " & ";
                        break;
                    case "pe_movement.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_consolidation + "&prompt3=" + model.Adjust_type_id + "&prompt4=" + p13 + "&prompt5=" + financial_year + "&";
                        break;
                    case "bank_recon.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_hospital + "&year_end_c=" + year_end;
                        break;
                    case "sch_q_rpt.rpt":
                    case "sch_q1_rpt.rpt":
                    case "sch_r_rpt.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&current_date_c=" + input_for;
                        break;
                    case "sch_stock_balance1.rpt":
                        rptModel.Parm_value = "item_no=1";
                        break;
                    case "sch_stock_balance2.rpt":
                        rptModel.Parm_value = "item_no=2";
                        break;
                    case "fc_ie_oe_breakdown.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Period + "&prompt1=" + model.Exclude + "&";
                        break;
                    case "fc_ie_item_breakdown.rpt":
                        rptModel.Parm_value = "prompt0="+ model.Period + "&prompt1=" + model.Exclude+ "&prompt2=" + model.Exp_type + "&";
                        break;
                    case "cbv_app4.rpt":
                        rptModel.Parm_value = "rpt_date=" + report_date + "&inst_code=" + model.Rpt_cluster + "&type=" + model.Proj_type_select + "&finance_year=" + financial_year;
                        break;
                    case "cbv_app6.rpt":
                        rptModel.Parm_value = "type=" + model.Proj_type_select + "&rpt_date=" + report_date + "&inst_code=" + model.Rpt_cluster + "&finance_year=" + financial_year;
                        break;
                    case "cbv_app7.rpt":
                        rptModel.Parm_value = "rpt_date=" + report_date + "&inst_code=" + model.Rpt_cluster + "&finance_year=" + financial_year;
                        break;
                    case "cbv_app6_2.rpt":
                    case "cbv_app2.rpt":
                    case "cbv_app3.rpt":
                    case "cbv_app8.rpt":
                    case "cbv_rae.rpt":
                        rptModel.Parm_value = "rpt_date=" + report_date + "&finance_year=" + financial_year;
                        break;
                    case "cbv_app1.rpt":
                        rptModel.Parm_value = "rpt_date=" + report_date + "&Corp=" + model.Rpt_cluster + "&finance_year=" + financial_year;
                        break;
                    case "cbv_mth_exp.rpt":
                        rptModel.Parm_value = "rpt_date=" + report_date + "&cur_roll_year=" + financial_year + "&report_type=M";
                        break;
                    case "cbv_item_mth_exp.rpt":
                        rptModel.Parm_value = "rpt_date=" + report_date + "&cur_roll_year=" + financial_year + "&report_type=I";
                        break;
                    case "cwrf_recurrent.rpt":
                    case "cwrf_recurrent_cluster.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Cwrf_recur + "&prompt1=" + report_date + "&prompt2=" + model.Cwrf_sort_list;
                        rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + model.Closed_only + "&prompt4=" + model.Next_month + "&prompt5=" + model.Rpt_cluster + "&prompt6=" + financial_year + "&";
                        break;
                    //case "cwrf_recurrent_cluster.rpt":
                    //    rptModel.Parm_value = "prompt0=" + model.Cwrf_recur + "&prompt1=" + report_date + "&prompt2=" + model.Cwrf_sort_list;
                    //    rptModel.Parm_value = rptModel.Parm_value + "&prompt3=" + model.Closed_only + "&prompt4=" + model.Rpt_cluster + "&";
                    //    break;
                    case "cwrf_recur_summary.rpt":
                        if (model.Next_month == "0")
                        {
                            rptModel.Parm_value = "cwrf_recur=" + model.Cwrf_recur + "&rpt_date=" + report_date + "&hosp_gp=" + model.Budget_cluster + "&up_to_date=" + model.Next_month + "&cur_roll_year=" + financial_year;
                        }
                        else
                        {
                            rptModel.Parm_value = "cwrf_recur=" + model.Cwrf_recur + "&rpt_date=" + next_day_c + "&hosp_gp=" + model.Budget_cluster + "&up_to_date=" + model.Next_month + "&cur_roll_year=" + financial_year;
                        }
                        break;
                    case "cwrf_nonrecur.rpt":
                    case "cwrf_nonrecur_cluster.rpt":
                        rptModel.Parm_value = "cwrf_nonrecur=" + model.Cwrf_nonrecur + "&rpt_date=" + report_date + "&cwrf_rpt_gp=" + model.Cwrf_sort_list + "&closed=" + model.Closed_only;
                        rptModel.Parm_value = rptModel.Parm_value + "&inst_code=" + model.Rpt_cluster + "&next_month=" + model.Next_month + "&cur_roll_year=" + financial_year;
                        break;
                    case "cwrf_imp_project.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + report_date + "&";
                        break;
                    case "cwrf_imp_ha_ref.rpt":
                        rptModel.Parm_value = "prompt0=" + report_date + "&prompt1=" + model.Work_agent + "&prompt2=" + model.Closed_only + "&prompt3=" + model.Cwrf_nonrecur_imp_list + "&prompt4=" + model.Rpt_cluster + "&prompt5=" + financial_year + "&";
                        break;
                    case "sch_v_rpt.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&consolidation_ind=" + model.Rpt_consolidation + "&current_date_c=" + input_for;
                        break;
                    case "statement_of_exp.rpt":
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + rnd.NextDouble() + "&prompt2=" + report_date + "&prompt3=1&";
                        break;
                    case "statement_of_inc.rpt":
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + rnd.NextDouble() + "&prompt2=1&";
                        break;
                    case "statement_of_exp_new.rpt":
                        rptModel.Report_type = "statement_of_exp.rpt";
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + rnd.NextDouble() + "&prompt2=" + report_date + "&prompt3=0&";
                        break;
                    case "statement_of_inc_new.rpt":
                        rptModel.Report_type = "statement_of_inc.rpt";
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + rnd.NextDouble() + "&prompt2=0&";
                        break;
                    case "actual_hc.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_hospital + "&prompt1=" + last_month_c + "&prompt2=" + financial_year + "&";
                        break;
                    case "actual_hc2.rpt":
                        rptModel.Parm_value = "prompt1=" + model.Rpt_hospital + "&prompt2=" + last_month_c + "&prompt0=" + financial_year + "&";
                        break;
                    case "other_inc_asoi.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + model.Rpt_consolidation + "&prompt2=" + last_month_c + "&prompt3=" + financial_year + "&";
                        break;
                    case "forecast_outturn.rpt":
                    case "forecast_outturn2.rpt":
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + financial_year + "&prompt2=N&";
                        break;
                    case "forecast_outturn3.rpt":
                        rptModel.Report_type = "forecast_outturn.rpt";
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + financial_year + "&prompt2=Y&";
                        break;
                    case "forecast_outturn4.rpt":
                        rptModel.Report_type = "forecast_outturn2.rpt";
                        rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + financial_year + "&prompt2=Y&";
                        break;
                    case "cbv.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + last_month_c + "&";
                        break;
                    case "sch_bank_input.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_hospital + "&period_end_date_c=" + input_for;
                        break;
                    case "oc_projection.rpt":
                        rptModel.Parm_value = "prompt0=" + report_date + "&";
                        break;
                    case "project_exception_report.rpt":
                    case "project_exception_report_a4.rpt":
                        rptModel.Report_type = "project_exception_report_cbv.rpt";
                        rptModel.Parm_value = "nature=" + model.Cwrf_recur + "&input_at_c=" + report_date + "&criteria=" + model.Criteria + "&random=" + rnd.NextDouble();
                        rptModel.Parm_value = rptModel.Parm_value + "&rpt_status=" + model.Report_status + "&user_group=" + user_group;
                        rptModel.Parm_value = rptModel.Parm_value + "&inst_code=" + model.Rpt_cluster + "&financial_year_yy=" + financial_year;
                        break;
                    case "rpt_project_funded.rpt":
                        rptModel.Parm_value = "hosp_gp=" + model.Rpt_cluster + "&nature=" + model.Cwrf_recur + "&current_date_c=" + report_date;
                        break;
                    case "etransfer.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + model.Rpt_consolidation + "&prompt3=" + p13 + "&";
                        break;
                    case "rpt_b_epp_program.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + model.Budget_cluster + "&";
                        break;
                    case "rpt_b_bna.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + model.Rpt_cluster + "&";
                        break;
                    case "rpt_b_project_detail.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + model.Rpt_hospital + "&";
                        break;
                    case "rpt_b_pe_adjust.rpt":
                    case "rpt_b_summary_pe_adjust.rpt":
                    case "rpt_b_pe_adjust_by_type.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + model.Rpt_cluster + "&";
                        break;
                    case "rpt_b_recon_oc.rpt":
                    case "rpt_b_recon_pe.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&prompt1=" + model.Rpt_cluster + "&prompt2=" + model.Rpt_consolidation + "&";
                        break;
                    case "rpt_b_project_all_cluster.rpt":
                        rptModel.Parm_value = "prompt0=" + financial_year + "&";
                        break;
                    case "project_insuff_fund.rpt":
                        rptModel.Parm_value = "imp=1&ape_vs_tot_exp=1&rpt_date=" + report_date + "&hospital=PROJ&financial_year_yy=" + financial_year + "&cwrf_recur=" + model.Cwrf_recur;
                        break;
                    case "project_outstanding_commit.rpt":
                        rptModel.Parm_value = "imp=1&ape_vs_tot_exp=2&rpt_date=" + report_date + "&hospital=PROJ&financial_year_yy=" + financial_year + "&cwrf_recur=" + model.Cwrf_recur;
                        break;
                    case "project_cbv_status.rpt":
                        rptModel.Parm_value = "hospital_code=" + model.Rpt_cluster + "&input_at_c=" + report_date + "&project_type=" + model.Proj_type_main_select + "&financial_year_yy=" + financial_year;
                        break;
                    case "detail_list_8100mx.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&rpt_date=" + report_date + "&rpt_type=1&criteria=" + model.Criteria2 + "&next_month=" + model.Next_month + "&sort=" + model.Cluster_sort;
                        break;
                    case "summary_list_8100mx_cluster.rpt":
                    case "summary_list_8100mx.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&rpt_date=" + report_date + "&rpt_type=3&criteria=" + model.Criteria2 + "&next_month=" + model.Next_month;
                        break;
                    case "rpt_8100mx_o3_year.rpt":
                        rptModel.Parm_value = "sort_order=" + model.App_yr_sort + "&imp=1&ape_vs_tot_exp=3&rpt_date=" + report_date + "&hospital=" + model.Rpt_cluster + "&financial_year_yy=" + financial_year + "&cwrf_recur=" + model.Cwrf_recur;
                        break;
                    case "rpt_mnt_adj.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&rpt_date=" + last_month_c + "&financial_year=" + financial_year + "&proj_type=" + model.Cwrf_recur;
                        break;
                    case "rpt_mnt_adj_8100mx.rpt":
                        rptModel.Report_type = "rpt_mnt_adj.rpt";
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + financial_year + "&prompt3=X&";
                        break;
                    case "rpt_8100mx_var.rpt":
                    case "rpt_8100mx_var_actual.rpt":
                        rptModel.Parm_value = "rpt_date=" + last_month_c;
                        break;
                    case "its_trend.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=" + financial_year + "&";
                        break;
                    case "fc_annual_costing.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&consolidate=" + model.Rpt_consolidation + "&financial_year=" + (current_year - 1);
                        break;
                    case "fc_annual_cost_depr.rpt":
                        rptModel.Parm_value = "hospital=" + model.Rpt_cluster + "&consolidate=" + model.Rpt_consolidation + "&financial_year=" + financial_year + "&rpt_type=" + model.Cost_depr;
                        break;
                    case "bssd_cwrf_fe.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + last_month_c + "&prompt2=0&prompt3=" + financial_year + "&prompt4=11&prompt5=I&prompt6=Y&prompt7=0&prompt8=ALL&prompt9=M&prompt10=project&";
                        break;
                    case "pe_proj_vs_actual.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + model.Adjust_type_id + "&prompt2=" + report_date + "&prompt3=" + model.Rpt_consolidation + "&prompt4=" + financial_year + "&";
                        break;
                    case "cwrf_ce_approval_list.rpt":
                        if (model.Next_month == "1")
                            rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=ALL&prompt2=" + model.Cwrf_recur + "&prompt3=" + current_date + "&prompt4=0&";
                        else
                            rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=ALL&prompt2=" + model.Cwrf_recur + "&prompt3=" + last_month_end_c + "&prompt4=0&";
                        break;
                    case "cwrf_pending_deleted.rpt":
                        if (model.Next_month == "1")
                            rptModel.Parm_value = "user_group=" + model.Rpt_cluster + "&nature=" + model.Cwrf_recur + "&as_at_c=" + current_date + "&user_group_ind=0";
                        else
                            rptModel.Parm_value = "user_group=" + model.Rpt_cluster + "&nature=" + model.Cwrf_recur + "&as_at_c=" + last_month_end_c + "&user_group_ind=0";
                        break;
                    case "sch_u.rpt":
                        rptModel.Parm_value = "hosp_gp=" + model.Rpt_cluster + "&account_group=" + model.Account_group + "&period_end_date_c=" + period_end_date_date.ToString("yyyyMMdd") + "&account_code=" + model.Account_code;
                        break;
                    case "manpower_adj.rpt":
                        rptModel.Parm_value = "prompt0=" + model.Rpt_cluster + "&prompt1=" + model.Rpt_consolidation + "&prompt2=" + report_date + "&prompt3=" + financial_year + "&";
                            break;
                    //Financial Vetting
                    case "fv_rpt_trans_vol.rpt":
                        rptModel.Parm_value = "startPeriod=" + model.Startdate_y + Convert.ToInt32(model.Startdate_m).ToString("00") + "&endPeriod=" + model.Enddate_y + Convert.ToInt32(model.Enddate_m).ToString("00");
                        break;
                    case "fv_rpt_leverage_ratio.rpt":
                        rptModel.Parm_value = "";
                        break;
                    case "fv_rpt_ratio_analysis.rpt":
                        rptModel.Parm_value = "tenderer_ids=" + model.Cb_fv_tenderer + "&show_all_years_data=" + model.Show_all_period;
                        break;
                    //End of Financial Vetting
                    default:
                        if (model.Report_type.Substring(model.Report_type.Length - 4) == ".rpt")
                            rptModel.Parm_value = "prompt0=" + last_month_c + "&prompt1=" + model.Rpt_hospital + "&prompt2=" + financial_year + "&";
                        break;
                }
            }

            return rptModel;
        }
        #endregion


    }
}
