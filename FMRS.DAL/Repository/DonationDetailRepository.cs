using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace FMRS.DAL.Repository
{
    public interface IDonationDetailRepository : IBaseRepository<DonationDetail>
    {
        List<DonationDetail> GetAllDonationDetail();
        DonationDetail GetDonationDetailById(int id);
        List<string> GetDonDesc(string hospital, int financial_year);
        decimal GetCMInput(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp, int id);
        DataSet GetExistList(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int id);
        DataSet GetExistList2(string hospital, string fund_code, string analytical_code, string section_code, int financial_year, int curr_financial_year, DateTime current_date, int detail_id, string don_desc, string donor_name, short don_cat);
        int GetExistListCount(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int id);
        DataSet GetDonationList(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated);
        void DonationLinkRecord(int id, int oid);
        void UpdateDonationLinkID(int oid, int link_id);
        void UpdateDonationReserve(DateTime value_date, string inst_code, int id, string login_id);
        void UpdateDonationLinkDetail(int id);
        void UpdateDonationDetail(DonationRecNExpModel model, string login_id);
        void InsertDonationDetail(DonationRecNExpModel model, string login_id);
        decimal GetIncomeInput(string hospital, string fund_code, string section_code, string analytical_code, string value_date);
        decimal GetExpenditure(string hospital, string fund_code, string section_code, string analytical_code, string value_date);
        decimal GetOutstanding(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp);
        string InsertDonationDetail(string hospital, string fund_code, string section_code, string analytical_code, int trust, int donor_id, 
            string donor_name, string don_inc_exp, int don_type, int don_purpose, int don_super_cat, int don_cat, int don_subcat, int don_subsubcat,
            string don_specific, string maj_don_1, string maj_don_2, string maj_don_3, string reimb, string don_kind_desc, decimal don_cur_mth,
            string login_id);
        void ExecuteUploadSQL(string upload_strsql);
    }
    public class DonationDetailRepository : BaseRepository<DonationDetail>, IDonationDetailRepository
    {
        private FMRSContext Context;
        private DbSet<DonationDetail> donationDetail;
        public DonationDetailRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donationDetail = Context.Set<DonationDetail>();
        }

        public List<DonationDetail> GetAllDonationDetail()
        {
            return Context.DonationDetail.ToList();
        }
        public DonationDetail GetDonationDetailById(int id)
        {
            return Context.DonationDetail.Where(d => d.Id == id).SingleOrDefault();
        }
        //if remove sql condition for h.input for financial year +-1, drop down list for Description of Donation can be display for all record in all year.
        public List<string> GetDonDesc(string hospital, int financial_year)
        {
            DataSet ds = new DataSet();
            List<string> result = new List<string>();
            var sql = "select distinct don_kind_desc from donation_detail d inner join donation_history h on h.id = d.id ";
            sql = sql + "  where d.hospital = '" + hospital + "' ";
            sql = sql + " and h.input_for  >'" + (financial_year - 1) + "0401'";
            sql = sql + " and h.input_for  <'" + (financial_year + 1) + "0401' ";
            sql = sql + "order by don_kind_desc"; 

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                            result.Add(row["don_kind_desc"].ToString().Replace("\r", " ").Replace("\n", " "));//.Replace("'", "&#39;").Replace("\"\"", "\"\"\"\""));
                    }
                }
            }
            return result;
        }

        public decimal GetCMInput(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp, int id)
        {
            DataSet ds = new DataSet();
            decimal result = new decimal();
            var sql = "select d.hospital, d.fund, d.section, d.analytical, sum(h.cost) as 'cm_input'";
            sql = sql + "from donation_detail d inner join donation_history h on d.id = h.id ";
            sql = sql + "where d.hospital = '" + hospital + "' and d.fund = '" + fund_code + "' and d.section = '" + section_code;
            sql = sql + "' and d.analytical  =  '" + analytical_code + "' and month(h.input_for) = month('" + value_date + "') ";
            sql = sql + "and d.id <> " + id + " and d.don_inc_exp = '" + don_inc_exp + "'";
            sql = sql + " and year(h.input_for) = year('" + value_date + "') group by d.hospital, d.fund, d.section, d.analytical";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var tempResult = ds.Tables[0].Rows[0]["cm_input"].ToString();
                        if (tempResult == "")
                            result = 0;
                        else
                            result = Decimal.Parse(tempResult);
                    }
                }
            }
            return result;
        }

        public DataSet GetExistList(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.donation_get_exist_list", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@hosp_gp", inst_code);
                    sqlCmd.Parameters.AddWithValue("@fund", fund_code);
                    sqlCmd.Parameters.AddWithValue("@analytical", analytical_code);
                    sqlCmd.Parameters.AddWithValue("@section", section_code);
                    sqlCmd.Parameters.AddWithValue("@financial_year", financial_year);
                    sqlCmd.Parameters.AddWithValue("@curr_financial_year", financial_year2);
                    sqlCmd.Parameters.AddWithValue("@current_date", value_date);
                    sqlCmd.Parameters.AddWithValue("@oid", id);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public DataSet GetExistList2(string hospital, string fund_code, string analytical_code, string section_code, int financial_year, int curr_financial_year, DateTime current_date, 
            int detail_id, string don_desc, string donor_name, short don_cat)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.donation_get_exist_list_2", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@hosp_gp", hospital);
                    sqlCmd.Parameters.AddWithValue("@fund", fund_code);
                    sqlCmd.Parameters.AddWithValue("@analytical", analytical_code);
                    sqlCmd.Parameters.AddWithValue("@section", section_code);
                    sqlCmd.Parameters.AddWithValue("@financial_year", financial_year);
                    sqlCmd.Parameters.AddWithValue("@curr_financial_year", curr_financial_year);
                    sqlCmd.Parameters.AddWithValue("@current_date", current_date);
                    sqlCmd.Parameters.AddWithValue("@oid", detail_id);
                    sqlCmd.Parameters.AddWithValue("@don_desc", don_desc.Replace("\\",""));
                    sqlCmd.Parameters.AddWithValue("@donor_name", donor_name);
                    sqlCmd.Parameters.AddWithValue("@don_cat", don_cat);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }
        public int GetExistListCount(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int id)
        {
            return GetExistList(inst_code, fund_code, analytical_code, section_code, financial_year, financial_year2, value_date, id).Tables[0].Rows.Count;
        }

        public DataSet GetDonationList(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.donation_get_list", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@hosp_gp", inst_code);
                    sqlCmd.Parameters.AddWithValue("@financial_year", show_year);
                    sqlCmd.Parameters.AddWithValue("@curr_financial_year", financial_year);
                    sqlCmd.Parameters.AddWithValue("@input_for", value_date);
                    sqlCmd.Parameters.AddWithValue("@cur_year_only", cur_year_only);
                    sqlCmd.Parameters.AddWithValue("@donor_name", in_donor_name);
                    sqlCmd.Parameters.AddWithValue("@inc_exp", inc_exp);
                    sqlCmd.Parameters.AddWithValue("@designated", designated);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public void DonationLinkRecord(int id, int oid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("donation_link_record", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.Parameters.AddWithValue("@oid", oid);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
        }

        public void UpdateDonationLinkID(int oid, int link_id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("update_donation_link_id", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@oid", oid);
                    sqlCmd.Parameters.AddWithValue("@link_id", link_id);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
        }

        public void UpdateDonationReserve(DateTime value_date, string inst_code, int id, string login_id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("update_donation_reserve", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@input_for", value_date);
                    sqlCmd.Parameters.AddWithValue("@hospital", inst_code);
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlCmd.Parameters.AddWithValue("@user_id", login_id);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
        }

        public void UpdateDonationLinkDetail(int id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("update_donation_link_detail", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@id", id);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
        }

        public void UpdateDonationDetail(DonationRecNExpModel model, string login_id)
        {
            var sql = "update donation_detail ";
            sql = sql + " set hospital = '" + model.Hospital + "',";
            sql = sql + "     trust = " + model.Trust + ", ";
            sql = sql + "     donor_type = '" + model.Donor_type + "', ";
            sql = sql + "     donor_name = N'" + model.Donor_name + "', ";
            sql = sql + "     donor_id = '" + model.Donor_name_exist + "', ";
            sql = sql + "     don_inc_exp = '" + model.Don_inc_exp + "', ";
            sql = sql + "     don_purpose = " + model.Don_purpose + ", ";
            sql = sql + "     don_supercat = " + model.Don_supercat + ", ";
            sql = sql + "     don_cat = " + model.Don_cat + ", ";
            sql = sql + "     don_subcat = " + model.Don_subcat + ", ";
            sql = sql + "     don_subsubcat = " + model.Don_subsubcat + ", ";
            sql = sql + "     don_specific = N'" + model.Don_specific + "', ";
            sql = sql + "     eqt_desc = '" + model.Eqt_desc + "', ";
            sql = sql + "     maj_don1 = '" + model.Maj_don1 + "', ";
            sql = sql + "     maj_don2 = '" + model.Maj_don2 + "', ";
            sql = sql + "     maj_don3 = '" + model.Maj_don3 + "', ";
            sql = sql + "     recurrent_con = '" + model.Recurrent_con + "', ";
            sql = sql + "     reimb = '" + model.Reimb + "', ";
            sql = sql + "     recurrent_cost = " + model.Recurrent_cost + ", ";
            sql = sql + "     don_kind_desc = N'" + model.Don_kind_desc + "', ";
            sql = sql + "     update_by = '" + login_id + "', ";
            sql = sql + "     update_at = getdate()";
            sql = sql + " where id = " + model.Id;

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertDonationDetail(DonationRecNExpModel model, string login_id)
        {
            DataSet ds = new DataSet();
            var sql = "insert into donation_detail ";
            sql = sql + " (hospital, trust, fund, section, analytical, donor_type, donor_name, donor_id, don_inc_exp, don_type, ";
            sql = sql + " don_purpose, don_supercat, don_cat, don_subcat, don_subsubcat, don_specific, maj_don1,maj_don2,maj_don3 , eqt_desc, recurrent_con, reimb, recurrent_cost, ";
            sql = sql + " don_kind_desc, input_by, input_at, ";
            sql = sql + " update_by, update_at) ";
            sql = sql + " values ";
            sql = sql + " ('" + model.Inst_code + "', " + model.Trust + ",'" + model.Fund + "', '" + model.Section + "','" + model.Analytical + "', '" + model.Donor_type + "', N'" ;
            sql = sql + model.Donor_name + "', '" + model.Donor_name_exist + "', '" + model.Don_inc_exp + "', " + model.Don_type + ", ";
            sql = sql + model.Don_purpose + ", " + model.Don_supercat + ", " + model.Don_cat + ", " + model.Don_subcat + ", " + model.Don_subsubcat + ", N'";
            sql = sql + model.Don_specific + "','" + model.Maj_don1 + "','" + model.Maj_don2 + "','" + model.Maj_don3 + "', '" + model.Eqt_desc + "', '";
            sql = sql + model.Recurrent_con + "', '" + model.Reimb + "', " + model.Recurrent_cost + ", N'" + model.Don_kind_desc + "', '";
            sql = sql + login_id + "', getdate(), '" + login_id + "', getdate()) ";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public decimal GetIncomeInput(string hospital, string fund_code, string section_code, string analytical_code, string value_date)
        {
            DataSet ds = new DataSet();
            decimal result = new decimal();
            var sql = "select sum(h.cost) income ";
            sql = sql + "from donation_detail d, donation_history h ";
            sql = sql + "where d.id = h.id ";
            sql = sql + "and d.hospital = '" + hospital + "' ";
            sql = sql + "and d.fund = '" + fund_code + "' ";
            sql = sql + "and d.section = '" + section_code + "' ";
            sql = sql + "and d.analytical = '" + analytical_code + "' ";
            sql = sql + "and d.don_inc_exp = 'I' ";
            sql = sql + "and h.input_for between '" + value_date + "' and dateadd(d, -1, dateadd(mm, 1, '" + value_date + "'))";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var tempResult = ds.Tables[0].Rows[0]["income"].ToString();
                        if (tempResult == "")
                            result = 0;
                        else
                            result = Decimal.Parse(tempResult);
                    }
                }
            }
            return result;
        }

        public decimal GetExpenditure(string hospital, string fund_code, string section_code, string analytical_code, string value_date)
        {
            DataSet ds = new DataSet();
            decimal result = new decimal();
            var sql = "select sum(h.cost) expenditure ";
            sql = sql + "from donation_detail d, donation_history h ";
            sql = sql + "where d.id = h.id ";
            sql = sql + "and d.hospital = '" + hospital + "' ";
            sql = sql + "and d.fund = '" + fund_code + "' ";
            sql = sql + "and d.section = '" + section_code + "' ";
            sql = sql + "and d.analytical = '" + analytical_code + "' ";
            sql = sql + "and d.don_inc_exp = 'E' ";
            sql = sql + "and h.input_for between '" + value_date + "' and dateadd(d, -1, dateadd(mm, 1, '" + value_date + "'))";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var tempResult = ds.Tables[0].Rows[0]["expenditure"].ToString();
                        if (tempResult == "")
                            result = 0;
                        else
                            result = Decimal.Parse(tempResult);
                    }
                }
            }
            return result;
        }

        public decimal GetOutstanding(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp)
        {
            DataSet ds = new DataSet();
            decimal result = new decimal();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.donation_get_cm_amt", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@hosp_gp", hospital);
                    sqlCmd.Parameters.AddWithValue("@fund", fund_code);
                    sqlCmd.Parameters.AddWithValue("@section", section_code);
                    sqlCmd.Parameters.AddWithValue("@analytical", analytical_code);
                    sqlCmd.Parameters.AddWithValue("@input_for", value_date);
                    sqlCmd.Parameters.AddWithValue("@don_inc_exp", don_inc_exp);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            var tempResult = ds.Tables[0].Rows[0]["outstanding"].ToString();
                            if (tempResult == "")
                                result = 0;
                            else
                                result = Decimal.Parse(tempResult);
                        }
                    }
                }
            }
            return result;
        }

        public string InsertDonationDetail(string hospital, string fund_code, string section_code, string analytical_code, int trust, int donor_id,
            string donor_name, string don_inc_exp, int don_type, int don_purpose, int don_super_cat, int don_cat, int don_subcat, int don_subsubcat,
            string don_specific, string maj_don_1, string maj_don_2, string maj_don_3, string reimb, string don_kind_desc, decimal don_cur_mth,
            string login_id)
        {
            if (donor_id == 0)
                donor_name = "";
            string sql = "insert into donation_detail ";
            sql = sql + " (hospital, fund, section, analytical, trust, donor_id, donor_name, don_inc_exp, don_type, ";
            sql = sql + " don_purpose, don_supercat, don_cat, don_subcat, don_subsubcat, don_specific, maj_don1, maj_don2, maj_don3, reimb, don_kind_desc, ";
            sql = sql + " don_cur_mth, input_by, input_at, ";
            sql = sql + " update_by, update_at) ";
            sql = sql + " values ";
            sql = sql + " ('" + hospital + "', '" + fund_code + "', '" + section_code + "', '" + analytical_code + "', '" + trust + "', " + donor_id + ", N'', '" + don_inc_exp + "', '";
            sql = sql + don_type + "', " + don_purpose + ", " + don_super_cat + ", " + don_cat + ", " + don_subcat + ", " + don_subsubcat + ", N'" + don_specific + "', '";
            sql = sql + maj_don_1 + "', '" + maj_don_2 + "', '" + maj_don_3 + "', '" + reimb + "', N'" + don_kind_desc + "', " + don_cur_mth + ", '" + login_id + "', getdate(), '" + login_id + "', getdate())  ";
            return sql;
        }

        public void ExecuteUploadSQL(string upload_strsql)
        {
            var sql = upload_strsql;
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
