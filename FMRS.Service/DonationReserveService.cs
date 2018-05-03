using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Common.Resources;
using FMRS.Model.DTO;
using System.Data;
using System.Globalization;
using FMRS.DAL.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FMRS.Service
{
    public interface IDonationReserveService
    {
        DonationReserveModelCollectionSet GetDonationReserveModelCollectionSet(string value_date, string inst_code);
        List<DonationReserveModelCollection> GetDonationReserveModelCollection(string value_date, string inst_code, out decimal total_reserve_bal_begin, out decimal total_income, out decimal total_expenditure, out decimal total_gl_bal_begin, out decimal total_gl_bal_end);
        List<DonationReserveModel> GetGetDonationReserveByCOA(string value_date, string inst_code, string hospital, string fund, string section, string analytical, out decimal reserve_bal_begin_sub, out decimal income_sub, out decimal expenditure_sub, out decimal gl_bal_begin, out decimal gl_bal_end, out int record_cnt);
        List<DonationReserveModel> GetDonationReserveSummary(string value_date, string inst_code);
        List<COA> GetDonationReserveCOAList(string value_date, string inst_code);
        List<DonationRecNExpModel> GetExistList2(string don_inc_exp, string hospital, string fund_code, string analytical_code, string section_code, int financial_year, int curr_financial_year, DateTime current_date, int detail_id, string don_desc, string donor_name, short don_cat, out int record_cnt);

    }
    public class DonationReserveService : IDonationReserveService
    {
        private IDonationReserveRepository DonationReserveRepository;
        private IDonationDetailRepository DonationDetailRepository;
        public DonationReserveService(IDonationReserveRepository _donationReserveRepository, IDonationDetailRepository _donationDetailRepository)
        {
            DonationReserveRepository = _donationReserveRepository;
            DonationDetailRepository = _donationDetailRepository;
        }
        public DonationReserveModelCollectionSet GetDonationReserveModelCollectionSet(string value_date, string inst_code)
        {
            decimal total_reserve_bal_begin = 0;
            decimal total_income = 0;
            decimal total_expenditure = 0;
            decimal total_gl_bal_begin = 0;
            decimal total_gl_bal_end = 0;
            DonationReserveModelCollectionSet model = new DonationReserveModelCollectionSet();
            model.Set = GetDonationReserveModelCollection(value_date, inst_code, out total_reserve_bal_begin, out total_income, out total_expenditure,
                out total_gl_bal_begin, out total_gl_bal_end);
            model.Total_reserve_bal_begin = total_reserve_bal_begin;
            model.Total_income = total_income;
            model.Total_expenditure = total_expenditure;
            model.Total_cal_reserve_bal = total_reserve_bal_begin + total_income + total_expenditure;
            model.Total_gl_bal_begin = total_gl_bal_begin;
            model.Total_reserve_bal_var_begin = total_gl_bal_begin - total_reserve_bal_begin;
            model.Total_gl_bal_end = total_gl_bal_end;
            model.Total_reserve_bal_var_end = total_gl_bal_end - total_reserve_bal_begin - total_income - total_expenditure;
            return model;
        }

        public List<DonationReserveModelCollection> GetDonationReserveModelCollection(string value_date, string inst_code,
             out decimal total_reserve_bal_begin, out decimal total_income, out decimal total_expenditure, out decimal total_gl_bal_begin, out decimal total_gl_bal_end)
        {
            total_reserve_bal_begin = 0;
            total_income = 0;
            total_expenditure = 0;
            total_gl_bal_begin = 0;
            total_gl_bal_end = 0;
            List<DonationReserveModelCollection> result = new List<DonationReserveModelCollection>();
            List<COA> coa = GetDonationReserveCOAList(value_date, inst_code);
            foreach (var r in coa)
            {
                decimal reserve_bal_begin_sub = 0;
                decimal income_sub = 0;
                decimal expenditure_sub = 0;
                decimal gl_bal_begin = 0;
                decimal gl_bal_end = 0;
                int record_cnt = 0;
                DonationReserveModelCollection model = new DonationReserveModelCollection();
                model.Coa = r;
                model.Collection = GetGetDonationReserveByCOA(value_date, inst_code, r.Hospital, r.Fund, r.Section, r.Analytical, 
                    out reserve_bal_begin_sub, out income_sub, out expenditure_sub, out gl_bal_begin, out gl_bal_end, out record_cnt);
                model.Reserve_bal_begin_sub = reserve_bal_begin_sub;
                model.Income_sub = income_sub;
                model.Expenditure_sub = expenditure_sub;
                model.Cal_reserve_bal_sub = reserve_bal_begin_sub + income_sub + expenditure_sub;
                model.Gl_bal_begin = gl_bal_begin;
                model.Reserve_bal_var_begin_sub = gl_bal_begin - reserve_bal_begin_sub;
                model.Gl_bal_end = gl_bal_end;
                model.Reserve_bal_var_end_sub = gl_bal_end - reserve_bal_begin_sub - income_sub - expenditure_sub;
                model.Record_cnt = record_cnt;
                result.Add(model);
                total_reserve_bal_begin = total_reserve_bal_begin + reserve_bal_begin_sub;
                total_income = total_income + income_sub;
                total_expenditure = total_expenditure + expenditure_sub;
                total_gl_bal_begin = total_gl_bal_begin + gl_bal_begin;
                total_gl_bal_end = total_gl_bal_end + gl_bal_end;
            }
            return result;
        }

        //Get list of donation reserve base on COA
        //return sub total
        public List<DonationReserveModel> GetGetDonationReserveByCOA(string value_date, string inst_code, string hospital, string fund, string section, string analytical,
             out decimal reserve_bal_begin_sub, out decimal income_sub, out decimal expenditure_sub, out decimal gl_bal_begin, out decimal gl_bal_end, out int record_cnt)
        {
            reserve_bal_begin_sub = 0;
            income_sub = 0;
            expenditure_sub = 0;
            gl_bal_begin = 0;
            gl_bal_end = 0;
            record_cnt = 0;
            List<DonationReserveModel> result = new List<DonationReserveModel>();
            var ds = GetDonationReserveSummary(value_date, inst_code);
            foreach (var dr in ds)
            {
                if (dr.Hospital == hospital
                    && dr.Fund == fund
                    && dr.Section == section
                    && dr.Analytical == analytical)
                { 
                    result.Add(dr);
                    gl_bal_begin = dr.Gl_bal_begin;
                    gl_bal_end = dr.Gl_bal_end;
                    reserve_bal_begin_sub = reserve_bal_begin_sub + dr.Reserve_bal_begin;
                    income_sub = income_sub + dr.Income;
                    expenditure_sub = expenditure_sub + dr.Expenditure;
                    record_cnt = record_cnt + 1;
                }
            }
            return result;
        }

        //Get full list of donation reserve
        public List<DonationReserveModel> GetDonationReserveSummary(string value_date, string inst_code)
        {
            List<DonationReserveModel> result = new List<DonationReserveModel>();
            var ds = DonationReserveRepository.GetDonationReserveSummary(value_date, inst_code);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationReserveModel model = new DonationReserveModel();
                    model.Id = Convert.ToInt32(dr["id"]);
                    model.Detail_id = Convert.ToInt32((dr["detail_id"] is DBNull) ? 0 : dr["detail_id"]);
                    model.Hospital = dr["hospital"].ToString();
                    model.Fund = dr["fund"].ToString();
                    model.Section = dr["section"].ToString();
                    model.Analytical = dr["analytical"].ToString();
                    model.Don_desc = dr["don_desc"].ToString();
                    model.Donor_name = dr["donor_name"].ToString();
                    model.Receive_dt = Convert.ToInt32(dr["receive_dt"]);
                    model.Don_cat_id = Convert.ToInt32(dr["doncat_id"]);
                    model.Don_cat = dr["doncat_desc"].ToString();
                    model.Reserve_bal_begin = Convert.ToDecimal(dr["reserve_bal_begin"]);
                    model.Income = Convert.ToDecimal(dr["income"]);
                    model.Expenditure = Convert.ToDecimal(dr["expenditure"]);
                    model.Gl_bal_begin = Convert.ToDecimal(dr["gl_bal_begin"]);
                    model.Gl_bal_end = Convert.ToDecimal(dr["gl_bal_end"]);
                    result.Add(model);
                }
            }
            return result;
        }

        public List<COA> GetDonationReserveCOAList(string value_date, string inst_code)
        {
            List<COA> result = new List<COA>();
            var ds = GetDonationReserveSummary(value_date, inst_code);
            var last_coa = new COA();
            foreach (var dr in ds)
            {
                if (dr.Hospital != last_coa.Hospital
                    || dr.Fund != last_coa.Fund
                    || dr.Section != last_coa.Section
                    || dr.Analytical != last_coa.Analytical)
                {
                    COA new_coa = new COA();
                    new_coa.Hospital = dr.Hospital;
                    new_coa.Fund = dr.Fund;
                    new_coa.Section = dr.Section;
                    new_coa.Analytical = dr.Analytical;
                    result.Add(new_coa);
                    last_coa.Hospital = dr.Hospital;
                    last_coa.Fund = dr.Fund;
                    last_coa.Section = dr.Section;
                    last_coa.Analytical = dr.Analytical;
                }
            }
            return result;
        }

        public List<DonationRecNExpModel> GetExistList2(string don_inc_exp, string hospital, string fund_code, string analytical_code, string section_code, int financial_year, int curr_financial_year, DateTime current_date, int detail_id, string don_desc, string donor_name, short don_cat, out int record_cnt)
        {
            record_cnt = 0;
            List<DonationRecNExpModel> result = new List<DonationRecNExpModel>();
            var ds = DonationDetailRepository.GetExistList2(hospital, fund_code, analytical_code, section_code, financial_year, curr_financial_year, current_date, detail_id, don_desc, donor_name, don_cat);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationRecNExpModel model = new DonationRecNExpModel();
                    model.Id = Convert.ToInt32(dr["id"]);
                    model.Hospital = dr["hospital"].ToString();
                    model.Fund = dr["fund"].ToString();
                    model.Section = dr["section"].ToString();
                    model.Analytical = dr["analytical"].ToString();
                    model.Don_kind_desc = dr["don_kind_desc"].ToString();
                    model.Donor_name = dr["donor_name"].ToString();
                    model.Don_type_desc = dr["don_type"].ToString();
                    model.Don_purpose_desc = dr["don_purpose"].ToString();
                    model.Don_cat_desc = dr["don_cat"].ToString();
                    model.Don_cur_mth = Convert.ToDecimal(dr["cm_amt"]);
                    model.Don_YTD = Convert.ToDecimal(dr["ytd"]);
                    model.Don_inc_exp = dr["don_inc_exp"].ToString();
                    if (model.Don_inc_exp == don_inc_exp)
                    {
                        result.Add(model);
                        record_cnt = record_cnt + 1;
                    }
                }
            }
            return result;
        }
    }

    
}
