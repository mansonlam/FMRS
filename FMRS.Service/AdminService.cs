using FMRS.Common.Resources;
using FMRS.DAL.Repository;
using FMRS.Model.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FMRS.Service
{
    public interface IAdminService
    {
        EnquireUserViewModel GetEnquireUser(string modules, string loginId, string user_group);
        FVUser GetFVUserByLoginId(string loginId);
        List<SelectListItem> DisplayCwrfMenuRight();
        List<SelectListItem> DisplayCwrfRight(string with_fe, string with_fi, string with_mnt, string with_imp, string with_none, string with_e, string with_y);
        List<SelectListItem> DisplayRight(string with_edit, string with_approve, string fv_admin_user_cluster = "", string fv_user_role = "", string fv_user_cluster = "");
        List<SelectListItem> GetUserGroup(string user_group);
        string GetPrivilage(string access_type, string admin_login_id, string privilage_default);
        string GetUserSpecialtyByLoginID(string loginId);
        List<ProjectReportModel> GetProjectReportList(string modules, string admin_login_id, int group_id);
        string GetReportNameByReportId(string report_id);
        ProjectReportModel GetReportDetail(string admin_login_id, string report_id, string detail_type);
        List<ProjectReportDetailRightModel> GetProjectReportDetailRightList(string admin_login_id, string report_id, string detail_type);

        #region Update Action
        void UpdateProjectReportDetail(ProjectReportDetailViewModel model);
        void UpdateProjectReport(ProjectReportViewModel model);
        void UpdateUserAccessRight(EnquireUserViewModel model, string privilege_admin, string privilege_cluster_admin, string login_id, string user_inst_code);
        bool CheckUserExistenceByAccessRight(string update_login_id, string modules);
        int CheckUserExistenceByAccessRightNotN(string update_login_id, string modules);
        void RemoveUser(string update_login_id, string modules);
        bool FrmsCheckAnyUser(string loginId);
        void CID_FUNC_delink_userproj(string domain_user, string loginId);
        void FvUpdateUser(string domain_user, string fvInput, string fvCluster, string fvUserAdmin);
        #endregion
    }
    public class AdminService : IAdminService
    {
        private IAccessRightRepository AccessRightRepository;
        private IAccessRightDRepository AccessRightDRepository;
        private IAccessRightMRepository AccessRightMRepository;
        private IAccessRightYRepository AccessRightYRepository;
        private IUserInfoRepository UserInfoRepository;
        private IUserSpecialtyRespository UserSpecialtyRespository;
        private IUserGroupRespository UserGroupRespository;
        private IFinancialClosingRepository FinancialClosingRepository;
        private IReportRepository ReportRepository;
        private IReportNotAccessRepository ReportNotAccessRepository;
        private IReportDetailNotAccessRepository ReportDetailNotAccessRepository;
        private IFRMSModelRepository FRMSModelRepository;
        private ICIDRepository CIDRepository;
        public AdminService(IAccessRightRepository _accessRightRepository, IUserInfoRepository _userInfoRepository,
                            IUserSpecialtyRespository _userSpecialtyRespository, IUserGroupRespository _userGroupRespository,
                            IFinancialClosingRepository _financialClosingRepository, IReportRepository _reportRepository,
                            IReportNotAccessRepository _reportNotAccessRepository, IReportDetailNotAccessRepository _reportDetailNotAccessRepository,
                            IAccessRightDRepository _accessRightDRepository,IAccessRightMRepository _accessRightMRepository,
                            IAccessRightYRepository _accessRightYRepository, IFRMSModelRepository _fRMSModelRepository,
                            ICIDRepository _cIDRepository)
        {
            AccessRightRepository = _accessRightRepository;
            AccessRightDRepository = _accessRightDRepository;
            AccessRightMRepository = _accessRightMRepository;
            AccessRightYRepository = _accessRightYRepository;
            UserInfoRepository = _userInfoRepository;
            UserSpecialtyRespository = _userSpecialtyRespository;
            UserGroupRespository = _userGroupRespository;
            FinancialClosingRepository = _financialClosingRepository;
            ReportRepository = _reportRepository;
            ReportNotAccessRepository = _reportNotAccessRepository;
            ReportDetailNotAccessRepository = _reportDetailNotAccessRepository;
            FRMSModelRepository = _fRMSModelRepository;
            CIDRepository = _cIDRepository;
        }

        public EnquireUserViewModel GetEnquireUser(string modules, string loginId, string user_group)
        {
            EnquireUserViewModel model = UserInfoRepository.GetEnquireUser(modules, loginId, user_group);
            model.FV_User = GetFVUserByLoginId(loginId);
            return model;
        }

        public FVUser GetFVUserByLoginId(string loginId)
        {
            return FinancialClosingRepository.GetFVUserByLoginId(loginId);
        }

        public List<SelectListItem> DisplayCwrfMenuRight()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Value = "I", Text = Resource.IMP });
            result.Add(new SelectListItem() { Value = "N", Text = Resource.None });
            return result;
        }

        public List<SelectListItem> DisplayCwrfRight(string with_fe, string with_fi, string with_mnt, string with_imp, string with_none, string with_e, string with_y)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            if(with_fe == "Y")
                result.Add(new SelectListItem() { Value = "F", Text = Resource.EditAfterEndorsed });
            if (with_fi == "Y")
                result.Add(new SelectListItem() { Value = "F", Text = Resource.EditAndInterface });
            if (with_e == "Y")
                result.Add(new SelectListItem() { Value = "I", Text = Resource.Edit });
            if (with_y == "Y")
                result.Add(new SelectListItem() { Value = "I", Text = Resource.Yes });
            result.Add(new SelectListItem() { Value = "R", Text = Resource.ReadOnly });
            if (with_none == "Y")
                result.Add(new SelectListItem() { Value = "N", Text = with_y == "Y"?Resource.No: Resource.None });
            return result;
        }

        public List<SelectListItem> DisplayRight(string with_edit, string with_approve,string fv_admin_user_cluster, string fv_user_role, string fv_user_cluster)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            if (with_edit == "FV")
            {
                result.Add(new SelectListItem() { Value = "N", Text = Resource.None });
                if (fv_admin_user_cluster == "HAHO")
                {
                    if (fv_user_role == "CU") result.Add(new SelectListItem() { Value = "CU", Text = Resource.ClusterUser });
                    if (fv_user_role == "CA") result.Add(new SelectListItem() { Value = "CA", Text = Resource.ClusterApprover });
                    result.Add(new SelectListItem() { Value = "HU", Text = Resource.HOUser });
                    result.Add(new SelectListItem() { Value = "HA", Text = Resource.HOApprover });
                }
                else
                {
                    if (fv_user_role == "HU") result.Add(new SelectListItem() { Value = "HU", Text = Resource.HOUser });
                    if (fv_user_role == "HA") result.Add(new SelectListItem() { Value = "HA", Text = Resource.HOApprover });
                    result.Add(new SelectListItem() { Value = "CU", Text = Resource.ClusterUser });
                    result.Add(new SelectListItem() { Value = "CA", Text = Resource.ClusterApprover });
                }
            }
            else if (with_edit == "FU")
            {
                result.Add(new SelectListItem() { Value = "N", Text = Resource.None });
                result.Add(new SelectListItem() { Value = "edit", Text = Resource.Edit });
            }
            else if (with_edit == "FC")
            {
                result.Add(new SelectListItem() { Value = "N", Text = Resource.None });
                string switch_val = fv_user_cluster != fv_admin_user_cluster ? fv_admin_user_cluster : fv_user_cluster;
                switch (switch_val)
                {
                    case "HAHO": result.Add(new SelectListItem() { Value = "HAHO", Text = Resource.HAHO }); break;
                    case "HEC": result.Add(new SelectListItem() { Value = "HEC", Text = "HEC" }); break;
                    case "HWC": result.Add(new SelectListItem() { Value = "HWC", Text = "HWC" }); break;
                    case "KEC": result.Add(new SelectListItem() { Value = "KEC", Text = "KEC" }); break;
                    case "KCC": result.Add(new SelectListItem() { Value = "KCC", Text = "KCC" }); break;
                    case "KWC": result.Add(new SelectListItem() { Value = "KWC", Text = "KWC" }); break;
                    case "NTEC": result.Add(new SelectListItem() { Value = "NTEC", Text = "NTEC" }); break;
                    case "NTWC": result.Add(new SelectListItem() { Value = "NTWC", Text = "NTWC" }); break;
                }
            }
            else
            {
                if (with_edit == "Y")
                {
                    result.Add(new SelectListItem() { Value = "I", Text = Resource.Edit });
                    if (with_approve == "Y")
                    {
                        result.Add(new SelectListItem() { Value = "A", Text = Resource.Approve });
                    }
                }
                result.Add(new SelectListItem() { Value = "R", Text = (with_edit=="Y"?Resource.ReadOnly:(with_edit=="N"?Resource.Generate: Resource.ReadOnly)) });
                result.Add(new SelectListItem() { Value = "N", Text = Resource.None });
            }

            return result;
        }

        public List<SelectListItem> GetUserGroup(string user_group)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            List<UserGroup> gp_list = new List<UserGroup>();
            DataSet ds = new DataSet();
            if (user_group == "ALL")
            { 
                gp_list = UserGroupRespository.GetUserGroup();
                foreach (UserGroup gp in gp_list)
                {
                    result.Add(new SelectListItem() { Value = gp.UserGroup1, Text = gp.Description });
                }
            }
            else
            { 
                ds = UserGroupRespository.GetUserGroupDescFromUserGpHosp(user_group);
                if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(new SelectListItem() { Value = dr["user_group"].ToString(), Text = dr["description"].ToString() });
                    }
                }
            }
            result.Add(new SelectListItem() { Value = "HOSP", Text = "HOSP" });
            return result;
        }

        public string GetPrivilage(string access_type, string admin_login_id, string privilage_default)
        {
            string privilage = privilage_default;
            var recordD = AccessRightDRepository.GetAccessRightD(admin_login_id);
            var recordM = AccessRightMRepository.GetAccessRightM(admin_login_id);
            var recordY = AccessRightYRepository.GetAccessRightY(admin_login_id);
            switch (access_type)
            {
                case "admin_D":
                    privilage = recordD.AdminD; break;
                case "admin_M":
                    privilage = recordM.AdminM; break;
                case "admin_Y":
                    privilage = recordY.AdminY; break;
                case "asoi_input":
                    privilage = recordY.AsoiInput; break;
                case "asoi_rpt_D":
                    privilage = recordD.AsoiRpt; break;
                case "asoi_rpt_M":
                    privilage = recordM.AsoiRpt; break;
                case "asoi_rpt_Y":
                    privilage = recordY.AsoiRpt; break;
                case "cbv":
                    privilage = recordM.Cbv; break;
                case "cbv_funding":
                    privilage = recordM.CbvFunding; break;
                case "cbv_ori_update":
                    privilage = recordM.CbvOriUpdate; break;
                case "closing_Y":
                    privilage = recordY.Closing; break;
                case "cluster_admin_M":
                    privilage = recordM.ClusterAdminM; break;
                
                case "cwrf":
                    privilage = recordM.Cwrf; break;
                case "cwrf_hpd":
                    privilage = recordM.CwrfHpd; break;
                case "cwrf_cwd":
                    privilage = recordM.CwrfCwd; break;
                case "cwrf_funding":
                    privilage = recordM.CwrfFunding; break;
                case "cwrf_ho":
                    privilage = recordM.CwrfHo; break;
                case "cwrf_status":
                    privilage = recordM.CwrfStatus; break;
                case "cwrf_submenu":
                    privilage = recordM.CwrfSubmenu; break;
                case "donation":
                    privilage = recordD.Donation; break;
                case "far_access":
                    privilage = recordY.FarAccess; break;
                case "fv_input":
                    privilage = recordY.FvInput; break;
                case "fv_cluster":
                    privilage = recordY.FvCluster; break;
                case "fv_user_admin":
                    privilage = recordY.FvUserAdmin; break;
                case "report_D":
                    privilage = recordD.ReportD; break;
                case "report_M":
                    privilage = recordM.ReportM; break;
                case "report_Y":
                    privilage = recordY.ReportY; break;
            }
            return privilage;
        }

        public string GetUserSpecialtyByLoginID(string loginId)
        {
            var specialty_list = UserSpecialtyRespository.GetUserSpecialtyByLoginID(loginId);
            var result = "";
            foreach (string specialty in specialty_list)
            {
                result = result + "*" + specialty + "*";
            }
            return result;
        }

        public List<ProjectReportModel> GetProjectReportList(string modules, string admin_login_id, int group_id)
        {
            List<ProjectReportModel> result = new List<ProjectReportModel>();
            var ds = ReportRepository.GetReportByGroupID(group_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ProjectReportModel item = new ProjectReportModel();
                    item.Report_id = dr["id"].ToString();
                    item.Rpt_name = dr["rpt_name"].ToString();
                    item.Show_project_item_ind = dr["show_project_item_ind"].ToString();
                    item.Cwrf_recur = dr["cwrf_recur"].ToString();
                    item.Cnt = ReportNotAccessRepository.GetReportCntByLoginIdRptID(admin_login_id, item.Report_id, modules);
                    result.Add(item);
                }
            }
            return result;
        }

        public string GetReportNameByReportId(string report_id)
        {
            string report_name = "";
            var ds = ReportRepository.GetReportByReportId(report_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                report_name = ds.Tables[0].Rows[0]["rpt_name"].ToString();
            }
            return report_name;
        }
        public ProjectReportModel GetReportDetail(string admin_login_id, string report_id, string detail_type)
        {
            ProjectReportModel model = new ProjectReportModel();
            model.Report_id = report_id;
            model.Rpt_name = GetReportNameByReportId(report_id);
            model.Project_right = GetProjectReportDetailRightList(admin_login_id, report_id, detail_type);
            return model;
        }

        public List<ProjectReportDetailRightModel> GetProjectReportDetailRightList(string admin_login_id, string report_id, string detail_type)
        {
            List<ProjectReportDetailRightModel> result = new List<ProjectReportDetailRightModel>();
            var ds = ReportRepository.GetReportDetailRightByDetailTypeRptId(detail_type, report_id);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    ProjectReportDetailRightModel item = new ProjectReportDetailRightModel();
                    item.Id = dr["id"].ToString();
                    item.Description = dr["description"].ToString();
                    if (detail_type == "1" && (report_id == "17" || report_id == "18"))
                    {
                        item.Id_type = dr["id_type"].ToString();
                    }
                    else
                        item.Id_type = "";
                    item.Cnt = ReportNotAccessRepository.GetReportCntByLoginIdRptIDIdType(admin_login_id, report_id, detail_type, item.Id, item.Id_type);
                    result.Add(item);
                }
            }
            return result;
        }

        #region Update Action
        public void UpdateProjectReportDetail(ProjectReportDetailViewModel model)
        {
            int can_gen = ReportDetailNotAccessRepository.UpdateReportDetailNotAccessByRptId(model.Admin_login_id, model.Report.Report_id, model.Admin_login_id,model.Access_right);
            ReportNotAccessRepository.UpdateReportNotAccessByCanGen(can_gen, model.Admin_login_id, model.Report.Report_id);
        }

        public void UpdateProjectReport(ProjectReportViewModel model)
        {
            ReportNotAccessRepository.UpdateReportNotAccessByLoginIdRptId(model);
        }

        public void UpdateUserAccessRight(EnquireUserViewModel model, string privilege_admin, string privilege_cluster_admin, string login_id, string user_inst_code)
        {
            if (model.Modules == "Y")
            {
                AccessRightYRepository.UpdateUserAccessRightY(model);
            }
            else if (model.Modules == "M")
            {
                AccessRightMRepository.UpdateUserAccessRightM(model, privilege_admin, privilege_cluster_admin, login_id);
            }
            else if (model.Modules == "D")
            {
                AccessRightDRepository.UpdateUserAccessRightD(model);
            }

            //user_specialty
            UserSpecialtyRespository.DeleteUserSpecialtyByLoginId(model.AdminUserInfo.LoginId);
            if (user_inst_code == "HAHO")
            {
                model.All_specialty = "on";
            }
            if (model.All_specialty == "on")
            {
                UserSpecialtyRespository.InsertUserSpecialtyByLoginIdSpecialty(login_id, "*ALL*");
            }
            else
            {
                string[] user_spec_code_list = model.UserSpecCode.Split(", ");
                foreach (string code in user_spec_code_list)
                {
                    UserSpecialtyRespository.InsertUserSpecialtyByLoginIdSpecialty(login_id, code);
                }
            }

        }

        public bool CheckUserExistenceByAccessRight(string update_login_id, string modules)
        {
            bool result = false;
            var ds = AccessRightRepository.GetUserExistenceByAccessRight(update_login_id, modules);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                int colCount = ds.Tables[0].Columns.Count;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    for (int i = 0; i < colCount; i++)
                    {
                        if (!DBNull.Value.Equals(dr[i]))
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public int CheckUserExistenceByAccessRightNotN(string update_login_id, string modules)
        {
            return AccessRightRepository.GetUserExistenceByAccessRightNotN(update_login_id, modules);
        }
        public void RemoveUser(string update_login_id, string access_right_table)
        {
            AccessRightRepository.RemoveUser(update_login_id, access_right_table);
        }

        public bool FrmsCheckAnyUser(string loginId)
        {
            return FRMSModelRepository.FrmsCheckAnyUser(loginId);
        }

        public void CID_FUNC_delink_userproj(string domain_user, string loginId)
        {
            CIDRepository.CID_FUNC_delink_userproj(domain_user, loginId);
        }

        public void FvUpdateUser(string domain_user, string fvInput, string fvCluster, string fvUserAdmin)
        {
            FinancialClosingRepository.FvUpdateUser(domain_user, fvInput, fvCluster, fvUserAdmin);
        }
        #endregion
    }
}
