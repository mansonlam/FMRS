using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IReportRepository
    {
        DataSet GetReportHospGpList(string user_group, string user_inst_code, string ho_access, string login_id, string current_sys);
        DataSet GetReportHospList(string login_id, string current_sys);
        string GetReportSQL(string current_sys, string user_group, string login_id, string privilege_non_pjt_report, string privilege_asoi_rpt, string privilege_report, string ho_access, int report_period);
        DataSet GetReportDetailDataSet(string current_sys, string user_group, string user_inst_code, string login_id, string privilege_non_pjt_report,
            string privilege_asoi_rpt, string privilege_report, string privilege_closing_report, string ho_access, int report_period, string cluster_level);
        DataSet GetRtpItem_list(string user_group, string login_id);
        DataSet GetProjListRptItem(string login_id, string dr_rpt_id);
        int GetCbvAccessControlByUserNameCount(string login_id);
        
        List<int> GetSnapshotYr();
        int GetSnapshotMaxYr();
        List<string> GetSnapshotDateByMaxYr();
        
        DataSet GetWorkAgent();
        string GetBatchNo();

        #region Generate Report
        int GetParmNo(string rpt_name);
        void InsertReportLog(string login_id, string report_type);
        #endregion

        #region User Right
        DataSet GetReportByGroupID(int group_id);
        DataSet GetReportByReportId(string report_id);
        DataSet GetReportDetailRightByDetailTypeRptId(string detail_type, string report_id);
        #endregion
    }
    public class ReportRepository : IReportRepository
    {
        private FMRSContext Context;
        private ICwrfAccessControlRepository CwrfAccessControlRepository;
        private IFlashRptHospGpRepository FlashRptHospGpRepository;
        public ReportRepository(FMRSContext _context, ICwrfAccessControlRepository _cwrfAccessControlRepository,
                                IFlashRptHospGpRepository _flashRptHospGpRepository) 
        {
            Context = _context;
            CwrfAccessControlRepository = _cwrfAccessControlRepository;
            FlashRptHospGpRepository = _flashRptHospGpRepository;
        }
        

        public DataSet GetReportHospGpList(string user_group, string user_inst_code, string ho_access, string login_id, string current_sys)
        {
            DataSet ds = new DataSet();
            var sql = "";
            if (current_sys == "Y")
                sql = FlashRptHospGpRepository.SelectUnionFlashRptHospGpUserGpHosp(user_group);
            else
            {
                if (user_inst_code != "QMH" || user_group != "HOSP")
                {
                    if (ho_access == "Y")
                        sql = FlashRptHospGpRepository.SelectFlashRptHospGp();
                    else
                        sql = "exec user_group_access '" + login_id + "', '" + current_sys + "'";
                }
                else
                    sql = FlashRptHospGpRepository.SelectFlashRptHospGpHospQMH();
            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }

            return ds;
        }
        public DataSet GetReportHospList(string login_id, string current_sys)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.user_group_access", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@login_id", login_id);
                    sqlCmd.Parameters.AddWithValue("@fmrs_system", current_sys);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public string GetReportSQL(string current_sys, string user_group, string login_id, string privilege_non_pjt_report, string privilege_asoi_rpt, string privilege_report, string ho_access, int report_period)
        {
            var sql = "select distinct report_group.group_id, report_group_desc.group_desc, ";
            sql = sql + "        report.id report_id, rpt_name, asp_file, hosp_asp_file, HAHO_ind, hosp_ind, ";
            sql = sql + "        show_hosp_ind, show_pe_type_ind, ";
            sql = sql + "        show_pe_var_type_ind,  show_project_item_ind, ";
            sql = sql + "        show_staff_type_ind, show_cluster_ind, annual_costing_ind, eis_element_ind, ";
            sql = sql + "        activity_ind, consolidation_ind, next_year_ind, fund_gp_ind, res_officer, period, ";
            sql = sql + "        exp_type, with_eis_stat, ";
            sql = sql + "        cbv_rpt_type, cwrf_recur, cwrf_nonrecur, ";
            sql = sql + "        report_group.display_order, budget_cluster, consolidate_individual ";
            sql = sql + " from report_group, report_group_desc, report ";
            sql = sql + " where report_group.group_id =  report_group_desc.group_id ";
            sql = sql + "   and report_group_desc.system_code = '" + current_sys + "'";
            sql = sql + "   and report.id = report_id ";
            sql = sql + "   and (report.id != 276 or '" + user_group + "' = 'ALL' or '" + user_group + "' = 'HAHO') ";
            sql = sql + "   and ((report_group.group_id <> 4 and report_group.group_id <> 9) ";
            sql = sql + "        or not exists (select * from report_not_access rna ";
            sql = sql + "                       where rna.login_id ='" + login_id + "'";
            sql = sql + "                         and rna.report_id = report.id "; 
            sql = sql +     "                         and rna.fmrs_system = '" + current_sys + "')) ";
            sql = sql + "   and ('" + current_sys + "' != 'F' ";
            sql = sql + "        or report_group.group_id = 4 ";
            sql = sql + "        or '" + privilege_non_pjt_report + "' <> 'N')";

            if (privilege_asoi_rpt != "R")
            { sql = sql + " and (report_id <> 298 and report_id <> 299 and report_id <> 300  and report_id <> 301 ) "; }

            if (privilege_asoi_rpt == "R" && privilege_report == "N")
            { sql = sql + " and (report_id = 298 or report_id = 299 or report_id = 300  or report_id = 301 ) "; }

            if (ho_access == "Y")
            {
                sql = sql + " and (report_group_desc.access = 'B' or report_group_desc.access = 'C') ";
                sql = sql + " and HAHO_ind = 'Y' ";
            }
            else
            {
                sql = sql + " and (report_group_desc.access = 'B' or report_group_desc.access = 'H') ";
                sql = sql + " and hosp_ind = 'Y' ";
            }

            if (current_sys == "M")
            {
                var temp_strsql = "";
                int cnt_cbv = GetCbvAccessControlByUserNameCount(login_id);
                temp_strsql = cnt_cbv > 0 ? " cbv_ind = 'Y' " : " cbv_ind = 'N' ";

                if (user_group.Substring(0, 3) == "CWD")
                { temp_strsql = " (" + temp_strsql + " or cwrf_ind = 'C') "; }
                else
                {
                    int cnt_cwrf = CwrfAccessControlRepository.GetCwrfAccessControlByUserName(login_id).Count;
                    if (cnt_cwrf > 0)
                    {
                        if (temp_strsql == "")
                            temp_strsql = " (cwrf_ind = 'Y' or cwrf_ind = 'C')";
                        else
                            temp_strsql = "(" + temp_strsql + " or cwrf_ind = 'Y' or cwrf_ind = 'C') ";
                    }
                    else
                    {
                        if (temp_strsql == "")
                            temp_strsql = " cwrf_ind = 'N' ";
                        else
                            temp_strsql = "(" + temp_strsql + " or cwrf_ind = 'N') ";
                    }
                }

                if (temp_strsql != "")
                    sql = sql + " and " + temp_strsql;
            }

            if ((3 > user_group.Length ? user_group : user_group.Substring(user_group.Length - 3)).ToUpper() == "BSS" && login_id.ToUpper() != "BSS")
            {
                if ((4 > user_group.Length ? user_group : user_group.Substring(user_group.Length - 4)).ToUpper() == "PBSS")
                    sql = sql + " and bss = 'Y' ";
                else
                    sql = sql + " and (bss = 'Y' or bss = 'P')";
            }
            sql = sql + " and report_period <= " + report_period + " order by report_group.group_id, report_group.display_order ";
            return sql;
        }

        public DataSet GetReportDetailDataSet(string current_sys, string user_group, string user_inst_code, string login_id, string privilege_non_pjt_report,
            string privilege_asoi_rpt, string privilege_report, string privilege_closing_report, string ho_access, int report_period, string cluster_level)
        {
            DataSet ds = new DataSet();
            var sql = GetReportSQL(current_sys, user_group, login_id, privilege_non_pjt_report, privilege_asoi_rpt, privilege_report, ho_access, report_period);
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            return ds;
        }

        public DataSet GetRtpItem_list(string user_group, string login_id)
        {
            List<ReportViewModel> result = new List<ReportViewModel>();
            DataSet ds = new DataSet();
            var sql = "select report.id rpt_id, item.id, item.description ";
            sql = sql + " from item, subgrouping, subgrouping_linking, report ";
            sql = sql + " where subgrouping.id = subgrouping_linking.subgrouping_id ";
            sql = sql + "   and item.id = subgrouping_linking.item_id ";
            sql = sql + "   and subgrouping.display_order <> 0 ";
            sql = sql + "   and project_detail_ind = 'Y' ";
            sql = sql + "   and show_project_item_ind = 'Y'";
            sql = sql + "   and (report.id != 276 or '" + user_group + "' = 'ALL' or '" + user_group + "' = 'HAHO') ";
            sql = sql + "   and not exists (select * from report_detail_not_access r ";
            sql = sql + "                   where r.report_id = report.id ";
            sql = sql + "                     and r.login_id = '" + login_id + "'";
            sql = sql + "                     and r.item_id = item.id) ";
            sql = sql + " order by report.id, subgrouping.display_order, item.display_order ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            return ds;
        }

        public DataSet GetProjListRptItem(string login_id, string dr_rpt_id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.get_project_list_rpt_item", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@login_id", login_id);
                    sqlCmd.Parameters.AddWithValue("@report_id", dr_rpt_id);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        // cbv_access_control
        public int GetCbvAccessControlByUserNameCount(string login_id)
        {
            int result = 0;
            login_id = login_id.Replace("\\", "/").ToLower();
            DataSet ds = new DataSet();
            var sql = "select count(*) cnt_cbv from cbv_access_control where user_name = '" + login_id + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0]["cnt_cbv"]);
                    }
                }
            }
            return result;
        }

        

        // as_snapshot
        public List<int> GetSnapshotYr()
        {
            List<int> result = new List<int>();
            DataSet ds = new DataSet();
            var sql = "select distinct year from as_snapshot order by year desc";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            result.Add(Convert.ToInt32(dr["year"]));
                        }
                    }
                }
            }
            return result;
        }

        public int GetSnapshotMaxYr()
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = "select max(year) as year from as_snapshot";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    }
                }
            }
            return result;
        }

        public List<string> GetSnapshotDateByMaxYr()
        {
            List<string> result = new List<string>();
            DataSet ds = new DataSet();
            var sql = " select distinct date from as_snapshot ";
            sql = sql + " where year = (select max(year) as year from as_snapshot) ";
            sql = sql + " order by date desc ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            result.Add(dr["date"].ToString());
                        }
                    }
                }
            }
            return result;
        }

        public DataSet GetWorkAgent()
        {
            DataSet ds = new DataSet();
            var sql = "select distinct wk_agent from cwrf_wk_project ";
            sql = sql + " order by wk_agent ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            return ds;
        }

        public string GetBatchNo()
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select isnull(max(convert(int, batch_no)), 0) batch_no ";
            sql = sql + " from cwrf_annex_detail ";
            sql = sql + " where endorsed = 'Y' "; 
            sql = sql + " and(ref_no like 'EMR%' or ref_no like 'IMP%' or ref_no like 'PMN%') ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0].Rows[0]["batch_no"].ToString();
                    }
                }
            }
            return result;
        }


        #region Generate Report
        public int GetParmNo(string rpt_name)
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = "select parm_no from report where id = " + rpt_name;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0]["parm_no"]);
                    }
                }
            }
            return result;
        }

        public void InsertReportLog(string login_id, string report_type)
        {
            var sql = "insert into report_log values ('" + login_id + "', '" + report_type + "', getdate())";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }
        #endregion

        #region User Right
        public DataSet GetReportByGroupID(int group_id)
        {
            DataSet ds = new DataSet();
            var sql = "select id, rpt_name, g.display_order, show_project_item_ind, cwrf_recur from report r, report_group g";
            sql = sql + " where group_id = " + group_id;
            sql = sql + " and report_id = id ";
            sql = sql + " and hosp_ind = 'Y' ";
            sql = sql + " and report_period <= 2 ";
            sql = sql + " order by g.display_order ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public DataSet GetReportByReportId(string report_id)
        {
            DataSet ds = new DataSet();
            var sql = "select * from report where id='" + report_id + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            return ds;
        }

        public DataSet GetReportDetailRightByDetailTypeRptId(string detail_type, string report_id)
        {
            DataSet ds = new DataSet();
            var sql = "";
            if (detail_type == "1")
            {
                if (report_id == "17" || report_id == "18")
                {
                    sql = "select b.*, (case when i.description is not null then i.description else s.description end) as description from budget_gp b left join item i ";
                    sql = sql + "on b.id = i.id and b.id_type = 'I' left join subgrouping s ";
                    sql = sql + "on b.id = s.id and b.id_type = 'S' where b.budget_mode = 7 ";
                    sql = sql + "and not (b.id = 31 and b.id_type = 'I') ";
                    sql = sql + "order by b.group_id, b.display_order";
                }
                else
                {
                    sql = "select item.id, item.description from item, subgrouping, subgrouping_linking ";
                    sql = sql + " where subgrouping.id = subgrouping_linking.subgrouping_id ";
                    sql = sql + " and item.id = subgrouping_linking.item_id ";
                    sql = sql + " and subgrouping.display_order <> 0 ";
                    sql = sql + " and project_detail_ind != 'N' order by subgrouping.display_order, item.display_order ";
                }
            }
            else
            {
                sql = "select n.nature id, n.description ";
                sql = sql + " from rpt_nature_type n ";
                sql = sql + " order by display_order ";
            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            return ds;
        }
        #endregion
    }
}
