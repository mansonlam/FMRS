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
    public interface IReportDetailNotAccessRepository //: IBaseRepository<ReportDetailNotAccess>
    {
        int UpdateReportDetailNotAccessByRptId(string detail_type, string report_id, string loginId, string access_right);
    }
    public class ReportDetailNotAccessRepository : IReportDetailNotAccessRepository //BaseRepository<ReportDetailNotAccess>, IReportDetailNotAccessRepository
    {
        private FMRSContext Context;
        //private DbSet<ReportDetailNotAccess> reportDetailNotAccess;
        public ReportDetailNotAccessRepository(FMRSContext _context) //: base(_context)
        {
            Context = _context;
            //reportDetailNotAccess = Context.Set<ReportDetailNotAccess>();
        }

        public int UpdateReportDetailNotAccessByRptId(string detail_type, string report_id, string loginId, string access_right)
        {
            int can_gen = 0;
            string sql = "";
            string strsql_update = "";
            string id = "";
            int access_right_cnt = 0;
            DataSet ds = new DataSet();
            if (detail_type == "1")
            {
                strsql_update = "delete from report_detail_not_access ";
                strsql_update = strsql_update + " where login_id = '" + loginId + "'";
                strsql_update = strsql_update + " and report_id = " + report_id;
                if (report_id == "17" || report_id == "18")
                {
                    sql = "select id, id_type from budget_gp where budget_mode = 7 and not (id = 31 and id_type = 'I')";
                }
                else
                {
                    sql = "select item.id from item, subgrouping, subgrouping_linking ";
                    sql = sql + " where subgrouping.id = subgrouping_linking.subgrouping_id ";
                    sql = sql + " and item.id = subgrouping_linking.item_id ";
                    sql = sql + " and subgrouping.display_order <> 0 ";
                    sql = sql + " and project_detail_ind != 'N' order by subgrouping.display_order, item.display_order ";
                }
                can_gen = 0;

                var id_type = "";           
                using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                    {
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(ds);
                            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                id = ds.Tables[0].Rows[0]["id"].ToString();
                                if (report_id == "17" || report_id == "18")
                                {
                                    id_type = ds.Tables[0].Rows[0]["id_type"].ToString();
                                }
                                if (id_type == "S")
                                {
                                    int st = access_right.IndexOf("Access_right_s" + id+":") + 16 + id.Length + 1;
                                    access_right_cnt = Convert.ToInt32(access_right.Substring(st,1));
                                }
                                else
                                {
                                    int st = access_right.IndexOf("Access_right_" + id + ":") + 14 + id.Length + 1;
                                    access_right_cnt = Convert.ToInt32(access_right.Substring(st, 1));
                                }
                                if (access_right_cnt == 0)
                                {
                                    if (id_type == "S")
                                    {
                                        strsql_update = strsql_update + "insert into report_detail_not_access ";
                                        strsql_update = strsql_update + " (login_id, report_id, item_id, id_type) ";
                                        strsql_update = strsql_update + " values ";
                                        strsql_update = strsql_update + "('" + loginId + "', " + report_id + ", " + id + ", 'S')";
                                    }
                                    else
                                    {
                                        strsql_update = strsql_update + "insert into report_detail_not_access ";
                                        strsql_update = strsql_update + " (login_id, report_id, item_id, id_type) ";
                                        strsql_update = strsql_update + " values ";
                                        strsql_update = strsql_update + "('" + loginId + "', " + report_id + ", " + id + ", 'I')";
                                    }
                                }
                                else
                                {
                                    can_gen = 1;
                                }
                                id = "";
                                access_right_cnt = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                strsql_update = "delete from report_nature_detail_not_access ";
                strsql_update = strsql_update + " where login_id = '" + loginId + "'";
                strsql_update = strsql_update + " and report_id = " + report_id;

                sql = "select n.nature id, n.description ";
                sql = sql + " from rpt_nature_type n ";
                sql = sql + " order by display_order ";

                can_gen = 0;
                using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                    {
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(ds);
                            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                id = ds.Tables[0].Rows[0]["id"].ToString();
                                int st = access_right.IndexOf("Access_right_" + id + ":") + 14 + id.Length + 1;
                                access_right_cnt = Convert.ToInt32(access_right.Substring(st, 1));
                                if (access_right_cnt == 0)
                                {
                                    strsql_update = strsql_update + "insert into report_nature_detail_not_access ";
                                    strsql_update = strsql_update + " (login_id, report_id, nature) ";
                                    strsql_update = strsql_update + " values ";
                                    strsql_update = strsql_update + "('" + loginId + "', " + report_id + ", '" + id + "')";
                                }
                                else
                                {
                                    can_gen = 1;
                                }
                                id = "";
                                access_right_cnt = 0;
                            }
                        }
                    }
                }

            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(strsql_update, sqlConn))
                {
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return can_gen;
        }



    }
}
