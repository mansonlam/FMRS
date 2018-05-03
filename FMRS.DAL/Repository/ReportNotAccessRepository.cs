using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace FMRS.DAL.Repository
{
    public interface IReportNotAccessRepository //: IBaseRepository<ReportNotAccess>
    {
        int GetReportCntByLoginIdRptID(string loginId, string reportId, string modules);
        int GetReportCntByLoginIdRptIDIdType(string admin_login_id, string report_id, string detail_type, string id, string id_type);
        void UpdateReportNotAccessByCanGen(int can_gen, string admin_login_id, string report_id);
        void UpdateReportNotAccessByLoginIdRptId(ProjectReportViewModel model);
        void InsertReportNotAccessByLoginId(string update_login_id, string loginn_id, string modules);
    }
    public class ReportNotAccessRepository : IReportNotAccessRepository //BaseRepository<DonCat>, IReportNotAccessRepository
    {
        private FMRSContext Context;
        //private DbSet<ReportNotAccess> reportNotAccess;
        public ReportNotAccessRepository(FMRSContext _context) //: base(_context)
        {
            Context = _context;
            //reportNotAccess = Context.Set<ReportNotAccess>();
        }

        public int GetReportCntByLoginIdRptID(string loginId, string reportId, string modules)
        {
            int cnt = 0;
            DataSet ds = new DataSet();
            var sql = "select count(*) cnt from report_not_access ";
            sql = sql + " where login_id = '" + loginId + "'";
            sql = sql + "   and report_id = " + reportId;
            sql = sql + "   and fmrs_system = '" + modules + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            cnt = Convert.ToInt32(ds.Tables[0].Rows[0]["cnt"].ToString());
                        }
                    }
                }
            }
            return cnt;
        }

        public int GetReportCntByLoginIdRptIDIdType(string admin_login_id, string report_id, string detail_type, string id, string id_type)
        {
            int cnt = 0;
            DataSet ds = new DataSet();
            var sql = "";
            if (detail_type == "1")
            {
                if (report_id == "17" || report_id == "18")
                {
                    sql = "select count(*) cnt from report_detail_not_access ";
                    sql = sql + " where login_id = '" + admin_login_id + "'";
                    sql = sql + " and report_id = " + report_id;
                    sql = sql + " and item_id = " + id;
                    sql = sql + " and id_type = '" + id_type + "'";
                }
                else
                {
                    sql = "select count(*) cnt from report_detail_not_access ";
                    sql = sql + " where login_id = '" + admin_login_id + "'";
                    sql = sql + " and report_id = " + report_id;
                    sql = sql + " and item_id = " + id;
                }
            }
            else
            {
                sql = "select count(*) cnt from report_nature_detail_not_access ";
                sql = sql + " where login_id = '" + admin_login_id + "'";
                sql = sql + " and report_id = " + report_id;
                sql = sql + " and nature = '" + id + "'";
            }

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            cnt = Convert.ToInt32(ds.Tables[0].Rows[0]["cnt"].ToString());
                        }
                    }
                }
            }
            return cnt;
        }

        public void UpdateReportNotAccessByCanGen(int can_gen, string admin_login_id, string report_id)
        {
            var sql = "";
            if (can_gen == 1)
            {
                sql = "if exists (select * from report_not_access ";
                sql = sql + " where login_id = '" + admin_login_id + "'";
                sql = sql + " and report_id = " + report_id + ") ";
                sql = sql + " begin ";
                sql = sql + " delete report_not_access ";
                sql = sql + " where login_id = '" + admin_login_id + "'";
                sql = sql + " and report_id = " + report_id;
                sql = sql + " end ";
            }
            else
            {
                sql = "if not exists (select * from report_not_access ";
                sql = sql + " where login_id = '" + admin_login_id + "'";
                sql = sql +" and report_id = " + report_id + ") ";
                sql = sql +" begin ";
                sql = sql +" insert into report_not_access ";
                sql = sql +" (login_id, report_id) ";
                sql = sql +" values ";
                sql = sql +"('" + admin_login_id + "', " + report_id + ")";
                sql = sql +" end ";
            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateReportNotAccessByLoginIdRptId(ProjectReportViewModel model)
        {
            var strsql_update = "delete from report_not_access ";
            strsql_update = strsql_update + " where login_id = '" + model.Admin_login_id + "' and fmrs_system = '" + model.Modules + "'";

            var sql = "select id, rpt_name, g.display_order  from report r, report_group g";
            sql = sql + " where group_id = " + model.Group_id;
            sql = sql + " and report_id = id ";
            sql = sql + " and hosp_ind = 'Y' ";
            sql = sql + " order by g.display_order ";

            string report_id = "";
            int access_right_cnt = 0;
            DataSet ds = new DataSet();
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
                            report_id = dr["id"].ToString();
                            int st = model.Access_right.IndexOf("Access_right_" + report_id + ":") + 14 + report_id.Length + 1;
                            access_right_cnt = Convert.ToInt32(model.Access_right.Substring(st, 1));
                            if (access_right_cnt == 0)
                            {
                                strsql_update = strsql_update + "insert into report_not_access ";
                                strsql_update = strsql_update + " (login_id, report_id, fmrs_system) ";
                                strsql_update = strsql_update + " values ";
                                strsql_update = strsql_update + "('" + model.Admin_login_id + "', " + report_id + ", '" + model.Modules + "')";
                            }
                        }
                    }
                }
            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(strsql_update, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }

        }

        public void InsertReportNotAccessByLoginId(string update_login_id, string login_id, string modules)
        {
            var sql = "insert into report_not_access (login_id, report_id, fmrs_system) ";
            sql = sql + "select '" + update_login_id + "', r1.report_id, r1.fmrs_system ";
            sql = sql + "from report_not_access r1 left join report_not_access r2 ";
            sql = sql + "    on (r1.report_id = r2.report_id ";
            sql = sql + "        and r1.fmrs_system = r2.fmrs_system ";
            sql = sql + "        and r2.login_id = '" + update_login_id + "') ";
            sql = sql + "where r1.login_id = '" + login_id + "' ";
            sql = sql + "  and r1.fmrs_system = '" + modules + "' ";
            sql = sql + "  and r2.login_id is null "; 
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

    }
}
