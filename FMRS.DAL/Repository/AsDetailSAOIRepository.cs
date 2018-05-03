using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsDetailSAOIRepository
    {
        int CountTotalDetailASOI();
        int CountDetailASOI(string rp_id);
        int GetLatestDetailASOIId();
        DataSet GetDetailASOIByRpId(string Rp_id);
        DataSet GetDetailASOIById(string id);
        void DeleteDetailASOIById(string id);
        void InsertAsDetailASOI(int id, string rp_id, string location, string department, string start_date, string end_date,
                                decimal income, decimal pe, decimal oc, string remarks, string user_name);
        void UpdateAsDetailASOI(int id, string rp_id, string location, string department, string start_date, string end_date,
                                decimal income, decimal pe, decimal oc, string remarks, string user_name);
    }

    public class AsDetailSAOIRepository : IAsDetailSAOIRepository
    {
        private FMRSContext Context;
        public AsDetailSAOIRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public int CountTotalDetailASOI()
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = " select count(*) as num from as_detail_asoi";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0]["num"]);
                    }
                }
            }
            return result;
        }

        public int CountDetailASOI(string rp_id)
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = " select count(*) as num from as_detail_asoi where rp_id = " + rp_id;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0]["num"]);
                    }
                }
            }
            return result;
        }

        public int GetLatestDetailASOIId()
        {
            int result = 0;
            DataSet ds = new DataSet();
            var sql = " select top 1 id from as_detail_asoi order by id desc";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0]["id"]);
                    }
                }
            }
            return result;
        }

        public DataSet GetDetailASOIByRpId(string rp_id)
        {
            DataSet ds = new DataSet();
            var sql = "select * from as_detail_asoi where rp_id = '" + rp_id + "' order by id";
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

        public DataSet GetDetailASOIById(string id)
        {
            DataSet ds = new DataSet();
            var sql = "select * from as_detail_asoi where id = " + id;
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

        public void DeleteDetailASOIById(string id)
        {
            var sql = "delete from as_detail_asoi where id = " + id;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertAsDetailASOI(int id, string rp_id, string location, string department, string start_date, string end_date,
                                decimal income, decimal pe, decimal oc, string remarks, string user_name)
        {
            var sql = " insert into as_detail_asoi values (";
            sql = sql + id + "," + rp_id + ",N'" + location + "',N'" + department + "','" + start_date + "','" + end_date + "',";
            sql = sql + "'" + income + "','" + pe + "','" + oc + "',N'" + remarks + "', '" + user_name + "', GETDATE())";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAsDetailASOI(int id, string rp_id, string location, string department, string start_date, string end_date,
                                decimal income, decimal pe, decimal oc, string remarks, string user_name)
        {
            var sql = " update as_detail_asoi set";
            sql = sql + " prog_desc = N'" + location + "',";
            sql = sql + " prog_organizer = N'" + department + "',";
            sql = sql + " start_date = '" + start_date + "',";
            sql = sql + " end_date = '" + end_date + "',";
            sql = sql + " income = '" + income + "',";
            sql = sql + " pe = '" + pe + "',";
            sql = sql + " oc = '" + oc + "',";
            sql = sql + " remarks = N'" + remarks + "',";
            sql = sql + " update_by = '" + user_name + "',";
            sql = sql + " update_datetime = GETDATE()";
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
    }
}
