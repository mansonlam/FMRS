using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsNatureIncomeRepository
    {
        string GetNatIncCheck(string cat_no);
        DataSet GetNatureIncomeByCatNo(string cat_no);
        string GetNatureIncomeById(string nature_income);
    }

    public class AsNatureIncomeRepository : IAsNatureIncomeRepository
    {
        private FMRSContext Context;
        public AsNatureIncomeRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public string GetNatIncCheck(string cat_no)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = " select top 1 id from as_nature_income where cat" + cat_no + " = 'Y'" ;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = ds.Tables[0].Rows[0]["id"].ToString();
                    }
                }
            }
            return result;
        }

        public DataSet GetNatureIncomeByCatNo(string cat_no)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct id, value, display_order ";
            sql = sql + "from as_nature_income where cat" + cat_no + " = 'Y'";
            sql = sql + "order by display_order";
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

        public string GetNatureIncomeById(string nature_income)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select distinct value from as_nature_income ";
            sql = sql + "where id like '" + nature_income + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = ds.Tables[0].Rows[0]["value"].ToString();
                    }
                }
            }
            return result;
        }
    }
}
