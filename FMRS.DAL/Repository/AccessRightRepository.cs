using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using FMRS.Common.DataSource;

namespace FMRS.DAL.Repository
{
    public interface IAccessRightRepository : IBaseRepository<AccessRight>
    {
        List<AccessRight> GetAllAccessRight();
        IFMRSUserPrincipal GetUserPrivilegeByLoginId(string Login_ID, string fmrs_system);
        DataSet GetUserExistenceByAccessRight(string update_login_id, string modules);
        int GetUserExistenceByAccessRightNotN(string update_login_id, string modules);
        string GetAccessRightRequiredField(string table_name);
        string GetAccessRightRequiredFieldWhereNotNullCause(string table_name);
        void RemoveUser(string login_Id, string access_right_table);

    }
    public class AccessRightRepository : BaseRepository<AccessRight>, IAccessRightRepository
    {
        private FMRSContext Context;
        private DbSet<AccessRight> accessRight;
        private IFMRSUserPrincipal User;
        public AccessRightRepository(FMRSContext _context, IFMRSUserPrincipal _user) : base(_context)
        {
            Context = _context;
            accessRight = Context.Set<AccessRight>();
            User = _user;
        }

        public List<AccessRight> GetAllAccessRight()
        {
            return Context.AccessRight.ToList();
        }

        public IFMRSUserPrincipal GetUserPrivilegeByLoginId(string Login_ID, string fmrs_system)
        {
            Login_ID = Login_ID.Replace("\\", "/").ToLower();
            var result = Context.AccessRight.Where(a => a.LoginId == Login_ID).ToList();
            foreach (var row in result)
            {
                var access_type = row.AccessType;
                var privilege = row.Privilege;
                switch (access_type)
                {
                    //Unknow if useful
                    case "pe_adj":
                        User.Privilege_Pe_Adjust = privilege;
                        break;
                    case "far_access":
                        User.Privilege_Far_Access = privilege;
                        break;
                    case "re_budget":
                        User.Privilege_Re_Budget = privilege;
                        break;
                        /*
                        //User Privilege
                        case "closing":
                            User.Privilege_Closing_Report = privilege;
                            break;
                        case "non_pjt_report":
                            User.Privilege_Non_Pjt_Report = privilege;
                            break;
                        case "asoi_rpt":
                            User.Privilege_Asoi_Rpt = privilege;
                            break;
                        //User Privilege Financial Closing
                        case "report_Y":
                            if (fmrs_system != "F") { User.Privilege_Report = privilege; }
                            break;
                        case "admin_Y":
                            User.Privilege_Admin = privilege;
                            break;
                        case "asoi_input":
                            User.Privilege_Asoi_Input = privilege;
                            break;
                        //User Privilege Project Management
                        case "report_M":
                            if (fmrs_system != "F") { User.Privilege_Report = privilege; }
                            break;
                        case "admin_M":
                            User.Privilege_Admin = privilege;
                            break;
                        case "cluster_admin_M":
                            User.Privilege_Cluster_Admin = privilege;
                            break;
                        case "cbv":
                            User.Privilege_Cbv_Report = privilege;
                            break;
                        case "cbv_funding":
                            User.Privilege_Cbv_Funding = privilege;
                            break;
                        case "cwrf_funding":
                            User.Privilege_Cwrf_Funding = privilege;
                            break;
                        case "cwrf":
                            User.Privilege_Cwrf = privilege;
                            break;
                        case "cwrf_submenu":
                            User.Privilege_Cwrf_Submenu = privilege;
                            break;
                        //User Privilege Donation
                        case "report_D":
                            if (fmrs_system != "F") { User.Privilege_Report = privilege; }
                            break;
                        case "admin_D":
                            User.Privilege_Admin = privilege;
                            break;
                        case "donation":
                            User.Privilege_Donation = privilege;
                            break;
                            */

                }
            }
            return User;
        }

        public DataSet GetUserExistenceByAccessRight(string update_login_id, string modules)
        {
            string access_right = "";
            string access_type = "";
            var sql = "";
            switch (modules)
            {
                case "D":
                    access_right = "access_right_d";break;
                case "M":
                    access_right = "access_right_m"; break;
                case "Y":
                    access_right = "access_right_y"; break;
            }
            access_type = GetAccessRightRequiredField(access_right);
            sql = "select "+ access_type + " from "+ access_right + " where login_id ='" + update_login_id + "'";
            DataSet ds = new DataSet();
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

        public int GetUserExistenceByAccessRightNotN(string update_login_id, string modules)
        {
            string access_right = "";
            var sql = "";
            int result = 0;
            switch (modules)
            {
                case "D":
                    access_right = "access_right_d"; break;
                case "M":
                    access_right = "access_right_m"; break;
                case "Y":
                    access_right = "access_right_y"; break;
            }
            sql = "select count(*) from " + access_right + " where login_id ='" + update_login_id + "'";
            sql = sql + " and " + GetAccessRightRequiredFieldWhereNotNullCause(access_right);
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        result = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    }
                }
            }

            return result;
        }

        public string GetAccessRightRequiredField(string table_name)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = " select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS c  ";
            sql = sql + " inner join login_access t on c.COLUMN_NAME = t.access_type ";
            sql = sql + " WHERE TABLE_NAME = '"+ table_name + "'";
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
                            if (result != "")
                                result = result + ",";
                            result = result + dr["COLUMN_NAME"].ToString();
                        }
                    }
                }
            }
            return result;
        }

        public string GetAccessRightRequiredFieldWhereNotNullCause(string table_name)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = " select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS c  ";
            sql = sql + " inner join login_access t on c.COLUMN_NAME = t.access_type ";
            sql = sql + " WHERE TABLE_NAME = '" + table_name + "'";
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
                            if (result != "")
                                result = result + " is not null and " + result + " != 'N' ";
                            result = result + dr["COLUMN_NAME"].ToString();
                        }
                    }
                }
            }
            return result;
        }

        public void RemoveUser(string login_Id, string access_right_table)
        {
            var sql = " delete from " + access_right_table;
            sql = sql + " WHERE login_id = '" + login_Id + "'";
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
