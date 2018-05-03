using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IFlashRptHospGpRepository // : IBaseRepository<FlashRptHospGp>
    {
        string SelectUnionFlashRptHospGpUserGpHosp(string user_group);
        string SelectFlashRptHospGp();
        string SelectFlashRptHospGpHospQMH();
        DataSet GetErpDonationMovementList(string inst_code, string value_date, int financial_year, int recon_type);
    }
    public class FlashRptHospGpRepository : IFlashRptHospGpRepository// BaseRepository<FlashRptHospGp>, IFlashRptHospGpRepository
    {
        private FMRSContext Context;
        //private DbSet<FlashRptHospGp> flashRptHospGp;
        public FlashRptHospGpRepository(FMRSContext _context) //: base(_context)
        {
            Context = _context;
            //flashRptHospGp = Context.Set<FlashRptHospGp>();
        }

        public string SelectUnionFlashRptHospGpUserGpHosp(string user_group)
        {
            string sql = " select hosp_gp hospital_code, description hospital_desc, ";
            sql = sql + "         sub_description sub_desc, 'Y' consolidateion_ind ";
            sql = sql + "  from flash_rpt_hosp_gp f, user_group_hosp u ";
            sql = sql + "  where consolidate_ind = 'Y' ";
            sql = sql + "    and user_group = '" + user_group + "'";
            sql = sql + "    and not exists (select * from flash_rpt_hosp_gp f2 ";
            sql = sql + "                    where f.hosp_gp = f2.hosp_gp ";
            sql = sql + "                      and consolidate_ind = 'N' ";
            sql = sql + "                      and f2.hospital_code not in (select hospital_code from user_group_hosp u2 ";
            sql = sql + "                                                   where u2.user_group = '" + user_group + "')) ";
            sql = sql + "  union ";
            sql = sql + "  select hosp_gp, description, sub_description, 'N' consolidate_ind ";
            sql = sql + "  from flash_rpt_hosp_gp f, user_group_hosp u ";
            sql = sql + "  where consolidate_ind = 'N' ";
            sql = sql + "    and f.hospital_code = u.hospital_code ";
            sql = sql + "    and user_group = '" + user_group + "'";
            sql = sql + "    and f.hosp_gp = f.hospital_code ";
            sql = sql + " order by consolidateion_ind desc, hospital_desc ";
            return sql;
        }
        public string SelectFlashRptHospGp()
        {
            string sql = "select hosp_gp hospital_code, description hospital_desc, ";
            sql = sql + "        sub_description sub_desc, 'Y' consolidateion_ind";
            sql = sql + " from flash_rpt_hosp_gp ";
            sql = sql + " where consolidate_ind = 'Y' ";
            sql = sql + " union ";
            sql = sql + " select hosp_gp, description, sub_description, 'N' consolidate_ind  ";
            sql = sql + " from flash_rpt_hosp_gp ";
            sql = sql + " where consolidate_ind = 'N' ";
            sql = sql + "   and not exists (select * from flash_rpt_hosp_gp T2 ";
            sql = sql + "                   where flash_rpt_hosp_gp.hosp_gp = T2.hosp_gp ";
            sql = sql + "                     and T2.consolidate_ind = 'Y') ";
            sql = sql + " order by consolidateion_ind desc, hospital_desc, hospital_code   ";
            return sql;
        }
        public string SelectFlashRptHospGpHospQMH()
        {
            string sql = "select hosp_gp hospital_code, description hospital_desc, ";
            sql = sql + "        sub_description sub_desc, 'Y' consolidateion_ind ";
            sql = sql + " from flash_rpt_hosp_gp ";
            sql = sql + " where consolidate_ind = 'Y' ";
            sql = sql + "   and hosp_gp = 'QMH_TYH' ";
            sql = sql + " union ";
            sql = sql + " select hosp_gp, description, sub_description, 'N' consolidate_ind  ";
            sql = sql + " from flash_rpt_hosp_gp ";
            sql = sql + " where consolidate_ind = 'N' ";
            sql = sql + "   and (hosp_gp = 'QMH' or hosp_gp = 'TYH' ) ";
            sql = sql + "   and not exists (select * from flash_rpt_hosp_gp T2 ";
            sql = sql + "                   where flash_rpt_hosp_gp.hosp_gp = T2.hosp_gp ";
            sql = sql + "                     and T2.consolidate_ind = 'Y') ";
            sql = sql + " order by consolidateion_ind desc, hospital_desc, hospital_code  ";
            return sql;
        }
        public DataSet GetErpDonationMovementList(string inst_code, string value_date, int financial_year, int recon_type)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.get_erp_donation_movement", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@hosp_gp", inst_code);
                    sqlCmd.Parameters.AddWithValue("@current_date", value_date);
                    sqlCmd.Parameters.AddWithValue("@financial_year", financial_year);
                    sqlCmd.Parameters.AddWithValue("@recon_type", recon_type);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                    }
                }
            }
            return ds;
        }

    }
}
