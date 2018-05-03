using FMRS.DAL.Repository;
using FMRS.Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FMRS.Service
{
    public interface IDonationRecNExpReconService
    {
        int GetTrendTbCntByValueDate(string value_date);
        List<DonationMovement> GetErpDonationMovementList(string inst_code, string value_date, int financial_year, int recon_type, out decimal subtotal);
        decimal GetDonationMovement(string inst_code, string value_date, int financial_year, int recon_type, int fund);
        List<DonationRecNExpModel> GetDonationList(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated);
    }
    public class DonationRecNExpReconService : IDonationRecNExpReconService
    {
        private IFinancialClosingRepository FinancialClosingRepository;
        private IFlashRptHospGpRepository FlashRptHospGpRepository;
        private IDonationDetailRepository DonationDetailRepository;

        public DonationRecNExpReconService(IFinancialClosingRepository _financialClosingRepository, IFlashRptHospGpRepository _flashRptHospGpRepository,
                                           IDonationDetailRepository _donationDetailRepository)
        {
            FinancialClosingRepository = _financialClosingRepository;
            FlashRptHospGpRepository = _flashRptHospGpRepository;
            DonationDetailRepository = _donationDetailRepository;
        }
        public int GetTrendTbCntByValueDate(string value_date)
        {
            return FinancialClosingRepository.GetTrendTbCntByValueDate(value_date);
        }

        public List<DonationMovement> GetErpDonationMovementList(string inst_code, string value_date, int financial_year, int recon_type, out decimal total)
        {
            total = 0;
            List<DonationMovement> result = new List<DonationMovement>();
            var ds = FlashRptHospGpRepository.GetErpDonationMovementList(inst_code, value_date, financial_year, recon_type);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationMovement model = new DonationMovement();
                    model.Fund = dr["fund"].ToString();
                    model.Account = dr["account"].ToString();
                    model.Cost = Convert.ToDecimal(dr["cost"]);
                    result.Add(model);
                    total = total + model.Cost;
                }
            }
            return result;
        }
        public decimal GetDonationMovement(string inst_code, string value_date, int financial_year, int recon_type, int fund)
        {
            decimal result = 0;
            var ds = FinancialClosingRepository.GetDonationMovement(inst_code, value_date, financial_year, recon_type, fund);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                result = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            }
            return result;
        }
        public List<DonationRecNExpModel> GetDonationList(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated)
        {
            List<DonationRecNExpModel> result = new List<DonationRecNExpModel>();
            var ds = DonationDetailRepository.GetDonationList(inst_code, show_year, financial_year, value_date, cur_year_only, in_donor_name, inc_exp, designated);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationRecNExpModel model = new DonationRecNExpModel();
                    model.Id = Convert.ToInt32(dr["id"]);
                    model.Hospital = dr["hospital"].ToString();
                    model.Trust = Convert.ToInt16((dr["trust"] is DBNull) ? 0 : dr["trust"]);
                    model.Donor_type = dr["donor_type"].ToString();
                    model.Fund = dr["fund"].ToString();
                    model.Section = dr["section"].ToString();
                    model.Analytical = dr["analytical"].ToString();
                    model.Donor_name = dr["donor_name"].ToString();
                    model.Don_inc_exp = dr["don_inc_exp"].ToString();
                    model.Don_date = dr["don_date"].ToString();
                    model.Don_type = dr["don_type"].ToString();
                    model.Don_type_desc = dr["don_type_desc"].ToString();
                    model.Don_purpose = Convert.ToInt16((dr["don_purpose"] is DBNull) ? 0 : dr["don_purpose"]);
                    model.Don_purpose_desc = dr["don_purpose_desc"].ToString();
                    model.Don_cat = Convert.ToInt32((dr["don_cat"] is DBNull) ? 0 : dr["don_cat"]);
                    model.Don_cat_desc = dr["don_cat_desc"].ToString();
                    model.Don_subcat = Convert.ToInt32((dr["don_subcat"] is DBNull) ? 0 : dr["don_subcat"]);
                    model.Eqt_desc = Convert.ToInt16((dr["eqt_desc"] is DBNull) ? 0 : dr["eqt_desc"]);
                    model.Recurrent_con = dr["recurrent_con"].ToString();
                    model.Recurrent_cost = Convert.ToDecimal((dr["recurrent_cost"] is DBNull) ? 0 : dr["recurrent_cost"]);
                    model.Don_kind_desc = dr["don_kind_desc"].ToString();
                    model.Don_cur_mth = Convert.ToDecimal(dr["don_cur_mth"]);
                    model.Don_YTD = Convert.ToDecimal((dr["don_ytd"] is DBNull) ? 0 : dr["don_ytd"]);
                    model.Out_comm = Convert.ToDecimal((dr["out_comm"] is DBNull) ? 0 : dr["out_comm"]);
                    result.Add(model);
                }
            }

            return result;
        }
    }
}
