using FMRS.DAL.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FMRS.Service
{
    public interface IMenuService
    {
        List<SelectListItem> GetHospitalClusterList(string user_group, bool include_all = true);
        List<SelectListItem> GetHospitalList(string user_group, bool include_HAHO = true);
        List<SelectListItem> GetClusterList(string user_group, bool include_HAHO = true);
        List<SelectListItem> GetCapitalProjSubMenu(string user_name);
        List<SelectListItem> GetCBVProjSubMenu(string user_name);
        List<SelectListItem> GetCBVMinLumpSumSubMenu(string user_name);
        string GetFlashRptHospGpDesc(string inst_code);
    }
    public class MenuService : IMenuService
    {
        private IUserGroupHospRespository UserGroupHospRespository;
        public MenuService(IUserGroupHospRespository _userGroupHospRespository)
        {
            UserGroupHospRespository = _userGroupHospRespository;
        }

        public List<SelectListItem> GetHospitalClusterList(string user_group, bool include_all)
        {
            List<SelectListItem> hosp_cluster_list = new List<SelectListItem>();
            List<SelectListItem> hosp_cluster_exclude_all = new List<SelectListItem>();
            var ds = UserGroupHospRespository.GetHospitalClusterList(user_group);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp_hosp_code = ds.Tables[0].Rows[i][0].ToString().Trim();
                    var temp_cluster = ds.Tables[0].Rows[i][1].ToString().Trim();
                    hosp_cluster_list.Add( new SelectListItem() { Value = temp_hosp_code, Text = temp_cluster });
                    if (temp_hosp_code != "ALL")
                    {
                        hosp_cluster_exclude_all.Add( new SelectListItem() { Value = temp_hosp_code, Text = temp_cluster });
                    }
                }
            }
            return include_all==true? hosp_cluster_list : hosp_cluster_exclude_all;
        }

        public List<SelectListItem> GetHospitalList(string user_group, bool include_HAHO)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var hosp = UserGroupHospRespository.GetHospitalList(user_group);
            if (hosp.Count() > 0)
            {
                result = hosp.Select(h => new SelectListItem() { Value = h.HospitalCode.Trim(), Text = h.HospitalCode.Trim() }).ToList();
            }
            if (include_HAHO) { result.Add(new SelectListItem() { Value = "HAHO", Text = "HAHO" });}

            return result;

        }

        public List<SelectListItem> GetClusterList(string user_group, bool include_HAHO)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = UserGroupHospRespository.GetClusterList(user_group);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp_hosp_code = ds.Tables[0].Rows[i][0].ToString().Trim();
                    var temp_cluster = ds.Tables[0].Rows[i][1].ToString().Trim();
                    result.Add(new SelectListItem() { Value = temp_hosp_code, Text = temp_cluster });
                    if (include_HAHO) {  result.Add(new SelectListItem() { Value = "HAHO", Text = "HAHO" }); }
                }
            }
            return result;
        }

        //Get Capital project submenu
        public List<SelectListItem> GetCapitalProjSubMenu(string user_name)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = UserGroupHospRespository.GetCapitalProjSubMenu(user_name);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp_tran_id = ds.Tables[0].Rows[i][0].ToString().Trim();
                    var temp_tran_type = ds.Tables[0].Rows[i][1].ToString().Trim();
                    result.Add(new SelectListItem() { Value = temp_tran_id, Text = temp_tran_type });
                }
            }
            return result;
        }

        //Get CBV project submenu
        public List<SelectListItem> GetCBVProjSubMenu(string user_name)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = UserGroupHospRespository.GetCBVProjSubMenu(user_name);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp_tran_id = ds.Tables[0].Rows[i][0].ToString().Trim();
                    var temp_tran_type = ds.Tables[0].Rows[i][1].ToString().Trim();
                    result.Add(new SelectListItem() { Value = temp_tran_id, Text = temp_tran_type });
                }
            }
            return result;
        }

        //Get CBV Minor Lump Sum submenu
        public List<SelectListItem> GetCBVMinLumpSumSubMenu(string user_name)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var ds = UserGroupHospRespository.GetCBVMinLumpSumSubMenu(user_name);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var temp_tran_id = ds.Tables[0].Rows[i][0].ToString().Trim();
                    var temp_tran_type = ds.Tables[0].Rows[i][1].ToString().Trim();
                    result.Add(new SelectListItem() { Value = temp_tran_id, Text = temp_tran_type });
                }
            }
            return result;
        }

        public string GetFlashRptHospGpDesc(string inst_code)
        {
            return UserGroupHospRespository.GetFlashRptHospGpDesc(inst_code);
        }
    }
}
