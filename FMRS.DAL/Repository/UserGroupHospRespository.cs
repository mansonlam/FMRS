using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace FMRS.DAL.Repository
{
    public interface IUserGroupHospRespository : IBaseRepository<UserGroupHosp>
    {
        DataSet GetHospitalClusterList(string user_group);
        List<UserGroupHosp> GetHospitalList(string user_group);
        List<UserGroupHosp> GetUserGpHospByGp(string user_group);
        DataSet GetClusterList(string user_group);
        DataSet GetCapitalProjSubMenu(string user_name);
        DataSet GetCBVProjSubMenu(string user_name);
        DataSet GetCBVMinLumpSumSubMenu(string user_name);
        string GetFlashRptHospGpDesc(string inst_code);
        List<UserGroupHosp> GetUserGpHospByGpHosp(string user_group, string hospital);
        string GetReportClusterLevel(string user_group);
        DataSet GetHospClusterList_forM(string user_group);
    }

    public class UserGroupHospRespository : BaseRepository<UserGroupHosp>, IUserGroupHospRespository
    {
        private FMRSContext Context;
        private DbSet<UserGroupHosp> userGroupHosp;
        public UserGroupHospRespository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            userGroupHosp = Context.Set<UserGroupHosp>();
        }

        public DataSet GetHospitalClusterList(string user_group)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.donation_user_group_access", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@user_group", user_group);
                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public List<UserGroupHosp> GetHospitalList(string user_group)
        {
            return Context.UserGroupHosp.Where(u => u.UserGroup == user_group).OrderBy(u => u.HospitalCode).ToList();
        }

        public DataSet GetClusterList(string user_group)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct rtrim(T2.hospital_code) hospital_code, T2.Cluster cluster from user_group_hosp T1, hospital T2, hospital T3 ";
            sql = sql + "where T1.user_group = '" + user_group + "' and T2.cce = 'Y' and T3.Cluster = T2.Cluster and T1.hospital_code = T3.hospital_code ";
            sql = sql + "order by T2.hospital_code";

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

        //Get Capital project submenu
        public DataSet GetCapitalProjSubMenu(string user_name)
        {
            DataSet ds = new DataSet();
            var sql = "select h.tran_id, h.tran_type from cwrf_tran_type h, cwrf_access_control c ";
            sql = sql + "where h.tran_id = c.tran_id and user_name = '" + user_name + "' order by h.tran_id";

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

        //Get CBV project submenu
        public DataSet GetCBVProjSubMenu(string user_name)
        {
            DataSet ds = new DataSet();
            var sql = "select h.tran_id, h.tran_type from cbv_tran_type h, cbv_access_control c ";
            sql = sql + "where h.tran_id = c.tran_id  and h.tran_id not in (1,3)and user_name ='" + user_name + "' order by h.tran_id";

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

        //Get CBV Minor Lump Sum submenu
        public DataSet GetCBVMinLumpSumSubMenu(string user_name)
        {
            DataSet ds = new DataSet();
            var sql = "select h.tran_id, h.tran_type from cbv_min_tran_type h, cbv_access_control c ";
            sql = sql + " where h.tran_id = c.tran_id and h.tran_id not in (2,3) and user_name = '" + user_name + "' order by h.tran_id";

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

        public string GetFlashRptHospGpDesc(string inst_code)
        {
            DataSet ds = new DataSet();
            var sql = "select description from flash_rpt_hosp_gp where hosp_gp = '" + inst_code + "' and consolidate_ind = 'Y'";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return inst_code;
        }

        public List<UserGroupHosp> GetUserGpHospByGpHosp(string user_group, string hospital)
        {
            return Context.UserGroupHosp.Where(u => u.UserGroup == user_group && u.HospitalCode == hospital).ToList();
        }
        public List<UserGroupHosp> GetUserGpHospByGp(string user_group)
        {
            return Context.UserGroupHosp.Where(u => u.UserGroup == user_group).OrderBy(u => u.HospitalCode).ToList();
        }

        public string GetReportClusterLevel(string user_group)
        {
            var cluster_level = "N";
            DataSet ds = new DataSet();
            var sql = "select count(*) cnt ";
            sql = sql + " from user_group_hosp T1, budget_cluster T3";
            sql = sql + " where T1.user_group = '" + user_group + "' ";
            sql = sql + "   and T1.hospital_code = T3.hospital ";
            sql = sql + "   and T3.cluster is not null ";
            sql = sql + "   and T3.cluster != 'Other' ";
            sql = sql + "   and not exists (select * from budget_cluster T4  ";
            sql = sql + "                   where T3.cluster = T4.cluster ";
            sql = sql + "                     and T4.hospital not in (select hospital_code from user_group_hosp T5 ";
            sql = sql + "                                             where T1.user_group = T5.user_group)) ";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        cluster_level = (Convert.ToInt32(ds.Tables[0].Rows[0]["cnt"]) > 0) ? "Y" : "N";
                    }
                }
            }
            return cluster_level;
        }

        public DataSet GetHospClusterList_forM(string user_group)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct rtrim(T2.hospital_code) hospital_code, T2.Cluster ";
            sql = sql + " from user_group_hosp T1, hospital T2, hospital T3";
            sql = sql + " where T1.user_group = '" + user_group + "' ";
            sql = sql + "   and T2.cce = 'Y' ";
            sql = sql + "   and T3.Cluster = T2.Cluster ";
            sql = sql + "   and T1.hospital_code = T3.hospital_code ";
            sql = sql + " order by T2.hospital_code";

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


    }
}
