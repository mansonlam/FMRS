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
    public interface IAccessRightYRepository : IBaseRepository<AccessRightY>
    {
        List<AccessRightY> GetAllAccessRightY();
        IFMRSUserPrincipal GetFinClosingUserPrivilegeByLoginId(string login_Id);
        AccessRightY GetAccessRightY(string login_Id);
        void UpdateUserAccessRightY(EnquireUserViewModel model);
    }
    public class AccessRightYRepository : BaseRepository<AccessRightY>, IAccessRightYRepository
    {
        private FMRSContext Context;
        private DbSet<AccessRightY> accessRight;
        private IFMRSUserPrincipal User;
        public AccessRightYRepository(FMRSContext _context, IFMRSUserPrincipal _user) : base(_context)
        {
            Context = _context;
            accessRight = Context.Set<AccessRightY>();
            User = _user;
        }

        public List<AccessRightY> GetAllAccessRightY()
        {
            return Context.AccessRightY.ToList();
        }

        public IFMRSUserPrincipal GetFinClosingUserPrivilegeByLoginId(string login_Id)
        {
            login_Id = login_Id.Replace("\\", "/").ToLower();
            var result = Context.AccessRightY.Where(a => a.LoginId == login_Id).First();
            User.UserGroup_Y = result.UserGroup;
            User.Privilege_Admin_Y = result.AdminY;
            User.Privilege_Asoi_Input = result.AsoiInput;
            User.Privilege_Asoi_Rpt_Y = result.AsoiRpt;
            User.Privilege_Closing_Report_Y = result.Closing;
            User.Privilege_Report_Y = result.ReportY;
            User.Privilege_Non_Pjt_Report_Y = result.NonPjtReport;
            return User;
        }

        public AccessRightY GetAccessRightY(string login_Id)
        {
            return Context.AccessRightY.Where(a => a.LoginId == login_Id).FirstOrDefault();
        }

        public void UpdateUserAccessRightY(EnquireUserViewModel model)
        {
            var sql = "update access_right_y set ";
            sql = sql + "user_group = " + model.AdminRightY.UserGroup + ", ";
            sql = sql + "admin_Y = " + model.AdminRightY.AdminY + ", ";
            sql = sql + "asoi_input = " + model.AdminRightY.AsoiInput + ", ";
            sql = sql + "closing = " + model.AdminRightY.Closing + ", ";
            sql = sql + "report_Y = " + model.AdminRightY.ReportY + ", ";
            sql = sql + "non_pjt_report = " + model.AdminRightY.NonPjtReport + ", ";
            sql = sql + "far_access = " + model.AdminRightY.FarAccess + ", ";
            sql = sql + "fv_input = " + model.AdminRightY.FvInput + ", ";
            sql = sql + "fv_cluster = " + model.AdminRightY.FvCluster + ", ";
            sql = sql + "fv_user_admin = " + model.AdminRightY.FvUserAdmin + " ";
            sql = sql + "where login_id = " + model.AdminUserInfo.LoginId;
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
