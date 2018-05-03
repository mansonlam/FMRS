using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class ReportViewModel
    {
        public int Report_Id { get; set; }
        public string Annual_costing_ind { get; set; }
        public string HAHO_ind { get; set; }
        public string Hosp_ind { get; set; }
        public string Asp_file { get; set; }
        public string Hosp_asp_file { get; set; }
        public string Show_hosp_ind { get; set; }
        public string Show_pe_type_ind { get; set; }
        public string Show_pe_var_type_ind { get; set; }
        public string Show_project_item_ind { get; set; }
        public string Show_staff_type_ind { get; set; }
        public string Show_cluster_ind { get; set; }
        public string Eis_element_ind { get; set; }
        public string Activity_ind { get; set; }
        public string Consolidation_ind { get; set; }
        public string Next_year_ind { get; set; }
        public string Fund_gp_ind { get; set; }
        public string Res_officer { get; set; }
        public string Period { get; set; }
        public string Exclude { get; set; }
        public string With_eis_stat { get; set; }
        public string Cbv_rpt_type { get; set; }
        public string Cwrf_recur { get; set; }
        public string Cwrf_recur_list { get; set; }
        public string Cwrf_nonrecur { get; set; }
        public string Cwrf_nonrecur_list { get; set; }
        public string Budget_cluster { get; set; }
        public string Budget_cluster_hosp { get; set; }
        public string Consolidate_individual { get; set; }


        public string Export_to_excel { get; set; }
        public string Flash_rpt_date { get; set; }
        public string Flash_rpt_date2 { get; set; }
        public string Rpt_group { get; set; }
        public string Rpt_name { get; set; }
        public string Adjust_type_id { get; set; }
        public string Rpt_cluster { get; set; }
        public string Rpt_hospital { get; set; }
        public string Inst_code { get; set; }
        public string Rpt_item_id { get; set; }
        public string Rpt_consolidation { get; set; }
        public string Rpt_next_year { get; set; }
        public string Fund_gp { get; set; }
        public string Group_id { get; set; }
        public string Exp_type{ get; set; }
        public string Proj_type_select { get; set; }
        public string Work_agent { get; set; }
        public string Closed_only { get; set; }
        public string Top_10 { get; set; }
        public string Criteria { get; set; }
        public string Criteria2 { get; set; }
        public string Report_status { get; set; }
        public string Cwrf_nonrecur_imp_list { get; set; }
        public string One_line { get; set; }
        public string Proj_type_main_select { get; set; }
        public string Cwrf_sort_list { get; set; }
        public string Proj_sort_list { get; set; }
        public string Rec_type { get; set; }
        public string Rpt_format { get; set; }
        public string Don_purpose { get; set; }
        public string Don_fund { get; set; }
        public string Deleted { get; set; }
        public string Don_type { get; set; }
        public string As_at { get; set; }
        public string As_at_y { get; set; }
        public string As_at_m { get; set; }
        public string Basis { get; set; }
        public string Batch_no { get; set; }
        public string Don_cat { get; set; }
        public string Next_month { get; set; }
        public string Cluster_sort { get; set; }
        public string App_yr_sort { get; set; }
        public string Cost_depr { get; set; }
        public string Hosp_cluster { get; set; }
        public string In_million { get; set; }
        public string Trust_only { get; set; }
        public string Account_group { get; set; }
        public string Account_code { get; set; }
        public string Snapshot { get; set; }
        public string Snapshot_m { get; set; }
        public string Startdate_m { get; set; }
        public string Startdate_y { get; set; }
        public string Enddate_m { get; set; }
        public string Enddate_y { get; set; }
        public string Ddl_fv_ind { get; set; }
        public string Cb_fv_tenderer { get; set; }
        public string Show_all_period { get; set; }
        public string Report_name { get; set; }
        public string Report_format { get; set; }
        public string Report_type { get; set; }
        public string Modules { get; set; }
        public List<ReportViewModel> Report_detail_list { get; set; }
        public int Cnt { get; set; }
        public Dictionary<string, string> Consolidate_ind_dict { get; set; }
        public List<FVIndModel> FVIndTenderer_list { get; set; }
        public List<RptItemModel> Rtp_item_list { get; set; }
        public List<ReportGroupModel> Report_group_full_list { get; set; }
        #region Form Control
        public List<SelectListItem> AsAtList { get; set; }
        public List<SelectListItem> AsAtMList { get; set; }
        public List<SelectListItem> FlashRptDateList { get; set; }
        public List<SelectListItem> FlashRptDateList2 { get; set; }
        #endregion
    }

    public class FVIndModel
    {
        public string Fv_ind_id { get; set; }
        public string Fv_ind_name { get; set; }
        public List<FVTendererModel> Tenderer_list { get; set; }
    }

    public class FVTendererModel
    {
        public string Fv_tenderer_id { get; set; }
        public string Fv_tenderer_name { get; set; }
    }
    public class RptItemModel
    {
        public string Rpt_id { get; set; }
        public List<TableRptItemModel> Table_rtp_item_list { get; set; }
    }
    public class TableRptItemModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }

    public class ReportGroupModel
    {
        public string Group_id { get; set; }
        public int Rpt_cnt { get; set; }
        public List<string> Report_id_list { get; set; }
    }

    public class GenReportModel
    {
        public string Report_name { get; set; }
        public string Rpt_name { get; set; } //ID for Report_name
        public string Report_format { get; set; }
        public string Report_type { get; set; }
        public string Report_group { get; set; }
        public string Rpt_group { get; set; }//ID for Report_group
        public int Parm_no { get; set; }
        public string Parm_value { get; set; }
        public string Current_period { get; set; }
        
    }
}
