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
    public interface IAccessRightDRepository : IBaseRepository<AccessRightD>
    {
        List<AccessRightD> GetAllAccessRightD();
        IFMRSUserPrincipal GetDonationUserPrivilegeByLoginId(string login_Id);
        AccessRightD GetAccessRightD(string login_Id);
        void UpdateUserAccessRightD(EnquireUserViewModel model);

    }
    public class AccessRightDRepository : BaseRepository<AccessRightD>, IAccessRightDRepository
    {
        private FMRSContext Context;
        private DbSet<AccessRightD> accessRight;
        private IFMRSUserPrincipal User;
        public AccessRightDRepository(FMRSContext _context, IFMRSUserPrincipal _user) : base(_context)
        {
            Context = _context;
            accessRight = Context.Set<AccessRightD>();
            User = _user;
        }

        public List<AccessRightD> GetAllAccessRightD()
        {
            return Context.AccessRightD.ToList();
        }

        public IFMRSUserPrincipal GetDonationUserPrivilegeByLoginId(string login_Id)
        {
            login_Id = login_Id.Replace("\\", "/").ToLower();
            var result = Context.AccessRightD.Where(a => a.LoginId == login_Id).First();
            User.UserGroup_D = result.UserGroup;
            User.Privilege_Admin_D = result.AdminD;
            User.Privilege_Asoi_Rpt_D = result.AsoiRpt;
            User.Privilege_Closing_Report_D = result.Closing;
            User.Privilege_Donation = result.Donation;
            User.Privilege_Report_D = result.ReportD;
            User.Privilege_Non_Pjt_Report_D = result.NonPjtReport;
            return User;
        }

        public AccessRightD GetAccessRightD(string login_Id)
        {
            return Context.AccessRightD.Where(a => a.LoginId == login_Id).FirstOrDefault();
        }

        public void UpdateUserAccessRightD(EnquireUserViewModel model)
        {
            var sql = "update access_right_d set ";
            sql = sql + "user_group = " + model.AdminRightD.UserGroup + ", ";
            sql = sql + "admin_D = " + model.AdminRightD.AdminD + ", ";
            sql = sql + "donation = " + model.AdminRightD.Donation + ", ";
            sql = sql + "report_D = " + model.AdminRightD.ReportD + " ";
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
