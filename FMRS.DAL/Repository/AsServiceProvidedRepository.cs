using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsServiceProvidedRepository
    {
        string GetSerProvCheck(string cat_no);
        DataSet GetServiceProvidedByCatNo(string cat_no);
        string GetServiceProvidedById(string service_provided);
    }

    public class AsServiceProvidedRepository : IAsServiceProvidedRepository
    {
        private FMRSContext Context;
        public AsServiceProvidedRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public string GetSerProvCheck(string cat_no)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = " select top 1 id from as_service_provided where cat" + cat_no + " = 'Y'" ;
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

        public DataSet GetServiceProvidedByCatNo(string cat_no)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct id, value, display_order ";
            sql = sql + "from as_service_provided where cat" + cat_no + " = 'Y'";
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

        public string GetServiceProvidedById(string service_provided)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select distinct value from as_service_provided ";
            sql = sql + "where id like '" + service_provided + "'";
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
