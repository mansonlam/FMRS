using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IDonationRemarkRepository //: IBaseRepository<DonationRemark>
    {
        string GetRemarkByHosp(string inst_code);
        void DeleteRemarkByHosp(string inst_code);
        void InsertRemarkByHosp(string inst_code, string remark, string login_id);
    }
    public class DonationRemarkRepository : IDonationRemarkRepository
    {
        private FMRSContext Context;
        //private DbSet<DonationReserve> donationReserve;
        public DonationRemarkRepository(FMRSContext _context) //: base(_context)
        {
            Context = _context;
            //donationRemark = Context.Set<DonationRemark>();
        }

        public string GetRemarkByHosp(string inst_code)
        {
            DataSet ds = new DataSet();
            string result ="";
            var sql = "select remark from donation_remark";
            sql = sql + "  where hospital = '" + inst_code + "' ";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            return result;
        }

        public void DeleteRemarkByHosp(string inst_code)
        {
            var sql = "delete donation_remark ";
            sql = sql + "  where hospital = '" + inst_code + "' ";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertRemarkByHosp(string inst_code, string remark, string login_id)
        {
            var sql = "insert into donation_remark ";
            sql = sql + " (hospital, remark, input_by, input_at) ";
            sql = sql + " values ";
            sql = sql + " ('" + inst_code + "', '" + remark + "', '" + login_id + "', getdate())";

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
