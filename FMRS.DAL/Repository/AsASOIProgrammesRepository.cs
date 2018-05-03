using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsASOIProgrammesRepository
    {
        DateTime GetPeriod();
        DataSet GetASOIResultList(string hosp_code, string get_max_period, string sch_allcat, string cat_no, string sch_start_date_begin,
                                        string sch_start_date_until, string sch_analytical_start, string sch_analytical_end, string sch_section, string sch_program_code,
                                        string sch_prog_subcat, string sch_description_location, string sch_organizer_department, string sch_nature_income,
                                        string sch_service_provided);
        string GetCatNoById(string list_Id);
        DataSet GetASOIResultById(string list_id);
        int CheckIfRecordExist(string andlytical, string section, string hosp_code, string period, string list_id);
        int CntNumOfRecord();
        int GetLastRecordId();
        void InsertAsASOIProgrammesRecord(int id, string cat_no, string hosp_code, string analytical, string section, string program_code, string no_project,
            string prog_sub_cat, string prog_desc, string prog_organizer, string service_provided, string start_date, string end_date,
            string contract_signed, string  nature_income, string  roll_over, string ytd_income, string ytd_pe, string ytd_oc, string cyp_income, 
            string cyp_pe, string cyp_oc,  string poa_income, string poa_pe , string poa_oc, string remarks, string period, string user_id);

        void UpdateAsASOIProgrammesRecord(string list_id, string analytical, string section, string program_code, string no_project,
            string prog_sub_cat, string prog_desc, string prog_organizer, string service_provided, string start_date, string end_date,
            string contract_signed, string nature_income, string roll_over, string ytd_income, string ytd_pe, string ytd_oc, string cyp_income,
            string cyp_pe, string cyp_oc, string poa_income, string poa_pe, string poa_oc, string remarks, string user_id);
        void DeleteAsASOIProgrammesRecordById(string id);
        void AsUpdateFromAsGL(string period, string hosp_code, string analytical, string section);
    }

    public class AsASOIProgrammesRepository : IAsASOIProgrammesRepository
    {
        private FMRSContext Context;
        public AsASOIProgrammesRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public DateTime GetPeriod()
        {
            DateTime result = new DateTime();
            DataSet ds = new DataSet();
            var sql = " select max(period) as period, year(max(period)) as year, month(max(period)) as month, day(max(period)) as day from as_asoi_programmes";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = DateTime.Parse(ds.Tables[0].Rows[0]["period"].ToString());
                    }
                }
            }
            return result;
        }

        public DataSet GetASOIResultList(string hosp_code, string get_max_period, string sch_allcat, string cat_no, string sch_start_date_begin,
                                        string sch_start_date_until, string sch_analytical_start, string sch_analytical_end, string sch_section, string sch_program_code,
                                        string sch_prog_subcat, string sch_description_location, string sch_organizer_department, string sch_nature_income,
                                        string sch_service_provided)
        {
            DataSet ds = new DataSet();
            var sql = "select id, cat, analytical, section, program_code, prog_sub_cat, prog_desc,";
            sql = sql + " nature_income, prog_organizer, service_provided, start_date, no_project";
            sql = sql + " from as_asoi_programmes where hosp = '" + hosp_code + "'";
            sql = sql + " and period = '" + get_max_period + "'";
            if (sch_allcat != "all")
            {
                sql = sql + " and cat = " + cat_no;
            }
            if (!string.IsNullOrEmpty(sch_start_date_begin))
            {
                string[] temp_start_date_begin = sch_start_date_begin.Split("/");
                string start_date_begin_sql = temp_start_date_begin[2] + "-" + temp_start_date_begin[1] + "-" + temp_start_date_begin[0];
                string[] temp_start_date_until = sch_start_date_until.Split("/");
                string start_date_until_sql = temp_start_date_until[2] + "-" + temp_start_date_until[1] + "-" + temp_start_date_until[0];
                sql = sql + " and start_date >= '" + start_date_begin_sql + "' and start_date <= '" + start_date_until_sql + "'";
            }
            if (!string.IsNullOrEmpty(sch_analytical_start))
            {
                sql = sql + " and analytical between '" + sch_analytical_start + "' and '" + sch_analytical_end + "'";
            }
            if (!string.IsNullOrEmpty(sch_section))
            {
                sql = sql + " and section like '%" + sch_section + "%'";
            }
            if (!string.IsNullOrEmpty(sch_program_code))
            {
                sql = sql + " and lower(program_code) like lower('%" + sch_program_code + "%')";
            }
            if (!string.IsNullOrEmpty(sch_prog_subcat))
            {
                sql = sql + " and prog_sub_cat = " + sch_prog_subcat;
            }
            if (!string.IsNullOrEmpty(sch_description_location))
            {
                string sql_sch_description_location = sch_description_location.Replace("&quot;", "\"\"").Replace("'", "''");
                sql = sql + " and lower(prog_desc) like lower('%" + sql_sch_description_location + "%')";
            }
            if (!string.IsNullOrEmpty(sch_organizer_department))
            {
                string sql_sch_organizer_department = sch_organizer_department.Replace("&quot;", "\"\"").Replace("'", "''");
                sql = sql + " and lower(prog_organizer) like lower('%" + sql_sch_organizer_department + "%')";
            }
            if (!string.IsNullOrEmpty(sch_nature_income))
            {
                sql = sql + " and nature_income = " + sch_nature_income;
            }
            if (!string.IsNullOrEmpty(sch_service_provided))
            {
                sql = sql + " and service_provided = " + sch_service_provided;
            }
            sql = sql + " order by analytical, section";


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

        public string GetCatNoById(string list_Id)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select cat as cat_no from as_asoi_programmes where id = " + list_Id;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = ds.Tables[0].Rows[0]["cat_no"].ToString();
                    }
                }
            }
            return result;
        }

        public DataSet GetASOIResultById(string list_id)
        {
            DataSet ds = new DataSet();
            var sql = "select * from as_asoi_programmes where id = " + list_id ;
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

        public int CheckIfRecordExist(string analytical, string section, string hosp_code, string period, string list_id)
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = " select count(*) as duplicateRecord from [dbo].as_asoi_programmes ";
            sql = sql + " where analytical  = '" + analytical + "' ";
            sql = sql + " and section       = '" + section + "' ";
            sql = sql + " and hosp          = '" + hosp_code + "' ";
            sql = sql + " and period        = '" + period + "' ";
            if (!string.IsNullOrEmpty(list_id))
            { sql = sql + " and id        != '" + list_id + "' "; }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToInt32(ds.Tables[0].Rows[0]["duplicateRecord"]);
                }
            }
            return result;
        }

        public int CntNumOfRecord()
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = " select count(*) as num from as_asoi_programmes";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToInt32(ds.Tables[0].Rows[0]["num"]);
                }
            }
            return result;
        }

        public int GetLastRecordId()
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = " select top 1 id from as_asoi_programmes order by id desc";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);
                }
            }
            return result;
        }

        public void InsertAsASOIProgrammesRecord(int id, string cat_no, string hosp_code, string analytical, string section, string program_code, string no_project,
            string prog_sub_cat, string prog_desc, string prog_organizer, string service_provided, string start_date, string end_date,
            string contract_signed, string nature_income, string roll_over, string ytd_income, string ytd_pe, string ytd_oc, string cyp_income,
            string cyp_pe, string cyp_oc, string poa_income, string poa_pe, string poa_oc, string remarks, string period, string user_id)
        {
            var sql = " insert into as_asoi_programmes values (";
            sql = sql + id + ",";
            sql = sql + cat_no + ",";
            sql = sql + "'" + hosp_code + "',";
            sql = sql + "'" + analytical + "',";
            sql = sql + "'" + section + "',";
            sql = sql + "'" + program_code + "',";
            sql = sql + "'" + no_project + "',";
            sql = sql + "'" + prog_sub_cat + "',";
            sql = sql + "N'" + prog_desc + "',";
            sql = sql + "N'" + prog_organizer + "',";
            sql = sql + "'" + service_provided + "',";
            sql = sql + "'" + start_date + "','" + end_date + "',";
            sql = sql + "'" + contract_signed + "',";
            sql = sql + "'" + nature_income + "',";
            sql = sql + "'" + roll_over + "',";
            sql = sql + cyp_income + "," + cyp_pe + "," + cyp_oc + ",";
            sql = sql + ytd_income + "," + ytd_pe + "," + ytd_oc + ",";
            sql = sql + poa_income + "," + poa_pe + "," + poa_oc + ",";
            sql = sql + "N'" + remarks + "', '" + period + "', '" + user_id + "', GETDATE())";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAsASOIProgrammesRecord(string list_id, string analytical, string section, string program_code, string no_project,
            string prog_sub_cat, string prog_desc, string prog_organizer, string service_provided, string start_date, string end_date,
            string contract_signed, string nature_income, string roll_over, string ytd_income, string ytd_pe, string ytd_oc, string cyp_income,
            string cyp_pe, string cyp_oc, string poa_income, string poa_pe, string poa_oc, string remarks, string user_id)
        {

            var sql = " update as_asoi_programmes set";
            sql = sql + " analytical = '" + analytical + "',";
            sql = sql + " section = '" + section + "',";
            sql = sql + " program_code = '" + program_code + "',";
            sql = sql + " no_project = '" + no_project + "',";
            sql = sql + " prog_sub_cat = '" + prog_sub_cat + "',";
            sql = sql + " prog_desc = N'" + prog_desc + "',";
            sql = sql + " prog_organizer = N'" + prog_organizer + "',";
            sql = sql + " service_provided = '" + service_provided + "',";
            sql = sql + " start_date = '" + start_date + "',";
            sql = sql + " end_date = '" + end_date + "',";
            sql = sql + " contract_signed = '" + contract_signed + "',";
            sql = sql + " nature_income = '" + nature_income + "',";
            sql = sql + " roll_over = '" + roll_over + "',";
            sql = sql + " cyp_income = " + cyp_income + ",";
            sql = sql + " cyp_pe = " + cyp_pe + ",";
            sql = sql + " cyp_oc = " + cyp_oc + ",";
            sql = sql + " ytd_income = " + ytd_income + ",";
            sql = sql + " ytd_pe = " + ytd_pe + ",";
            sql = sql + " ytd_oc = " + ytd_oc + ",";
            sql = sql + " poa_income = " + poa_income + ",";
            sql = sql + " poa_pe = " + poa_pe + ",";
            sql = sql + " poa_oc = " + poa_oc + ",";
            sql = sql + " remarks = N'" + remarks + "',";
            sql = sql + " update_by = '" + user_id + "',";
            sql = sql + " update_datetime = GETDATE()";
            sql = sql + " where id = " + list_id;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAsASOIProgrammesRecordById(string id)
        {
            var sql = " delete from as_asoi_programmes";
            sql = sql + " where id = " + id;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void AsUpdateFromAsGL(string period, string hosp_code, string analytical, string section)
        {
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.as_update_from_as_gl", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@period", period);
                    sqlCmd.Parameters.AddWithValue("@hosp", hosp_code);
                    sqlCmd.Parameters.AddWithValue("@analytical", analytical);
                    sqlCmd.Parameters.AddWithValue("@section", section);
                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
