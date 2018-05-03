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
    public interface IAccessRightMRepository : IBaseRepository<AccessRightM>
    {
        List<AccessRightM> GetAllAccessRightM();
        IFMRSUserPrincipal GetProjMgtUserPrivilegeByLoginId(string login_Id);
        AccessRightM GetAccessRightM(string login_Id);
        void UpdateUserAccessRightM(EnquireUserViewModel model, string privilege_admin, string privilege_cluster_admin, string login_id);
        void DeleteCbvAccessControlByLoginId(string loginId);
        void InsertCbvAccessControlByLoginId(string loginId, int tran_id);
        void InsertCbvAccessControlFromCbvTranType(string loginId);
    }
    public class AccessRightMRepository : BaseRepository<AccessRightM>, IAccessRightMRepository
    {
        private FMRSContext Context;
        private DbSet<AccessRightM> accessRight;
        private IFMRSUserPrincipal User;
        private ICwrfAccessControlRepository CwrfAccessControlRepository;
        private IReportNotAccessRepository ReportNotAccessRepository;

        public AccessRightMRepository(FMRSContext _context, IFMRSUserPrincipal _user, ICwrfAccessControlRepository _cwrfAccessControlRepository,
                                        IReportNotAccessRepository _reportNotAccessRepository) : base(_context)
        {
            Context = _context;
            accessRight = Context.Set<AccessRightM>();
            User = _user;
            CwrfAccessControlRepository = _cwrfAccessControlRepository;
            ReportNotAccessRepository = _reportNotAccessRepository;
        }

        public List<AccessRightM> GetAllAccessRightM()
        {
            return Context.AccessRightM.ToList();
        }

        public IFMRSUserPrincipal GetProjMgtUserPrivilegeByLoginId(string login_Id)
        {
            login_Id = login_Id.Replace("\\", "/").ToLower();
            var result = Context.AccessRightM.Where(a => a.LoginId == login_Id).First();
            User.UserGroup_M = result.UserGroup;
            User.Privilege_Admin_M = result.AdminM;
            User.Privilege_Asoi_Rpt_M = result.AsoiRpt;
            User.Privilege_Closing_Report_M = result.Closing;
            User.Privilege_Cbv_Report = result.Cbv;
            User.Privilege_Cbv_Funding = result.CbvFunding;
            User.Privilege_Cluster_Admin = result.ClusterAdminM;
            User.Privilege_Cwrf = result.Cwrf;
            User.Privilege_Cwrf_Funding = result.CwrfFunding;
            User.Privilege_Cwrf_Submenu = result.CwrfSubmenu;
            User.Privilege_Report_M = result.ReportM;
            User.Privilege_Non_Pjt_Report_M = result.NonPjtReport;
            return User;
        }
 
        public AccessRightM GetAccessRightM(string login_Id)
        {
            return Context.AccessRightM.Where(a => a.LoginId == login_Id).FirstOrDefault();
        }
        public void UpdateUserAccessRightM(EnquireUserViewModel model, string privilege_admin, string privilege_cluster_admin, string login_id)
        {
            var sql = "update access_right_m set ";
            sql = sql + "user_group = " + model.AdminRightM.UserGroup + ", ";
            sql = sql + "admin_M = " + model.AdminRightM.AdminM + ", ";
            sql = sql + "cbv = " + model.AdminRightM.Cbv + ", ";
            sql = sql + "cbv_funding = " + model.AdminRightM.CbvFunding + ", ";
            sql = sql + "cluster_admin_M = " + model.AdminRightM.ClusterAdminM + ", ";
            sql = sql + "cwrf = " + model.AdminRightM.Cwrf + ", ";
            sql = sql + "cwrf_funding = " + model.AdminRightM.CwrfFunding + ", ";
            sql = sql + "cwrf_submenu = " + model.AdminRightM.CwrfSubmenu + ", ";
            sql = sql + "report_M = " + model.AdminRightM.ReportM + ", ";
            sql = sql + "cwrf_hpd = " + model.AdminRightM.CwrfHpd + ", ";
            sql = sql + "cwrf_cwd = " + model.AdminRightM.CwrfCwd + ", ";
            sql = sql + "cwrf_ho = " + model.AdminRightM.CwrfHo + ", ";
            sql = sql + "cwrf_status = " + model.AdminRightM.CwrfStatus + ", ";
            sql = sql + "cbv_ori_update = " + model.AdminRightM.CbvOriUpdate + ", ";
            sql = sql + "project = " + model.AdminRightM.Project + " ";
            sql = sql + "where login_id = " + model.AdminUserInfo.LoginId;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
            DeleteCbvAccessControlByLoginId(model.AdminUserInfo.LoginId);
            if (model.AdminRightM.Cbv == "R")
            {
                InsertCbvAccessControlByLoginId(model.AdminUserInfo.LoginId,4);
            }
            if (model.AdminRightM.Cbv == "I")
            {
                InsertCbvAccessControlFromCbvTranType(model.AdminUserInfo.LoginId);
            }
            CwrfAccessControlRepository.DeleteCwrfAccessControlByLoginId(model.AdminUserInfo.LoginId);
            if (model.AdminRightM.Cwrf == "R")
            {
                CwrfAccessControlRepository.InsertCwrfAccessControlByLoginId(model.AdminUserInfo.LoginId, 4);
            }
            if (model.AdminRightM.Cwrf == "I")
            {
                CwrfAccessControlRepository.InsertCwrfAccessControlFromCbvTranType(model.AdminUserInfo.LoginId);
            }
            if (model.AdminRightM.ReportM == "R" && privilege_cluster_admin == "I" && privilege_admin != "I")
            {
                ReportNotAccessRepository.InsertReportNotAccessByLoginId(model.AdminUserInfo.LoginId.Replace("\\", "/"), login_id, model.Modules);
            }
        }

        public void DeleteCbvAccessControlByLoginId(string loginId)
        {
            var sql = "delete cbv_access_control where user_name = '" + loginId.Replace("\\", "/") + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertCbvAccessControlByLoginId(string loginId, int tran_id)
        {
            var sql = "insert into cbv_access_control (user_name, tran_id) values ('" + loginId.Replace("\\", "/") + "', "+ tran_id +")";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertCbvAccessControlFromCbvTranType(string loginId)
        {
            var sql = "insert into cbv_access_control (user_name, tran_id) ";
            sql = sql + "select '" + loginId.Replace("\\", "/") + "', tran_id ";
            sql = sql + "from cbv_tran_type ";
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
