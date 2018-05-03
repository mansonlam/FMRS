using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using FMRS.Service;
using FMRS.Helper;
using System.Security.Claims;
using FMRS.Model.DTO;
using FMRS.Common.Resources;

namespace FMRS.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private IMenuService MenuService;
        private IUserService UserService;
        private IAdminService AdminService;
        private ISession Session => HttpContextAccessor.HttpContext.Session;
        public AdminController(IHttpContextAccessor _httpContextAccessor, IMenuService _menuService, IUserService _userService,
                                IAdminService _adminService)
        {
            HttpContextAccessor = _httpContextAccessor;
            MenuService = _menuService;
            UserService = _userService;
            AdminService = _adminService;
        }
        public IActionResult Index(string modules)
        {
            ViewBag.Modules = modules;
            Session.SetString("current_sys", modules);
            var user_group = GetUserGroup(modules);
            SetMenuDropDownList(user_group);
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;

            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            var privilege_cluster_admin = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Cluster_Admin");
            var fv_user_admin = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Fv_User_Admin");
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            var privilege_admin = GetPrivilegeAdmin(modules);
            
            ViewBag.UserInstCode = user_inst_code;
            ViewBag.UserGroup = user_group;
            ViewBag.FvUserAdmin = fv_user_admin;
            ViewBag.DomainUser = "N";
            if (privilege_admin == "I" || privilege_cluster_admin == "I" || fv_user_admin == "edit")
            {
                ViewBag.DomainUser = UserService.GetDomainUserByLoginId(login_id)??"N";
            }
            UserAdminCollection model = new UserAdminCollection();
            model.Collection = UserService.GetAdminUserList(user_group, modules, login_id);
            return View(model);
        }

        
        public IActionResult EnquireUser(string modules,string function_type, string admin_login_id)
        {
            Session.SetString("current_sys", modules);
            var user_group = GetUserGroup(modules);
            SetMenuDropDownList(user_group);
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;

            var privilege_admin = GetPrivilegeAdmin(modules);
            var privilege_cluster_admin = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Cluster_Admin");
            var fv_user_admin = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Fv_User_Admin");
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            var pe_by_spec_ind = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Pe_By_Spec_Ind");
            ViewBag.PrivilegeAdmin = privilege_admin;
            ViewBag.PrivilegeClusterAdmin = privilege_cluster_admin;
            ViewBag.FvUserAdmin = fv_user_admin;
            ViewBag.LoginId = login_id;
            ViewBag.UserGroup = user_group;
            ViewBag.UserInstCode = user_inst_code;
            ViewBag.PeBySpecInd = pe_by_spec_ind;
            ViewBag.DomainInst = UserService.GetDomainInstByLoginId(login_id) ?? "";
            ViewBag.UserGroupList = AdminService.GetUserGroup(user_group);
            
            EnquireUserViewModel model = new EnquireUserViewModel();

            if (function_type == "U")
                model = AdminService.GetEnquireUser(modules, admin_login_id, user_group);
            else
            {
                model.AdminUserInfo = new UserInfo();
                model.FV_User = new FVUser();
                model.AdminRightD = new AccessRightD();
                model.AdminRightM = new AccessRightM();
                model.AdminRightY = new AccessRightY();
                model.AdminUserInfo.LoginId = "";
                model.AdminUserInfo.UserName = "";
                model.AdminUserInfo.Password = "";
                model.AdminUserInfo.InstCode = "";
                model.AdminUserInfo.PwdExpiry = "N";
                model.LastLogin_string = "";
                model.AdminUserInfo.UserGroup = user_group;
                model.AdminUserInfo.DomainUser = "Y";
            }
            if (user_inst_code != "HAHO")
                model.Specialty_list = AdminService.GetUserSpecialtyByLoginID(admin_login_id);
            model.Modules = modules;
            model.Function_type = function_type;
            model.AdminFV_User = AdminService.GetFVUserByLoginId(login_id);
            SetUserDisplayDropDown(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateUser(EnquireUserViewModel model)
        {
            var user_inst_code = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "InstCode");
            var privilege_admin = GetPrivilegeAdmin(model.Modules);
            var privilege_cluster_admin = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Cluster_Admin");
            var login_id = UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "LoginId");
            try
            {
                //AdminService.UpdateUser(model);
                string update_login_id = model.AdminUserInfo.LoginId.Replace("\\", "/");
                string domain_user = update_login_id.ToLower();
                if (model.Function_type == "I")
                {
                    if (!AdminService.CheckUserExistenceByAccessRight(update_login_id, model.Modules))
                    {
                        //User Already Exists
                        TempData["ErrorMessage"] = String.Format(Resource.InsertUserFail, update_login_id);
                        return RedirectToAction("Index", new { modules = model.Modules });
                    }
                }
                if (model.Function_type == "I" || model.Function_type == "U")
                {
                    if (model.AdminUserInfo.UserGroup != "HOSP")
                        model.AdminUserHospital = model.AdminUserInfo.InstCode;
                    if (model.AdminUserHospital == "")
                        model.AdminUserHospital = user_inst_code;
                    if (model.AdminUserHospital.Trim() == "")
                        model.AdminUserHospital = "HAHO";
                    model.LastLogin_string = UserService.GetUserLastLogin(update_login_id);
                    UserService.DeleteUserByUserId(update_login_id);
                    UserService.InsertUserInfo(update_login_id, model.AdminUserInfo.UserName, model.AdminUserHospital, model.AdminUserInfo.UserGroup, model.LastLogin_string);
                }
                else if (model.Function_type == "D")
                {
                    AdminService.RemoveUser(update_login_id, GetAccessRightTable(model.Modules));
                    TempData["SuccessMessage"] = Resource.RemoveUserSuccess;
                }

                if (model.Function_type == "I" || model.Function_type == "U")
                {
                    if (model.Modules == "D")
                    {
                        model.AdminRightD.UserGroup = model.AdminUserInfo.UserGroup;
                        model.AdminRightD.AdminD = model.AdminRightD.AdminD ?? "N";
                        //model.AdminRightD.AsoiRpt = model.AdminRightD.AsoiRpt ?? "N";
                        //model.AdminRightD.Closing = model.AdminRightD.Closing ?? "N";
                        model.AdminRightD.Donation = model.AdminRightD.Donation ?? "N";
                        model.AdminRightD.ReportD = model.AdminRightD.ReportD ?? "N";
                        //model.AdminRightD.NonPjtReport = model.AdminRightD.NonPjtReport ?? "N";
                    }
                    else if (model.Modules == "M")
                    {
                        model.AdminRightM.UserGroup = model.AdminUserInfo.UserGroup;
                        model.AdminRightM.AdminM = model.AdminRightM.AdminM ?? "N";
                        //model.AdminRightM.AsoiRpt = model.AdminRightM.AsoiRpt ?? "N";
                        //model.AdminRightM.Closing = model.AdminRightM.Closing ?? "N";
                        model.AdminRightM.Cbv = model.AdminRightM.Cbv ?? "N";
                        model.AdminRightM.CbvFunding = model.AdminRightM.CbvFunding ?? "N";
                        model.AdminRightM.ClusterAdminM = model.AdminRightM.ClusterAdminM ?? "N";
                        model.AdminRightM.Cwrf = model.AdminRightM.Cwrf ?? "N";
                        model.AdminRightM.CwrfFunding = model.AdminRightM.CwrfFunding ?? "N";
                        model.AdminRightM.CwrfSubmenu = model.AdminRightM.CwrfSubmenu ?? "N";
                        model.AdminRightM.ReportM = model.AdminRightM.ReportM ?? "N";
                        //model.AdminRightM.NonPjtReport = model.AdminRightM.NonPjtReport ?? "N";
                        model.AdminRightM.CwrfHpd = model.AdminRightM.CwrfHpd ?? "N";
                        model.AdminRightM.CwrfCwd = model.AdminRightM.CwrfCwd ?? "N";
                        model.AdminRightM.CwrfHo = model.AdminRightM.CwrfHo ?? "N";
                        model.AdminRightM.CwrfStatus = model.AdminRightM.CwrfStatus ?? "N";
                        model.AdminRightM.CbvOriUpdate = model.AdminRightM.CbvOriUpdate ?? "N";
                        model.AdminRightM.Project = model.AdminRightM.Project ?? "N";
                    }
                    else if (model.Modules == "Y")
                    {
                        model.AdminRightY.UserGroup = model.AdminUserInfo.UserGroup;
                        model.AdminRightY.AdminY = model.AdminRightY.AdminY ?? "N";
                        model.AdminRightY.AsoiInput = model.AdminRightY.AsoiInput ?? "N";
                        model.AdminRightY.AsoiRpt = model.AdminRightY.AsoiRpt ?? "N";
                        model.AdminRightY.Closing = model.AdminRightY.Closing ?? "N";
                        model.AdminRightY.ReportY = model.AdminRightY.ReportY ?? "N";
                        model.AdminRightY.NonPjtReport = model.AdminRightY.NonPjtReport ?? "N";
                        model.AdminRightY.FarAccess = model.AdminRightY.FarAccess ?? "N";
                        model.AdminRightY.FvInput = model.AdminRightY.FvInput ?? "N";
                        model.AdminRightY.FvCluster = model.AdminRightY.FvCluster ?? "N";
                        model.AdminRightY.FvUserAdmin = model.AdminRightY.FvUserAdmin ?? "N";
                    }
                    AdminService.UpdateUserAccessRight(model, privilege_admin, privilege_cluster_admin, login_id, user_inst_code);
                    TempData["SuccessMessage"] = Resource.UpdateUserSuccess;
                }

                //delink User
                if (model.Function_type == "D")
                {
                    int cnt = AdminService.CheckUserExistenceByAccessRightNotN(update_login_id, model.Modules);
                    if (cnt == 0)
                    {
                        if (!AdminService.FrmsCheckAnyUser(update_login_id.ToLower()))
                        {
                            //DelinkUser
                            AdminService.CID_FUNC_delink_userproj(update_login_id.ToLower(), login_id);
                        }
                    }
                }

                if (model.Modules == "Y" && model.AdminRightY.FvInput != "N")
                {
                    AdminService.FvUpdateUser(update_login_id.ToLower(), model.AdminRightY.FvInput, model.AdminRightY.FvCluster, model.AdminRightY.FvUserAdmin);
                }

                return RedirectToAction("Index", new { modules = model.Modules});
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resource.SaveFail + ex;
                return RedirectToAction("Index", new { modules = model.Modules});
            }
        }

        public IActionResult ProjectReport(string modules, string admin_login_id, int group_id)
        {
            Session.SetString("current_sys", modules);
            var user_group = GetUserGroup(modules);
            SetMenuDropDownList(user_group);
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;

            ProjectReportViewModel model = new ProjectReportViewModel();
            model.Report_list = AdminService.GetProjectReportList(modules, admin_login_id, group_id);
            model.Admin_login_id = admin_login_id;
            model.Group_id = group_id;
            model.Modules = modules;
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateProjectReport(ProjectReportViewModel model)
        {
            try
            {
                AdminService.UpdateProjectReport(model);

                TempData["SuccessMessage"] = Resource.SaveSuccess;
                return RedirectToAction("ProjectReport", new { modules = model.Modules, admin_login_id = model.Admin_login_id, group_id = model.Group_id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resource.SaveFail + ex;
                return RedirectToAction("ProjectReport", new { modules = model.Modules, admin_login_id = model.Admin_login_id, group_id = model.Group_id });
            }
        }

        public IActionResult ProjectReportDetail(string modules, string admin_login_id, int group_id, string report_id, string detail_type)
        {
            Session.SetString("current_sys", modules);
            var user_group = GetUserGroup(modules);
            SetMenuDropDownList(user_group);
            ViewBag.IsHtmlMessage = true;
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"] : ViewBag.SuccessMessage;
            ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"] : ViewBag.ErrorMessage;

            ProjectReportDetailViewModel model = new ProjectReportDetailViewModel();
            model.Report = AdminService.GetReportDetail(admin_login_id,report_id, detail_type);
            model.Admin_login_id = admin_login_id;
            model.Group_id = group_id;
            model.Modules = modules;
            model.Detail_type = detail_type;
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProjectReportDetail(ProjectReportDetailViewModel model)
        {
            try
            {
                AdminService.UpdateProjectReportDetail(model);

                TempData["SuccessMessage"] = Resource.SaveSuccess;
                return RedirectToAction("ProjectReport", new { modules = model.Modules, admin_login_id = model.Admin_login_id, group_id=model.Group_id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resource.SaveFail + ex;
                return RedirectToAction("ProjectReport", new { modules = model.Modules, admin_login_id = model.Admin_login_id, group_id = model.Group_id });
            }
        }

       [HttpGet]
        public string GetUserGroupHospList(string user_group)
        {
            return UserService.GetUserGpHospByGp(user_group); 
        }

        private void SetMenuDropDownList(string user_group)
        {
            ViewBag.HospClusterList = MenuService.GetHospitalClusterList(user_group);
            ViewBag.HospClusterExAllList = MenuService.GetHospitalClusterList(user_group, false);
            ViewBag.HospitalExHAHOList = MenuService.GetHospitalList(user_group, false);
            ViewBag.HospitalList = MenuService.GetHospitalList(user_group);
            ViewBag.ClusterList = MenuService.GetClusterList(user_group);
            ViewBag.ClusterExHAHOList = MenuService.GetClusterList(user_group, false);
        }
        private void SetUserDisplayDropDown(EnquireUserViewModel model)
        {
            ViewBag.RightY_Closing = AdminService.DisplayRight("Y", "N");
            ViewBag.RightY_FarAccess = AdminService.DisplayRight("Y", "Y");
            ViewBag.RightY_ASOIInput = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_CWRF = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_CerfHpd = AdminService.DisplayCwrfRight("Y", "N", "N", "N", "N", "Y", "N");
            ViewBag.RightM_CwrfSubmenu = AdminService.DisplayCwrfMenuRight();
            ViewBag.RightM_CwrfCwd = AdminService.DisplayCwrfRight("N", "N", "Y", "Y", "Y", "Y", "N");
            ViewBag.RightM_CwrfHo = AdminService.DisplayCwrfRight("N", "Y", "N", "N", "Y", "Y", "N");
            ViewBag.RightM_CwrfStatus = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_CwrfFunding = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_Cbv = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_CbvOriUpdate = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_CbvFunding = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_CWRF_cluster = AdminService.DisplayRight("R", "N");
            ViewBag.RightM_CerfHpd_cluster = AdminService.DisplayCwrfRight("N", "N", "N", "N", "N", "N", "Y");
            ViewBag.RightM_CwrfFunding_cluster = AdminService.DisplayRight("R", "N");
            ViewBag.RightM_Cbv_cluster = AdminService.DisplayRight("R", "N");
            ViewBag.RightD_Donation = AdminService.DisplayRight("Y", "N");
            ViewBag.RightM_ReportM = AdminService.DisplayRight("N", "N");
            ViewBag.RightY_ReportY = AdminService.DisplayRight("N", "N");
            ViewBag.RightY_AsoiRpt = AdminService.DisplayRight("N", "N");
            ViewBag.RightY_FvInput = AdminService.DisplayRight("FV", "N", model.AdminFV_User.FV_user_cluster,model.FV_User.FV_user_role);
            ViewBag.RightY_FvCluster = AdminService.DisplayRight("FC", "N", model.AdminFV_User.FV_user_cluster,"",model.FV_User.FV_user_cluster);
            ViewBag.RightY_FvUserAdmin = AdminService.DisplayRight("FU", "N");
            ViewBag.RightD_ReportD = AdminService.DisplayRight("N", "N");
        }
        
        private string GetUserGroup(string modules)
        {
            switch (modules)
            {
                case "D":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_D");
                case "M":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_M");
                case "Y":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "UserGroup_Y");
                default:
                    return "";
            }
        }
        private string GetPrivilegeAdmin(string modules)
        {
            switch (modules)
            {
                case "D":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Admin_D");
                case "M":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Admin_M");
                case "Y":
                    return UserHelper.UserInfo(((ClaimsIdentity)User.Identity), "Privilege_Admin_Y");
                default:
                    return "";
            }
        }
        private string GetAccessRightTable(string modules)
        {
            switch (modules)
            {
                case "D":
                    return "access_right_d";
                case "M":
                    return "access_right_m";
                case "Y":
                    return "access_right_y";
                default:
                    return "";
            }
        }

    }
}