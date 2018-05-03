using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;

namespace FMRS.DAL.Repository
{
    public interface IDonationReserveRepository //: IBaseRepository<DonationReserve>
    {
        DataSet GetDonationReserveSummary(string value_date, string inst_code);
        string InsertDonationReserve(string hospital, string fund_code, string section_code, string analytical_code, string don_kind_desc, string donor_name,
             string value_date, DateTime don_date, int don_super_cat, decimal r_begin, decimal ytd_inc, decimal ytd_exp, string don_specific);
    }
    public class DonationReserveRepository : IDonationReserveRepository// BaseRepository<DonationReserve>, IDonationReserveRepository
    {
        private FMRSContext Context;
        //private DbSet<DonationReserve> donationReserve;
        public DonationReserveRepository(FMRSContext _context) //: base(_context)
        {
            Context = _context;
            //donationReserve = Context.Set<DonationReserve>();
        }

        public DataSet GetDonationReserveSummary(string value_date, string inst_code)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.donation_reserve_summary", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@input_at", value_date);
                    sqlCmd.Parameters.AddWithValue("@hospital", inst_code);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public string InsertDonationReserve(string hospital, string fund_code, string section_code, string analytical_code, string don_kind_desc, string donor_name,
             string value_date, DateTime don_date, int don_super_cat, decimal r_begin, decimal ytd_inc, decimal ytd_exp, string don_specific)
        {
            string sql = "insert into donation_reserve ";
            sql = sql + " (detail_id, hospital, fund, section, analytical, don_desc, donor_name, input_at, receive_dt, don_cat, ";
            sql = sql + " reserve_bal_begin, income, expenditure, cr_by, cr_dt, updt_by, updt_dt) ";
            sql = sql + " select d.id, '" + hospital + "', '" + fund_code + "', '" + section_code + "', '" + analytical_code + "', ";
            sql = sql + "  '" + don_kind_desc + "', '" + donor_name + "', '" + value_date + "',  ";
            sql = sql + " year(dateadd(mm, -3, '" + don_date + "')), " + don_super_cat + ", '" + r_begin + "', ";
            sql = sql + " " + ytd_inc + ", " + ytd_exp + ", 'upload', getdate(), 'upload', getdate() ";
            sql = sql + " from donation_detail d ";
            sql = sql + " where exists (select * from donation_reserve r where ";
            sql = sql + " r.hospital = d.hospital and r.fund = d.fund and r.section = d.section and r.analytical = d.analytical) ";
            sql = sql + " and d.id in (select max(id) from donation_detail where hospital = '" + hospital + "' and fund = '" + fund_code + "' and section = '" + section_code + "'";
            sql = sql + " and analytical = '" + analytical_code + "' and don_specific = '" + don_specific + "'";
            sql = sql + " and don_kind_desc = '" + don_kind_desc + "')  ";
            return sql;
        }
    }
}
