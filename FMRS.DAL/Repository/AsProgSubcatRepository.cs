using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsProgSubcatRepository
    {
        string GetSubcatCheck(string cat_no);
        DataSet GetSubCatByCatNo(string cat_no);
        string GetSubCatByValue(string prog_sub_cat);
    }

    public class AsProgSubcatRepository : IAsProgSubcatRepository
    {
        private FMRSContext Context;
        public AsProgSubcatRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public string GetSubcatCheck(string cat_no)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select top 1 value from as_prog_subcat where cat = " + cat_no;
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

        public DataSet GetSubCatByCatNo(string cat_no)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct description, value, display_order ";
            sql = sql + "from as_prog_subcat where cat = " + cat_no;
            sql = sql + " order by display_order";
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

        public string GetSubCatByValue(string prog_sub_cat)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select distinct description from as_prog_subcat ";
            sql = sql + "where value = '" + prog_sub_cat + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = ds.Tables[0].Rows[0]["description"].ToString();
                    }
                }
            }
            return result;
        }

    }
}
