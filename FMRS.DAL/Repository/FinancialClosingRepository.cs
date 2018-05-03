using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IFinancialClosingRepository
    {
        DataSet GetFVInd();
        DataSet GetFVIndTenderer();
        DataSet GetFundGp();
        DataSet GetProjItemIndList();
        DataSet GetCurrentPeriodByReportDate(string report_date);
        int GetTrendTbCntByValueDate(string value_date);
        DataSet GetDonationMovement(string inst_code, string value_date, int financial_year, int recon_type, int fund);
        FVUser GetFVUserByLoginId(string loginId);
        void FvUpdateUser(string domain_user, string fvInput, string fvCluster, string fvUserAdmin);
    }
    public class FinancialClosingRepository : IFinancialClosingRepository
    {
        private FMRSContext Context;

        public FinancialClosingRepository(FMRSContext _context) 
        {
            Context = _context;
        }

        public DataSet GetFVInd()
        {
            DataSet ds = new DataSet();
            var sql = "select id, industry_name from financial_closing..fv_industry order by id";
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
        public DataSet GetFVIndTenderer()
        {
            DataSet ds = new DataSet();
            var sql = "SELECT a.id,a.tenderer_name,a.industry_id,b.industry_name FROM financial_closing..fv_tenderer a, financial_closing..fv_industry b  where a.industry_id = b.id order by industry_id, tenderer_name";
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

        public DataSet GetFundGp()
        {
            DataSet ds = new DataSet();
            var sql = "select distinct code from financial_closing..fund";
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

        public DataSet GetProjItemIndList()
        {
            DataSet ds = new DataSet();
            var sql = "select id, description from financial_closing.dbo.sch_q_item where  subgrouping_id = 1030";
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

        public DataSet GetCurrentPeriodByReportDate(string report_date)
        {
            DataSet ds = new DataSet();
            var sql = "select period_for from financial_closing..input_period ";
            sql = sql + " where input_period_from <= '" + report_date.Substring(4, 4);
            sql = sql + "' and input_period_to >= '" + report_date.Substring(4, 4) + "'";
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

        public int GetTrendTbCntByValueDate(string value_date)
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = "select count(1) cnt from financial_closing..trend_tb ";
            sql = sql + " where input_for = '" + value_date + "'";
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

        public DataSet GetDonationMovement(string inst_code, string value_date, int financial_year, int recon_type, int fund)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.get_donation_movement", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@hosp_gp", inst_code);
                    sqlCmd.Parameters.AddWithValue("@current_date", value_date);
                    sqlCmd.Parameters.AddWithValue("@financial_year", financial_year);
                    sqlCmd.Parameters.AddWithValue("@recon_type", recon_type);
                    sqlCmd.Parameters.AddWithValue("@fund", fund);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public FVUser GetFVUserByLoginId(string loginId)
        {
            FVUser result = new FVUser();
            DataSet ds = new DataSet();
            var sql = "select * from financial_closing.dbo.fv_user_profile where domain_id = '" + loginId.ToUpper().Replace("/", "\\") + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result.FV_user_role = ds.Tables[0].Rows[0]["user_role"].ToString();
                        result.FV_user_cluster = ds.Tables[0].Rows[0]["cluster"].ToString();
                        result.FV_user_admin2 = ds.Tables[0].Rows[0]["user_admin"].ToString();
                    }
                }
            }
            return result;
        }

        public void FvUpdateUser(string domain_user, string fvInput, string fvCluster, string fvUserAdmin)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.fv_update_user", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@domain_user", domain_user);
                    sqlCmd.Parameters.AddWithValue("@role_cde", fvInput);
                    sqlCmd.Parameters.AddWithValue("@cluster", fvCluster);
                    sqlCmd.Parameters.AddWithValue("@user_admin", fvUserAdmin);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
        }
    }
}
