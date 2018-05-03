using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IReportGroupDescRepository
    {
        DataSet GetReportGpList(string current_system, string user_inst_code, int cnt, List<ReportViewModel> report_detail_list);
    }
    public class ReportGroupDescRepository : IReportGroupDescRepository
    {
        private FMRSContext Context;
        public ReportGroupDescRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public DataSet GetReportGpList(string current_system, string user_inst_code, int cnt, List<ReportViewModel> report_detail_list)
        {
            DataSet ds = new DataSet();
            var sql = "select group_id, group_desc from report_group_desc where system_code = '" + current_system + "'";
            if (user_inst_code == "HAHO")
            { sql = sql + " and (report_group_desc.access = 'B' or report_group_desc.access = 'C') "; }
            else
            { sql = sql + " and (report_group_desc.access = 'B' or report_group_desc.access = 'H') "; }
            sql = sql + " and (";
            for (var index = 1; index < cnt - 1; index++)
            { sql = sql + " group_id = " + report_detail_list[index].Group_id + " or "; }
            if (current_system == "Y")
            { sql = sql + "group_id = 19 or"; }
            sql = sql + " 0=1) order by group_id";

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
    }
}
