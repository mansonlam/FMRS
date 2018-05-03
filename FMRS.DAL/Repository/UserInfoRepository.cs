using System;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using FMRS.Common.DataSource;

namespace FMRS.DAL.Repository
{
    public interface IUserInfoRepository : IBaseRepository<UserInfo>
    {
        void test();
        UserInfo FindUserInfoByLoginId(string loginId);
        string FindUserInstCodeByLoginId(string loginId);
        string check_users(string user_id, string password, string fmrs_system);
        DataSet check_sys_access(string user_id);
        IFMRSUserPrincipal GetValueDate(DateTime current_date);
        string GetDonationPeriod();
        IFMRSUserPrincipal GetDomainUserAdmin(string LoginId);
        IFMRSUserPrincipal GetParValue();
        IFMRSUserPrincipal GetPeriodEnd(string value_date);
        IFMRSUserPrincipal GetPeriod(string loginId, string inst_code);
        List<UserInfo> GetUserInfoByLoginIdHoUser(string loginId, string ho_user);
        DataSet GetUserList(string user_group, string fmrs_system, string loginId);
        String GetDomainUserByLoginId(string loginId);
        string GetDomainInstByLoginId(string loginId);
        EnquireUserViewModel GetEnquireUser(string modules, string loginId, string user_group);
        string GetUserLastLogin(string loginId);
        void DeleteUserByUserId(string loginId);
        void InsertUserByLoginId(string loginId, string userName, string hospital, string userGroup, string lastLogin);
    }
    public class UserInfoRepository : BaseRepository<UserInfo>, IUserInfoRepository
    {
        private FMRSContext Context;
        private DbSet<UserInfo> userInfo;
        private IFMRSUserPrincipal User;
        private IAccessRightRepository AccessRightRepository;
        private IAccessRightDRepository AccessRightDRepository;
        private IAccessRightMRepository AccessRightMRepository;
        private IAccessRightYRepository AccessRightYRepository;

        public UserInfoRepository(FMRSContext _context, IFMRSUserPrincipal _user, IAccessRightRepository _accessRightRepository,
                                IAccessRightDRepository _accessRightDRepository, IAccessRightMRepository _accessRightMRepository,
                            IAccessRightYRepository _accessRightYRepository) : base(_context)
        {
            Context = _context;
            userInfo = Context.Set<UserInfo>();
            User = _user;
            AccessRightRepository = _accessRightRepository;
            AccessRightDRepository = _accessRightDRepository;
            AccessRightMRepository = _accessRightMRepository;
            AccessRightYRepository = _accessRightYRepository;
        }

        public void test()
        {
            /* TEST syntax */
            var id = "corp/ckc004";
            IEnumerable<UserInfo> test = from a in userInfo where EF.Functions.Like(a.LoginId, "%004") select a;
            var result = Context.UserInfo.FromSql($@"Select * FROM UserInfo WHERE LoginId = {id}");
            //List<UserInfo> sql_test = Context.UserInfo.FromSql("EXECUTE dbo.sp_name @username, @password", username, password).ToList();
            //var sp_test = Context.Set<UserInfo>().FromSql("dbo.sp_name @username = {0}, @password = {1}", username, password);

            /* END TEST */
        }

        public UserInfo FindUserInfoByLoginId(string loginId)
        {
            UserInfo UserInfoList = new UserInfo();
            loginId = loginId.ToLower().Replace("\\", "/");
            UserInfoList = Context.UserInfo.Where(u => u.LoginId == loginId).ToList().FirstOrDefault();   
            return UserInfoList;
        }

        public string FindUserInstCodeByLoginId(string loginId)
        {
            loginId = loginId.ToLower().Replace("\\", "/");
            var query = ((from u in Context.UserInfo
                          where u.LoginId == loginId
                          select new { UserGroup = u.UserGroup.Trim(), InstCode = u.InstCode.Trim(), Main = 'N' })
            .Union
                (from s in Context.SuppUserGroup
                 where s.LoginId == loginId
                 select new { UserGroup = s.UserGroup.Trim(), InstCode = s.InstCode.Trim(), Main = 'Y' })).OrderBy(o => o.Main);

            return query.FirstOrDefault().InstCode;
        }

        public string check_users(string user_id, string password, string fmrs_system)
        {
            var pwd_expiry = "";
            using (var cmd = Context.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "dbo.p_chk_users";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.VarChar) { Value = user_id });
                cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar) { Value = password });
                cmd.Parameters.Add(new SqlParameter("@fmrs_system", SqlDbType.Char) { Value = fmrs_system });
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                var MyDataReader = cmd.ExecuteReader();
                if (MyDataReader.Read())
                {
                    pwd_expiry = MyDataReader.GetValue(0).ToString();
                }
                cmd.Connection.Close();
            }
            return pwd_expiry;
        }

        public DataSet check_sys_access(string user_id)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.system_access", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@login_id", user_id);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public IFMRSUserPrincipal GetValueDate(DateTime current_date)
        {
            DataSet ds = new DataSet();
            var sql = "select convert(char(8), input_for, 112) input_for, convert(char(8),dateadd(mm, -1, input_for), 112) input_for2,  ";
            sql = sql + " day(DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0, convert(char(8),dateadd(mm, -1, input_for), 112)) + 1, 0))) as gl_end_day,";
            sql = sql + "       convert(char(8), dateadd(dd, -1, dateadd(mm, 1, input_for)), 112) get_current_date ";
            sql = sql + " from donation_period ";
            sql = sql + " where start_date = (select max(start_date) ";
            sql = sql + "                     from donation_period ";
            sql = sql + "                     where start_date <= '" + current_date + "')";

            //var query = from d in Context.DonationPeriod
            //            where d.startDate = ((from dp in Context.DonationPeriod where dp.StartDate <= current_date select dp).Max(x => x.StartDate))
            //            select new
            //            {
            //                input_for = input_for.ToString("yyyyMMdd"),// convert(char(8), input_for, 112),
            //                input_for2 = (input_for.AddMonths(-1)).ToString("yyyyMMdd"),//convert(char(8), dateadd(mm, -1, input_for), 112),
            //                gl_end_day = day(DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0, (input_for.AddMonths(-1))) + 1, 0))),
            //                get_current_date = (input_for.AddMonths(1)).AddDays(-1).ToString("yyyyMMdd") //convert(char(8), dateadd(dd, -1, dateadd(mm, 1, input_for)), 112)
            //            };
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        User.Value_Date = ds.Tables[0].Rows[0][0].ToString();
                        User.Value_Date2 = ds.Tables[0].Rows[0][1].ToString();
                        User.Gl_End_Day = ds.Tables[0].Rows[0][2].ToString();
                        User.Current_Date = ds.Tables[0].Rows[0][3].ToString();
                        User.Current_Year = User.Current_Date.Substring(0, 4);
                        User.Current_Month = User.Current_Date.Substring(4, 2);
                        User.Current_Day = User.Current_Date.Substring(6, 2);
                    }
                }
            }

            return User;
        }
        public string GetDonationPeriod()
        {
            string result = "N";
            DataSet ds = new DataSet();
            var sql = "select dbo.chk_don_period()";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = ds.Tables[0].Rows[0][0].ToString();
                }
            }
            return result;
        }

        public IFMRSUserPrincipal GetDomainUserAdmin(string loginId)
        {
            loginId = loginId.Replace("/", "\\").ToUpper();
            DataSet ds = new DataSet();
            var sql = "select * from financial_closing.dbo.fv_user_profile where domain_id = '" + loginId + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        User.Fv_Domain_Id = ds.Tables[0].Rows[0]["domain_id"].ToString().Trim();
                        User.Fv_User_Admin = ds.Tables[0].Rows[0]["user_admin"].ToString().Trim();
                    }
                }
            }
            return User;
        }
        public IFMRSUserPrincipal GetParValue()
        {
            DataSet ds = new DataSet();
            var sql = "select par_type, par_value from system_parameter";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        { 
                            var par_type = dr["par_type"].ToString().Trim();
                            var par_value = dr["par_value"].ToString().Trim();
                            switch (par_type)
                            {
                                case "fin_year":
                                    User.Financial_Year = Convert.ToInt32(par_value);
                                    break;
                                case "p13":
                                    User.p13 = par_value == "1" ? "Y" : "N";
                                    break;
                                case "p13_actual":
                                    User.p13_Actual = par_value;
                                    break;
                                case "other_wk_agent":
                                    User.Other_Wk_Agent = par_value == "1" ? "Y" : "N";
                                    break;
                            }

                        }
                    }
                }
            }
            return User;
        }

        public IFMRSUserPrincipal GetPeriodEnd(string value_date)
        {
            DataSet ds = new DataSet();
            var sql = "select financial_year_ind, period_ended from financial_closing..input_period";
            sql = sql + " where input_period_from <= '" + value_date.Substring(4) + "' and input_period_to >= '" + value_date.Substring(4) + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        User.Financial_Year_Ind = ds.Tables[0].Rows[0]["financial_year_ind"].ToString().Trim();
                        User.Period_Ended = ds.Tables[0].Rows[0]["period_ended"].ToString().Trim();
                    }
                }
            }
            if (value_date.Substring(4) == "1201")
            {
                User.Financial_Year_Ind = "0";
                User.Period_Ended = "0930";
            }
            User.Period_End_Date = (Convert.ToInt32(value_date.Substring(0, 4)) + Convert.ToInt32(User.Financial_Year_Ind)) + "-" + User.Period_Ended.Substring(0, 2) + "-" + User.Period_Ended.Substring(2, 2);
            return User;
        }

        public IFMRSUserPrincipal GetPeriod(string loginId, string inst_code)
        {
            User.Input_Period = "N";
            User.Adj_Period = "N";
            User.Far_Period = "N";
            User.Schedule_Cbv_Comment_Period  = "N";
            User.Interim_Closing_Period = "N";
            User.Year_End_Closing_Period = "N";
            User.F52_Interim_Closing_Period = "N";
            User.F52_Year_End_Closing_Period = "N";
            User.Other_Gov_Agent_8100mx_Period = "N";
            User.Re_Budget_Period = "N";
            DataSet ds = new DataSet();
            var sql = "select rtrim(type) period_type from fmrs_period  ";
            sql = sql + "where from_date <= getdate() and to_date >= getdate() ";
            sql = sql + "  and ((target_type = 'a') ";
            sql = sql + "       or ((target_type = 'c') and target_detail in (select Cluster from hospital where hospital_code = '" + inst_code + "')) ";
            sql = sql + "       or ((target_type = 'h') and target_detail = '" + inst_code + "') ";
            sql = sql + "       or ((target_type = 'u') and target_detail = '" + loginId + "') ";
            sql = sql + "      ) ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            var period_type = dr[0].ToString();
                            switch (period_type)
                            {
                                case "I":
                                    User.Input_Period = "Y";
                                    break;
                                case "A":
                                    User.Adj_Period = "Y";
                                    break;
                                case "F":
                                    User.Far_Period = "Y";
                                    break;
                                case "C":
                                    User.Schedule_Cbv_Comment_Period = "Y";
                                    break;
                                case "IC":
                                    User.Interim_Closing_Period = "Y";
                                    break;
                                case "YC":
                                    User.Year_End_Closing_Period = "Y";
                                    break;
                                case "IF":
                                    User.F52_Interim_Closing_Period = "Y";
                                    break;
                                case "YF":
                                    User.F52_Year_End_Closing_Period = "Y";
                                    break;
                                case "GA":
                                    User.Other_Gov_Agent_8100mx_Period = "Y";
                                    break;
                                case "B":
                                    User.Re_Budget_Period = "Y";
                                    break;
                            }

                        }
                    }
                }
            }
            return User;
        }

        public List<UserInfo> GetUserInfoByLoginIdHoUser(string loginId, string ho_user)
        {
            return Context.UserInfo.Where(u => u.LoginId == loginId && u.HoUser == ho_user).ToList();
        }

        public DataSet GetUserList(string user_group, string fmrs_system, string loginId)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct u.login_id, user_name, password, u.user_group, pwd_expiry, inst_code, isnull(convert(char(12), last_login, 106) + convert(char(5), last_login, 108), '-') last_login";
            sql = sql + " from user_info u, user_group_hosp, access_right a, login_access l  ";
            sql = sql + " where u.inst_code = hospital_code ";
            sql = sql +" and user_group_hosp.user_group = '" + user_group + "'";
            sql = sql +" and l.access_type = a.access_type ";
            sql = sql +" and u.login_id = a.login_id ";
            sql = sql +" and privilege <> 'N' ";
            sql = sql +" and l.fmrs_system = '" + fmrs_system + "'";
            sql = sql +" and pwd_expiry = 'N' ";
            sql = sql +" union ";
            sql = sql +"select distinct u.login_id, user_name, password, u.user_group, pwd_expiry, inst_code, isnull(convert(char(12), last_login, 106) + convert(char(5), last_login, 108), '-') last_login";
            sql = sql +" from user_info u, user_group_hosp h, access_right a, login_access l   ";
            sql = sql +" where u.user_group = h.user_group ";
            sql = sql +" and l.access_type = a.access_type ";
            sql = sql +" and u.login_id = a.login_id ";
            sql = sql +" and privilege <> 'N' ";
            sql = sql +" and l.fmrs_system = '" + fmrs_system + "'";
            sql = sql +" and pwd_expiry = 'N' ";
            sql = sql +" and not exists (select * from user_group_hosp h2 ";
            sql = sql +" where h2.user_group = h.user_group ";
            sql = sql +" and h2.hospital_code not in (select hospital_code from user_group_hosp h3 ";
            sql = sql +" where h3.user_group = '" + user_group + "'))";
            sql = sql +" union ";
            sql = sql +" select distinct u.login_id, user_name, password, u.user_group, pwd_expiry, inst_code, ";
            sql = sql +" isnull(convert(char(12), last_login, 106) + convert(char(5), last_login, 108), '-') last_login ";
            sql = sql +" from user_info u, financial_closing.dbo.fv_user_profile p ";
            sql = sql +" where u.login_id = LOWER(replace(p.domain_id,'\','/')) ";
            sql = sql +" and p.cluster = (select cluster from financial_closing.dbo.fv_user_profile where domain_id = '" + loginId.Replace( "/", "\\").ToUpper() + "')" ;
            sql = sql +" order by u.user_group, u.login_id";
            
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

        public String GetDomainUserByLoginId(string loginId)
        {
            return Context.UserInfo.Where(u => u.LoginId == loginId).Select(u => u.DomainUser).FirstOrDefault();
        }

        public string GetDomainInstByLoginId(string loginId)
        {
            return Context.UserInfo.Where(u => u.LoginId == loginId).Select(u => u.DomainInst).FirstOrDefault();
        }

        public EnquireUserViewModel GetEnquireUser(string modules, string loginId, string user_group)
        {
            loginId = loginId.Replace("\\", "/").ToLower();
            DataSet ds = new DataSet();
            EnquireUserViewModel user = new EnquireUserViewModel();
            user.AdminUserInfo = new UserInfo();
            var sql = "select login_id, user_name, password, pwd_expiry, inst_code, ";
            sql = sql + " domain_user, isnull(convert(char(12), last_login, 106) + convert(char(5), last_login, 108), '-') last_login ";
            sql = sql + " from user_info where login_id = '" + loginId + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        user.AdminUserInfo.LoginId = ds.Tables[0].Rows[0][0].ToString(); 
                        user.AdminUserInfo.UserName = ds.Tables[0].Rows[0][1].ToString().Trim()??"";
                        user.AdminUserInfo.Password = ds.Tables[0].Rows[0][2].ToString().Trim()??"";
                        user.AdminUserInfo.PwdExpiry = ds.Tables[0].Rows[0][3].ToString().Trim()??"N";
                        user.AdminUserInfo.InstCode = ds.Tables[0].Rows[0][4].ToString().Trim()??"";
                        user.AdminUserInfo.DomainUser = ds.Tables[0].Rows[0][5].ToString().Trim()??"Y";
                        user.LastLogin_string = ds.Tables[0].Rows[0][6].ToString().Trim()??"";
                        AccessRightD rightD = AccessRightDRepository.GetAccessRightD(loginId);
                        AccessRightM rightM = AccessRightMRepository.GetAccessRightM(loginId);
                        AccessRightY rightY = AccessRightYRepository.GetAccessRightY(loginId);
                        switch (modules)
                        { 
                            case "D":
                                user.AdminUserInfo.UserGroup = rightD.UserGroup ?? user_group;
                                user.AdminRightD = rightD;
                                break;
                            case "M":
                                user.AdminUserInfo.UserGroup = rightM.UserGroup ?? user_group;
                                user.AdminRightM = rightM;
                                break;
                            case "Y":
                                user.AdminUserInfo.UserGroup = rightY.UserGroup ?? user_group;
                                user.AdminRightY = rightY; break;
                        }
                    }
                }
            }

            return user;
        }

        public string GetUserLastLogin(string loginId)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = "select convert(char(12), last_login, 106) + convert(char(5), last_login, 108) last_login ";
            sql = sql + " from user_info ";
            sql = sql + " where login_id = '" + loginId + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    result = ds.Tables[0].Rows[0]["last_login"].ToString();
                }
            }
            return result;
        }

        public void DeleteUserByUserId(string loginId)
        {
            var sql = " delete user_info where login_id = '" + loginId + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertUserByLoginId(string loginId, string userName, string hospital, string userGroup, string lastLogin)
        {
            var sql = " insert into user_info ";
            sql = sql + " (login_id, user_name, inst_code, pwd_expiry, user_group, last_login, ho_user, domain_user)";
            sql = sql + " values ('" + loginId + "', '" + userName + "', ";
            sql = sql + " '" + hospital + "', 'N', '" + userGroup + "', ";
            if (lastLogin == "")
                sql = sql + " NULL ";
            else
                sql = sql + "'" + lastLogin + "'";
            sql = sql + ", 'N', 'Y')";
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
