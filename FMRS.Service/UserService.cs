using System;
using FMRS;
using FMRS.Common.DataSource;
using FMRS.DAL.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Globalization;
using FMRS.Model.DTO;

namespace FMRS.Service
{
    /// <summary>
    /// The service layer for user related logic
    /// </summary>
    public interface IUserService
    {
        bool LoginUser(string username, string password, out List<string> system_access, string domain = "CORP");
        bool DomainLogin(string username, string password, string domain);
        bool GetUserByUsernamePassword(string username, string password, string domain);
        List<Claim> CreateClaims(string username, List<string> system_access, string domain = "CORP");
        string GetHoAccess(string user_group, string loginId);
        string GetClusterLevel(string user_group);
        List<UserAdminModel> GetAdminUserList(string user_group, string fmrs_system, string login_id);
        string GetDomainUserByLoginId(string loginId);
        string GetDomainInstByLoginId(string loginId);
        string GetUserGpHospByGp(string user_group);
        string GetUserLastLogin(string loginId);
        void DeleteUserByUserId(string loginId);
        void InsertUserInfo(string loginId, string userName, string hospital, string userGroup, string lastLogin);
        string GetHospitalCode(string user_group, string user_inst_code);
    }

    public class UserService : IUserService
    {
        private IFMRSUserPrincipal UserPrincipal;
        private IUserInfoRepository UserInfoRepository;
        private IAccessRightRepository AccessRightRepository;
        private IAccessRightDRepository AccessRightDRepository;
        private IAccessRightMRepository AccessRightMRepository;
        private IAccessRightYRepository AccessRightYRepository;
        private IHospitalRepository HospitalRepository;
        private IUserGroupRespository UserGroupRespository;
        private IUserGroupHospRespository UserGroupHospRespository;

        public UserService(IFMRSUserPrincipal _userPrincipal, 
                            IUserInfoRepository _userInfoRepository,
                            IAccessRightRepository _accessRightRepository,
                            IAccessRightDRepository _accessRightDRepository,
                            IAccessRightMRepository _accessRightMRepository,
                            IAccessRightYRepository _accessRightYRepository,
                            IHospitalRepository _hospitalRepository,
                            IUserGroupRespository _userGroupRespository,
                            IUserGroupHospRespository _userGroupHospRespository
            )
        {
            UserPrincipal = _userPrincipal;
            UserInfoRepository = _userInfoRepository;
            AccessRightRepository = _accessRightRepository;
            AccessRightDRepository = _accessRightDRepository;
            AccessRightMRepository = _accessRightMRepository;
            AccessRightYRepository = _accessRightYRepository;
            HospitalRepository = _hospitalRepository;
            UserGroupRespository = _userGroupRespository;
            UserGroupHospRespository = _userGroupHospRespository;
        }
        /// <summary>
        /// Get user by username and password
        /// </summary>
        public bool LoginUser(string username, string password, out List<string> system_access, string domain)
        {
            FMRSUserPrincipal UserPrincipal = new FMRSUserPrincipal();
            var LoginId = domain + "\\" + username;
            system_access = new List<string>();
            try
            {
                // Check CID login
                var user = DomainLogin(username, password, domain);
                if (!user) { return false;}

                // Check FMRS Login
                var FMRSUser = GetUserByUsernamePassword(username, password, domain);
                if (!FMRSUser) { return false; }

                /* Check System access
                * Check which system user can view*/
                var ds = UserInfoRepository.check_sys_access(LoginId.ToLower());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var system_code = row[0].ToString();
                        switch (system_code)
                        {
                            case "D":
                                system_access.Add("FMRS_SYS_D");
                                break;
                            case "M":
                                system_access.Add("FMRS_SYS_M");
                                break;
                            case "Y":
                                system_access.Add("FMRS_SYS_Y");
                                break;
                        }
                    }
                }
                else
                { return false; }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;

        }

        /// <summary>
        /// Get user from CID Login by username and password
        /// </summary>
        public bool DomainLogin(string username, string password, string domain)
        {
            return true;
        }

        /// <summary>
        /// Get user from FMRS Login by username and password
        /// </summary>
        public bool GetUserByUsernamePassword(string username, string password, string domain)
        {
            // Check user info via FRMS
            var fmrs_check_user = UserInfoRepository.check_users(domain.ToLower() + "\\" + username.ToLower(), password, "Y");
            if (!String.IsNullOrEmpty(fmrs_check_user) && (fmrs_check_user == "Y" || fmrs_check_user == "N"))
            {
                //"p_chk_users" record exist
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Create claims and Assign User info
        /// </summary>
        public List<Claim> CreateClaims(string username, List<string> system_access , string domain)
        {
            var User_Info = UserInfoRepository.FindUserInfoByLoginId(domain + "\\" + username);
            var DomainUserAdmin = UserInfoRepository.GetDomainUserAdmin(domain + "\\" + username);
            var Par_Value = UserInfoRepository.GetParValue();
            var inst_code = UserInfoRepository.FindUserInstCodeByLoginId(domain + "\\" + username);
            var claims = new List<Claim>
                        {
                            new Claim("UserID", username), // CKC004
                            new Claim("UserName", User_Info.UserName),// Chan Tai Man
                            new Claim("LoginId", domain+"\\"+username),// CORP\CKC004
                            new Claim("Domain", domain),
                            new Claim("InstCode", inst_code), 
                            new Claim("DonationPeriod", "Y"),//UserInfoRepository.GetDonationPeriod()),
                            new Claim("Fv_Domain_Id", DomainUserAdmin.Fv_Domain_Id),
                            new Claim("Fv_User_Admin", DomainUserAdmin.Fv_User_Admin),
                            new Claim("Financial_year", "2017"),//Par_Value.Financial_Year.ToString()),
                            new Claim("p13", Par_Value.p13),
                            new Claim("p13_Actual", Par_Value.p13_Actual),
                            new Claim("Other_Wk_Agent", Par_Value.Other_Wk_Agent),
                            //new Claim("Domain", domain),
                            //new Claim("Domain", domain)
                        };
            foreach (var sys in system_access)
            { claims.Add(new Claim(sys, "Y")); }

            string firstDateOfCurrentMonth = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).ToString("yyyyMMdd", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            DateTime Current_Date = DateTime.Now;
            string Current_Year = Current_Date.Year.ToString();
            string Current_Month = Current_Date.Month.ToString("00");
            string Current_Day = Current_Date.Day.ToString();
            string Current_Date_string = Current_Date.ToString("yyyyMMdd", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            string Value_Date = firstDateOfCurrentMonth;
            
            if (system_access.Contains("FMRS_SYS_M"))
            {
                Value_Date = firstDateOfCurrentMonth;
            }
            if (system_access.Contains("FMRS_SYS_Y"))
            {
                Value_Date = firstDateOfCurrentMonth;
                var period_end = UserInfoRepository.GetPeriodEnd(Value_Date);
                claims.Add(new Claim("Financial_Year_Ind", period_end.Financial_Year_Ind));
                claims.Add(new Claim("Period_Ended", period_end.Period_Ended));
                claims.Add(new Claim("Period_End_Date", period_end.Period_End_Date)); //yyyy-MM-dd
            }
            if (system_access.Contains("FMRS_SYS_D"))
            {
                claims.Add(new Claim("Cal_date", Current_Date_string));
                var Donation_Value_Date = UserInfoRepository.GetValueDate(Current_Date);
                Value_Date = Donation_Value_Date.Value_Date;
                string Value_Date2 = Donation_Value_Date.Value_Date2;
                string Gl_End_Day = Donation_Value_Date.Gl_End_Day;
                Current_Date_string = Donation_Value_Date.Current_Date;
                Current_Year = Donation_Value_Date.Current_Year;
                Current_Month = Donation_Value_Date.Current_Month;
                Current_Day = Donation_Value_Date.Current_Day;
                claims.Add(new Claim("Value_Date2", "20171201"));// Value_Date2));
                claims.Add(new Claim("Gl_End_Day", Gl_End_Day));
            }

            claims.Add(new Claim("Value_Date", "20180101"));// Value_Date)); //yyyyMMdd
            claims.Add(new Claim("Current_Date", Current_Date_string));
            claims.Add(new Claim("Current_Year", Current_Year));
            claims.Add(new Claim("Current_Month", Current_Month));
            claims.Add(new Claim("Current_Day", Current_Day));
            claims.Add(new Claim("Report_Date", Value_Date));
            claims.Add(new Claim("Report_Period", "2"));

            var temp_financial_year = Par_Value.Financial_Year;
            var Financial_Year = temp_financial_year;
            if (system_access.Contains("FMRS_SYS_M"))
            {
                Financial_Year = temp_financial_year;
            }
            if (system_access.Contains("FMRS_SYS_D"))
            {
                Financial_Year = temp_financial_year;
            }
            if (system_access.Contains("FMRS_SYS_Y") && Current_Date.Year == Financial_Year && Current_Date.Month < 8)
            {
                Financial_Year = temp_financial_year + 1;
            }
            claims.Add(new Claim("Financial_Year", Financial_Year.ToString()));

            var year_end = new DateTime(Financial_Year, 3, 1);
            claims.Add(new Claim("Year_End", year_end.ToString("yyyyMMdd", CultureInfo.GetCultureInfo("en-US"))));

            var from_date = Financial_Year.ToString() + "-04-01";
            var to_date = Value_Date.Substring(0, 4) + "-" + Value_Date.Substring(4, 2) + "-" + Value_Date.Substring(6, 2);
            DateTime fromDate = DateTime.ParseExact(from_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(to_date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            claims.Add(new Claim("Actual_Cnt", (((toDate.Year - fromDate.Year) * 12) + toDate.Month - fromDate.Month).ToString()));

            //User Privilege Financial Closing
            var User_PrivilegeY = AccessRightYRepository.GetFinClosingUserPrivilegeByLoginId(domain + "/" + username);
            claims.Add(new Claim("UserGroup_Y", User_PrivilegeY.UserGroup_Y ?? "N"));
            claims.Add(new Claim("Privilege_Admin_Y", User_PrivilegeY.Privilege_Admin_Y ?? "N"));
            claims.Add(new Claim("Privilege_Asoi_Input", User_PrivilegeY.Privilege_Asoi_Input ?? "N"));
            claims.Add(new Claim("Privilege_Asoi_Rpt_Y", User_PrivilegeY.Privilege_Asoi_Rpt_Y ?? "N"));
            claims.Add(new Claim("Privilege_Closing_Report_Y", User_PrivilegeY.Privilege_Closing_Report_Y ?? "N"));
            claims.Add(new Claim("Privilege_Report_Y", User_PrivilegeY.Privilege_Report_Y ?? "N"));
            claims.Add(new Claim("Privilege_Non_Pjt_Report_Y", User_PrivilegeY.Privilege_Non_Pjt_Report_Y ?? "N"));
            claims.Add(new Claim("Ho_Access_Y", GetHoAccess(User_PrivilegeY.UserGroup_Y, domain + "/" + username)));
            //User Privilege Project Management
            var User_PrivilegeM = AccessRightMRepository.GetProjMgtUserPrivilegeByLoginId(domain + "/" + username);
            claims.Add(new Claim("UserGroup_M", User_PrivilegeM.UserGroup_M ?? "N"));
            claims.Add(new Claim("Privilege_Admin_M", User_PrivilegeM.Privilege_Admin_M ?? "N"));
            claims.Add(new Claim("Privilege_Asoi_Rpt_M", User_PrivilegeM.Privilege_Asoi_Rpt_M ?? "N"));
            claims.Add(new Claim("Privilege_Closing_Report_M", User_PrivilegeM.Privilege_Closing_Report_M ?? "N"));
            claims.Add(new Claim("Privilege_Cbv_Report", User_PrivilegeM.Privilege_Cbv_Report ?? "N"));
            claims.Add(new Claim("Privilege_Cbv_Funding", User_PrivilegeM.Privilege_Cbv_Funding ?? "N"));
            claims.Add(new Claim("Privilege_Cluster_Admin", User_PrivilegeM.Privilege_Cluster_Admin ?? "N"));
            claims.Add(new Claim("Privilege_Cwrf", User_PrivilegeM.Privilege_Cwrf ?? "N"));
            claims.Add(new Claim("Privilege_Cwrf_Funding", User_PrivilegeM.Privilege_Cwrf_Funding ?? "N"));
            claims.Add(new Claim("Privilege_Cwrf_Submenu", User_PrivilegeM.Privilege_Cwrf_Submenu ?? "N"));
            claims.Add(new Claim("Privilege_Report_M", User_PrivilegeM.Privilege_Report_M ?? "N"));
            claims.Add(new Claim("Privilege_Non_Pjt_Report_M", User_PrivilegeM.Privilege_Non_Pjt_Report_M ?? "N"));
            claims.Add(new Claim("Ho_Access_M", GetHoAccess(User_PrivilegeM.UserGroup_M, domain + "/" + username)));
            //User Privilege Donation
            var User_PrivilegeD = AccessRightDRepository.GetDonationUserPrivilegeByLoginId(domain + "/" + username);
            claims.Add(new Claim("UserGroup_D", User_PrivilegeD.UserGroup_D ?? "N"));
            claims.Add(new Claim("Privilege_Admin_D", User_PrivilegeD.Privilege_Admin_D ?? "N"));
            claims.Add(new Claim("Privilege_Asoi_Rpt_D", User_PrivilegeD.Privilege_Asoi_Rpt_D ?? "N"));
            claims.Add(new Claim("Privilege_Closing_Report_D", User_PrivilegeD.Privilege_Closing_Report_D ?? "N"));
            claims.Add(new Claim("Privilege_Donation", User_PrivilegeD.Privilege_Donation ?? "N"));
            claims.Add(new Claim("Privilege_Report_D", User_PrivilegeD.Privilege_Report_D ?? "N"));
            claims.Add(new Claim("Privilege_Non_Pjt_Report_D", User_PrivilegeD.Privilege_Non_Pjt_Report_D ?? "N"));
            claims.Add(new Claim("Ho_Access_D", GetHoAccess(User_PrivilegeD.UserGroup_D, domain + "/" + username)));

            //Unknow if useful
            var User_Privilege = AccessRightRepository.GetUserPrivilegeByLoginId(domain + "\\" + username, "Y"); // fmrs_system??
            claims.Add(new Claim("Privilege_Pe_Adjust", User_Privilege.Privilege_Pe_Adjust ?? "N"));
            claims.Add(new Claim("Privilege_Far_Access", User_Privilege.Privilege_Far_Access ?? "N"));
            claims.Add(new Claim("Privilege_Re_Budget", User_Privilege.Privilege_Re_Budget ?? "N"));

            claims.Add(new Claim("HospCode_Y", GetHospitalCode(User_PrivilegeY.UserGroup_Y, inst_code)));
            claims.Add(new Claim("HospCode_M", GetHospitalCode(User_PrivilegeM.UserGroup_M, inst_code)));
            claims.Add(new Claim("HospCode_D", GetHospitalCode(User_PrivilegeD.UserGroup_D, inst_code)));

            //claims.Add(new Claim("Privilege_Income_Proj", User_Privilege.Privilege_Income_Proj ?? "N"));
            //claims.Add(new Claim("Privilege_Annual_Costing", User_Privilege.Privilege_Annual_Costing ?? "N"));
            //claims.Add(new Claim("Privilege_Ce_Report", User_Privilege.Privilege_Ce_Report ?? "N"));
            //claims.Add(new Claim("Privilege_Cash_Position", User_Privilege.Privilege_Cash_Position ?? "N"));
            //claims.Add(new Claim("Privilege_Proj", User_Privilege.Privilege_Proj ?? "N"));
            //claims.Add(new Claim("Privilege_Cpr_Report", User_Privilege.Privilege_Cpr_Report ?? "N"));
            //claims.Add(new Claim("Privilege_Deposit", User_Privilege.Privilege_Deposit ?? "N"));
            //claims.Add(new Claim("Privilege_Fx_Rate", User_Privilege.Privilege_Fx_Rate ?? "N"));
            //claims.Add(new Claim("Privilege_Cwrf_Hpd", User_Privilege.Privilege_Cwrf_Hpd ?? "N"));
            //claims.Add(new Claim("Privilege_Cwrf_Cwd", User_Privilege.Privilege_Cwrf_Cwd ?? "N"));
            //claims.Add(new Claim("Privilege_Cwrf_Ho", User_Privilege.Privilege_Cwrf_Ho ?? "N"));
            //claims.Add(new Claim("Privilege_Subvention", User_Privilege.Privilege_Subvention ?? "N"));
            //claims.Add(new Claim("Privilege_Pending_Ce_Approval", User_Privilege.Privilege_Pending_Ce_Approval ?? "N"));
            //claims.Add(new Claim("Don_Hosp", User_Privilege.Don_Hosp ?? "N"));
            //claims.Add(new Claim("Don_Haho", User_Privilege.Don_Haho ?? "N"));
            //claims.Add(new Claim("Don_Bss", User_Privilege.Don_Bss ?? "N"));
            //claims.Add(new Claim("Ho_Access", GetHoAccess(User_Info.UserGroup, domain + "/" + username)));
            claims.Add(new Claim("Donation_Period", UserInfoRepository.GetDonationPeriod()));
            var period = UserInfoRepository.GetPeriod(domain + "/" + username, inst_code);
            claims.Add(new Claim("Input_Period", period.Input_Period));
            claims.Add(new Claim("Adj_Period", period.Adj_Period));
            claims.Add(new Claim("Far_Period", period.Far_Period));
            claims.Add(new Claim("Schedule_Cbv_Comment_Period", period.Schedule_Cbv_Comment_Period));
            claims.Add(new Claim("Interim_Closing_Period", period.Interim_Closing_Period));
            claims.Add(new Claim("Year_End_Closing_Period", period.Year_End_Closing_Period));
            claims.Add(new Claim("F52_Interim_Closing_Period", period.F52_Interim_Closing_Period));
            claims.Add(new Claim("F52_Year_End_Closing_Period", period.F52_Year_End_Closing_Period));
            claims.Add(new Claim("Other_Gov_Agent_8100mx_Period", period.Other_Gov_Agent_8100mx_Period));
            claims.Add(new Claim("Re_Budget_Period", period.Re_Budget_Period));
            claims.Add(new Claim("Pe_By_Spec_Ind", HospitalRepository.GetPeBySpecInd(inst_code) ?? "N"));
            return claims;
        }

        public string GetHoAccess(string user_group, string loginId)
        {
            var ho_access = "";
            var userGpHospCnt = UserGroupHospRespository.GetUserGpHospByGpHosp(user_group, "HO").Count;
            if (user_group != "HOSP" && userGpHospCnt > 0)
            { ho_access = "Y"; }
            else
            {
                ho_access = "N";
                loginId = loginId.Replace("\\", "/");
                var userInfoCnt = UserInfoRepository.GetUserInfoByLoginIdHoUser(loginId, "Y").Count;
                if(userInfoCnt > 0)
                    ho_access = "Y";
            }
            return ho_access;
        }

        public string GetClusterLevel(string user_group)
        {
            return UserGroupHospRespository.GetReportClusterLevel(user_group);
        }

        public List<UserAdminModel> GetAdminUserList(string user_group, string fmrs_system, string login_id)
        {
            List<UserAdminModel> result = new List<UserAdminModel>();

            var ds = UserInfoRepository.GetUserList(user_group, fmrs_system, login_id);
            string old_user_group = "";
            DateTime defaultDateTime = DateTime.Parse("01/01/0001 00:00");
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    if (old_user_group != dr["user_group"].ToString())
                    {
                        old_user_group = dr["user_group"].ToString();
                        UserAdminModel model = new UserAdminModel();
                        model.User_group = GetUserGroupDesc(old_user_group);
                        model.User_list = new List<UserInfoViewModel>();
                        result.Add(model);
                    }
                }
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    foreach (UserAdminModel model in result)
                    {
                        if (GetUserGroupDesc(dr["user_group"].ToString()) == model.User_group)
                        {
                            UserInfoViewModel user = new UserInfoViewModel();
                            user.UserInfo = new UserInfo();
                            user.UserInfo.UserGroup = GetUserGroupDesc(dr["user_group"].ToString());
                            user.UserInfo.LoginId = dr["login_id"].ToString();
                            user.UserInfo.UserName = dr["user_name"].ToString();
                            user.UserInfo.PwdExpiry = dr["pwd_expiry"].ToString();
                            user.UserInfo.InstCode = dr["inst_code"].ToString();
                            user.LastLogin_string = dr["last_login"].ToString();
                            model.User_list.Add(user);
                        }
                    }
                }
            }
            return result;
        }

        public string GetUserGroupDesc(string user_group)
        {
            string result = "";
            if (user_group != "HOSP")
            {
                result = UserGroupRespository.GetUserGroupDesc(user_group);
            }
            else
            {
                result = user_group;
            }
            return result;
        }

        public string GetDomainUserByLoginId(string loginId)
        {
            return UserInfoRepository.GetDomainUserByLoginId(loginId);
        }

        public string GetDomainInstByLoginId(string loginId)
        {
            return UserInfoRepository.GetDomainInstByLoginId(loginId);
        }

        public string GetUserGpHospByGp(string user_group)
        {
            string result = "";
            var ds = UserGroupHospRespository.GetUserGpHospByGp(user_group);
            foreach(var hosp in ds)
            {
                if (result != "")
                {
                    result = result + ", ";
                }
                result = result + hosp.HospitalCode;
            }
            return result;
        }

        public string GetUserLastLogin(string loginId)
        {
            return UserInfoRepository.GetUserLastLogin(loginId);
        }

        public void DeleteUserByUserId(string loginId)
        {
            UserInfoRepository.DeleteUserByUserId(loginId);
        }

        public void InsertUserInfo(string loginId, string userName, string hospital, string userGroup, string lastLogin)
        {
            UserInfoRepository.InsertUserByLoginId(loginId, userName, hospital, userGroup, lastLogin);
        }

        public string GetHospitalCode(string user_group, string user_inst_code)
        {
            string result = user_inst_code;
            var hosp_list = UserGroupHospRespository.GetHospitalList(user_group);
            if (hosp_list.Count > 0)
            {
                result = hosp_list[0].HospitalCode;
            }

            return result;

        }
    }
}
