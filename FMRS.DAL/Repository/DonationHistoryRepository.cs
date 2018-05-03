using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace FMRS.DAL.Repository
{
    public interface IDonationHistoryRepository //: IBaseRepository<DonationHistory>
    {

        decimal GetBalForward(int id, int financial_year);
        decimal GetRefund(int id, int financial_year);
        DataSet GetRefundDetail(int id, int financial_year, string rec_type);
        int UpdateDonationRefund(int id, int financial_year, string rec_type, int rec_cnt, string login_id, List<DonationRefundDetail> refund_detail);
        decimal GetCostA(string inst_code, int show_year, int financial_year, string in_donor_name);
        decimal GetCostB(string inst_code, int show_year, int financial_year, string in_donor_name);
        DataSet GetDoantionByYear(string inst_code, string in_donor_name);
        DataSet GetDonationHistoryByIdFinYr(int id, int financial_year);
        void DeleteDonation(string login_id, int id);
        void DeleteDonationHistory(int id, int financial_year);
        void InsertDonationHistory(int id, List<PreviousRecord> CM_record, List<PreviousRecord> Previous_record, string login_id, string don_type);
        void InsertDonationHistoryForNewDonation(List<PreviousRecord> CM_record, List<PreviousRecord> Previous_record, string login_id);
        string InsertDonationHistory(decimal don_cur_mth, int out_comm, DateTime don_date, string login_id);
    }
    public class DonationHistoryRepository : IDonationHistoryRepository// BaseRepository<DonationHistory>, IDonationHistoryRepository
    {
        private FMRSContext Context;
        //private DbSet<DonationHistory> donHistory;
        public DonationHistoryRepository(FMRSContext _context) //: base(_context)
        {
            Context = _context;
            //donHistory = Context.Set<DonationHistory>();
        }


        public decimal GetBalForward(int id, int financial_year)
        {
            DataSet ds = new DataSet();
            decimal result = 0;
            var sql = "select isnull(sum(cost), 0) cost ";
            sql = sql + " from donation_history ";
            sql = sql + " where id = " + id;
            sql = sql + " and original_don_date < '" + financial_year + "0401' and rec_type = ''";
            sql = sql + " and (" + financial_year + "<= 2003 or input_for < '" + financial_year + "0401')";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
                }
            }
            return result;
        }

        public decimal GetRefund(int id, int financial_year)
        {
            decimal result = 0;
            DataSet ds = new DataSet();
            var sql = "select isnull(sum(cost), 0) cost ";
            sql = sql + " from donation_history ";
            sql = sql + " where input_for >= '" + financial_year + "0401'";
            sql = sql + " and input_for < dateadd(yy, 1, '" + financial_year + "0401')";
            sql = sql + " and id = " + id;
            sql = sql + " and rec_type = 'R' ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
                }
            }
            return result;
        }

        public DataSet GetRefundDetail(int id, int financial_year, string rec_type)
        {
            DataSet ds = new DataSet();
            var sql = "select input_for, convert(char(8), input_for, 112) refund_date_yyyymmdd, ";
            sql = sql + " convert(char(8), input_for, 3) refund_date_ddmmyy, ";
            sql = sql + " original_don_date, convert(char(8), original_don_date, 112) original_don_date_yyyymmdd, ";
            sql = sql + " convert(char(8), original_don_date, 3) original_don_date_ddmmyy, ";
            sql = sql + " cost ";
            sql = sql + " from donation_history ";
            sql = sql + " where id = " + id;
            sql = sql + " and rec_type = '" + rec_type + "'";
            if (rec_type == "")
            {
                sql = sql + "and  original_don_date < '" + financial_year + "0401'";
                sql = sql + " and (" + financial_year + "<= 2003 or input_for < '" + financial_year + "0401')";
            }
            else
            {
                sql = sql + " and original_don_date >= '" + financial_year + "0401'";
                sql = sql + " and original_don_date < '" + (financial_year + 1) + "0401'";
            }
            sql = sql + " order by original_don_date desc ";
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

        public int UpdateDonationRefund(int id, int financial_year, string rec_type, int rec_cnt, string login_id, List<DonationRefundDetail> refund_detail)
        {
            var sql = "delete donation_history ";
            sql = sql + " where id = " + id;
            if (rec_type == "")
                sql = sql + " and original_don_date < '" + financial_year + "0401'";
            else
            { 
                sql = sql + " and original_don_date >= '" + financial_year + "0401'";
                sql = sql + " and original_don_date < '" + (financial_year + 1) + "0401'";
            }
            sql = sql + " and rec_type = '" + rec_type + "';";
            for (int i =0 ; i < rec_cnt; i++)
            {
                sql = sql + " insert into donation_history ";
                sql = sql + " (id, cost, out_comm, input_for, original_don_date, last_update_by, last_update_at, rec_type) ";
                sql = sql + " values ";
                sql = sql + "(" + id + ", " + refund_detail[i].Cost + ", 0, convert(datetime, '" + refund_detail[i].Refund_date_ddmmyy + "', 3), convert(datetime, '" + refund_detail[i].Original_don_date + "', 3), '" + login_id + "', getdate(), '" + rec_type + "')";
            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    return sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetCostA(string inst_code, int show_year, int financial_year, string in_donor_name)
        {
            DataSet ds = new DataSet();
            decimal result = 0;
            var sql = "select isnull(sum(cost), 0) cost ";
            sql = sql + " from donation_history h, flash_rpt_hosp_gp g, donation_detail d left join don_cat c";
            sql = sql + " on d.don_cat = c.id ";
            sql = sql + " where h.id = d.id ";
            sql = sql + " and isnull(c.designated, 'Y') = 'Y' ";
            sql = sql + " and g.hospital_code = d.hospital ";
            sql = sql + " and g.hosp_gp = '" + inst_code + "' ";
            sql = sql + " and original_don_date < '" + show_year + "0401' ";
            sql = sql + " and don_inc_exp in ('I', 'E') ";
            sql = sql + " and (" + financial_year + "<= 2003 or input_for < '" + show_year + "0401')";
            if (!string.IsNullOrEmpty(in_donor_name))
                sql = sql + " and donor_name like '" + in_donor_name + "%'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
                }
            }
            return result;
        }

        public decimal GetCostB(string inst_code, int show_year, int financial_year, string in_donor_name)
        {
            DataSet ds = new DataSet();
            decimal result = 0;
            var sql = "select isnull(sum(cost), 0) cost ";
            sql = sql + " from donation_history h, flash_rpt_hosp_gp g, donation_detail d left join don_cat c";
            sql = sql + " on d.don_cat = c.id ";
            sql = sql + " where h.id = d.id ";
            sql = sql + " and c.designated = 'N' ";
            sql = sql + " and g.hospital_code = d.hospital ";
            sql = sql + " and g.hosp_gp = '" + inst_code + "' ";
            sql = sql + " and original_don_date < '" + show_year + "0401' ";
            sql = sql + " and don_inc_exp in ('I', 'E') ";
            sql = sql + " and (" + financial_year + "<= 2003 or input_for < '" + show_year + "0401')";
            if (!string.IsNullOrEmpty(in_donor_name))
                sql = sql + " and donor_name like '" + in_donor_name + "%'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
                }
            }
            return result;
        }

        public DataSet GetDoantionByYear(string inst_code, string in_donor_name)
        {
            DataSet ds = new DataSet();
            var sql = "select case when datepart(mm, original_don_date) <=3 then convert(char(4),datepart(yy, original_don_date)-1) ";
            sql = sql + " else  convert(char(4),datepart(yy, original_don_date)) ";
            sql = sql + " end as temp_don_date, ";
            sql = sql + " isnull(sum(cost), 0) cost ";
            sql = sql + " from donation_history h, donation_detail d, flash_rpt_hosp_gp g ";
            sql = sql + " where h.id = d.id ";
            sql = sql + " and g.hospital_code = d.hospital ";
            sql = sql + " and g.hosp_gp = '" + inst_code + "' ";
            sql = sql + " and don_inc_exp in ('I', 'E') ";
            sql = sql + " and input_for < '20040401'";
            sql = sql + " and ((datepart(mm, original_don_date) <=3 and datepart(yy, original_don_date)-1 <= 2004) ";
            sql = sql + "	or (datepart(mm, original_don_date) >3 and datepart(yy, original_don_date) < 2004)) ";
            if (!string.IsNullOrEmpty(in_donor_name))
                sql = sql + " and upper(donor_name) like upper('" + in_donor_name + "%')";
            sql = sql + " group by case when datepart(mm, original_don_date) <=3 then convert(char(4),datepart(yy, original_don_date)-1) ";
            sql = sql + " else  convert(char(4),datepart(yy, original_don_date)) ";
            sql = sql + " end ";
            sql = sql + " union ";
            sql = sql + " select case when datepart(mm, input_for) <=3 then convert(char(4),datepart(yy, input_for)-1) ";
            sql = sql + " else  convert(char(4),datepart(yy, input_for)) ";
            sql = sql + " end as temp_don_date, ";
            sql = sql + " isnull(sum(cost), 0) cost ";
            sql = sql + " from donation_history h, donation_detail d, flash_rpt_hosp_gp g ";
            sql = sql + " where h.id = d.id ";
            sql = sql + " and g.hospital_code = d.hospital ";
            sql = sql + " and g.hosp_gp = '" + inst_code + "' ";
            sql = sql + " and don_inc_exp in ('I', 'E') ";
            sql = sql + " and input_for >= '20040401'";
            if (!string.IsNullOrEmpty(in_donor_name))
                sql = sql + " and upper(donor_name) like upper('" + in_donor_name + "%')";
            sql = sql + " group by case when datepart(mm, input_for) <=3 then convert(char(4),datepart(yy, input_for)-1) ";
            sql = sql + " else  convert(char(4),datepart(yy, input_for)) ";
            sql = sql + " end ";
            sql = sql + " order by temp_don_date";

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

        public DataSet GetDonationHistoryByIdFinYr(int id, int financial_year)
        {
            DataSet ds = new DataSet();
            var sql = "select convert(char(8), input_for, 112) input_for , ";
            sql = sql + " convert(char(8), original_don_date, 112) original_don_date, ";
            sql = sql + " cost, out_comm, datediff(m, '" + financial_year + "0401', input_for) mth_cnt";
            sql = sql + " from donation_history ";
            sql = sql + " where id = " + id;
            sql = sql + " and (rec_type = '' or rec_type is null)";
            sql = sql + " and original_don_date < dateadd(yy, 1, '" + financial_year + "0401')";
            sql = sql + " and input_for < dateadd(yy, 1, '" + financial_year + "0401') ";
            sql = sql + " and (original_don_date >= '" + financial_year + "0401'";
            sql = sql + " or (input_for >= '" + financial_year + "0401'";
            sql = sql + " and " + financial_year + " >= 2003))";
            sql = sql + " order by original_don_date desc, input_for desc";

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

        public void DeleteDonation(string login_id, int id)
        {
            var sql = " insert into donation_deleted ";
            sql = sql + " (id, hospital, trust, fund, section, analytical, donor_type, donor_name, donor_id , don_inc_exp, don_type, ";
            sql = sql + " don_purpose, don_supercat, don_cat, don_subcat, don_subsubcat, don_specific, eqt_desc, recurrent_con, reimb, recurrent_cost, ";
            sql = sql + " don_kind_desc,maj_don1,maj_don2,maj_don3, input_by, input_at, ";
            sql = sql + " update_by, update_at) ";
            sql = sql + " select id, hospital, trust, fund, section, analytical, donor_type, donor_name, donor_id, don_inc_exp, don_type, ";
            sql = sql + " don_purpose, don_supercat, don_cat, don_subcat, don_subsubcat, don_specific, eqt_desc, recurrent_con, reimb, recurrent_cost, ";
            sql = sql + " don_kind_desc,maj_don1,maj_don2,maj_don3, input_by, input_at, ";
            sql = sql + " '" + login_id + "', getdate()";
            sql = sql + " from donation_detail where id = " + id;

            sql = sql + " insert into donation_history_deleted ";
            sql = sql + " (id, cost, out_comm, input_for, last_update_by, last_update_at, original_don_date) ";
            sql = sql + " select id, cost, out_comm, input_for, ";
            sql = sql + " '" + login_id + "', getdate(), original_don_date";
            sql = sql + " from donation_history where id = " + id;

            sql = sql + " exec [delete_donation_reserve] " + id;
            sql = sql + "delete donation_detail where id = " + id;
            sql = sql + "delete donation_history where id = " + id;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteDonationHistory(int id, int financial_year)
        {
            var sql = " delete donation_history ";
            sql = sql + " where id = " + id;
            sql = sql + " and (rec_type = '' or rec_type is null)";
            sql = sql + " and original_don_date < dateadd(yy, 1, '" + financial_year + "0401')";
            sql = sql + " and input_for < dateadd(yy, 1, '" + financial_year + "0401') ";
            sql = sql + " and ((original_don_date >= '" + financial_year + "0401'";
            sql = sql + " and " + financial_year + " < 2003) ";
            sql = sql + " or (input_for >= '" + financial_year + "0401'";
            sql = sql + " and " + financial_year + " >= 2003))";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertDonationHistory(int id, List<PreviousRecord> CM_record, List<PreviousRecord> Previous_record, string login_id, string don_type)
        {
            List<PreviousRecord> donationList = new List<PreviousRecord>();
            donationList.AddRange(CM_record);
            donationList.AddRange(Previous_record);
            var sql = "";
            foreach (var r in donationList)
            {
                if (r.Input_for.Trim().Length == 0)
                    r.Input_for = r.Original_don_date;
                if (r.Input_for.Trim().Length > 0)
                {
                    if (r.Cost != 0 || don_type == "1")
                    {
                        sql = sql + " insert into donation_history ";
                        sql = sql + " (id, cost, out_comm, input_for, last_update_by, last_update_at, rec_type, original_don_date) ";
                        sql = sql + " values ";
                        sql = sql + " (" + id + ", " + r.Cost + ", 0, convert(datetime, '" + r.Input_for + "', 3), '" + login_id + "', getdate(), '', convert(datetime, '" + r.Original_don_date + "', 3))";
                    }
                }
            }

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertDonationHistoryForNewDonation(List<PreviousRecord> CM_record, List<PreviousRecord> Previous_record, string login_id)
        {
            List<PreviousRecord> donationList = new List<PreviousRecord>();
            donationList.AddRange(CM_record);
            donationList.AddRange(Previous_record);
            var sql = "";
            foreach (var r in donationList)
            {
                if (r.Cost != 0 || r.Input_for.Trim().Length > 0)
                {
                    if (r.Input_for.Trim().Length > 0)
                        r.Input_for = r.Original_don_date;
                    sql = sql + " insert into donation_history ";
                    sql = sql + " (id, cost, input_for, last_update_by, last_update_at, rec_type, original_don_date) ";
                    sql = sql + " values ";
                    sql = sql + " (@@identity, " + r.Cost + ", convert(datetime, '" + r.Input_for + "', 3), '" + login_id + "', getdate(), '', convert(datetime, '" + r.Original_don_date + "', 3))";
                }
            }
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public string InsertDonationHistory(decimal don_cur_mth, int out_comm, DateTime don_date, string login_id)
        {
            string sql = "insert into donation_history ";
            sql = sql + " (id, cost, out_comm, input_for, last_update_by, last_update_at, rec_type, original_don_date) ";
            sql = sql + " values ";
            sql = sql + " (@@identity, " + don_cur_mth + ", " + out_comm + ", '" + don_date + "', '" + login_id + "', getdate(), '', '" + don_date + "')  ";

                return sql;
        }
    }
}
