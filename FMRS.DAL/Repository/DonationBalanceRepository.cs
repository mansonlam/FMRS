using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Globalization;

namespace FMRS.DAL.Repository
{
    public interface IDonationBalanceRepository : IBaseRepository<DonationBalance>
    {
        List<DonationBalance> GetAllDonationBalance();
        List<DonationBalance> GetDonationBalance(string hospital, string fund_code, string section_code, string analytical_code, string value_date);
        int GetBalAlert(string hospital, string fund_code, string section_code, string analytical_code, string value_date);
        DataSet GetReserveBal(string hospital, string fund_code, string section_code, string analytical_code, string value_date);
    }
    public class DonationBalanceRepository : BaseRepository<DonationBalance>, IDonationBalanceRepository
    {
        private FMRSContext Context;
        private DbSet<DonationBalance> donationBalance;
        public DonationBalanceRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donationBalance = Context.Set<DonationBalance>();
        }

        public List<DonationBalance> GetAllDonationBalance()
        {
            return Context.DonationBalance.ToList();
        }

        public List<DonationBalance> GetDonationBalance(string hospital, string fund_code, string section_code, string analytical_code, string value_date)
        {
            DateTime date; 
            DateTime.TryParseExact(value_date, "yyyyMMdd", new CultureInfo("en-US"), DateTimeStyles.None, out date);
            return Context.DonationBalance.Where(d => d.Hospital == hospital 
                                                    && d.Fund == fund_code 
                                                    && d.Section == section_code 
                                                    && d.Analytical == analytical_code 
                                                    && d.InputFor == date).ToList();
        }

        public int GetBalAlert(string hospital, string fund_code, string section_code, string analytical_code, string value_date)
        {
            DateTime date;
            DateTime.TryParseExact(value_date, "yyyyMMdd", new CultureInfo("en-US"), DateTimeStyles.None, out date);
            DataSet ds = new DataSet();
            int result = 0;
            var sql = "SELECT case when reserve_bal_begin > 0 or reserve_bal_end > 0 then 1 else 0 end  as 'reserve_bal_alert' ";
            sql = sql + " from donation_balance ";
            sql = sql + " where hospital ='" + hospital + "' and fund ='" + fund_code + "' and section ='" + section_code + "' and";
            sql = sql + " analytical ='" + analytical_code + "' and input_for = '" + date + "'";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if(ds.Tables[0].Rows.Count > 0)
                    { 
                        result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    }
                }
            }
            return result;
        }

        public DataSet GetReserveBal(string hospital, string fund_code, string section_code, string analytical_code, string value_date)
        {
            DateTime date;
            DateTime.TryParseExact(value_date, "yyyyMMdd", new CultureInfo("en-US"), DateTimeStyles.None, out date);
            DataSet ds = new DataSet();
            var sql = "select reserve_bal_begin, reserve_bal_end from donation_balance ";
            sql = sql + " where input_for = '" + date + "'";
            sql = sql + " and (reserve_bal_begin >= 500000 or reserve_bal_end >= 500000) ";
            sql = sql + " and hospital ='" + hospital + "' and fund ='" + fund_code + "'";
            sql = sql + " and section ='" + section_code + "' and analytical ='" + analytical_code + "'";

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
