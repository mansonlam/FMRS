using System;
using System.Collections.Generic;
using System.Text;
using FMRS.DAL.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using FMRS.Common.Resources;
using FMRS.Model.DTO;
using System.Data;
using System.Globalization;
using System.Web;

namespace FMRS.Service
{
    public interface IDonationRecNExpService
    {
        List<SelectListItem> GetDonTypeList(bool blankRow = true);
        List<SelectListItem> GetDonDonorList(bool blankRow = true);
        List<SelectListItem> GetDonDescList(string hospital, int financial_year, bool blankRow = true);
        List<SelectListItem> GetDonPurposeList(bool blankRow = true);
        List<SelectListItem> GetDonSuperCat(bool blankRow = true);
        List<SelectListItem> GetDonCatBySuperCatId(int supercat_id, bool blankRow = true);
        List<SelectListItem> GetDonSubcatByCatId(int cat_id, bool blankRow = true);
        List<SelectListItem> GetDonSubsubCatBySubCatId(int subcat_id, bool blankRow = true);
        decimal GetCMperGL(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp);
        decimal GetCMInput(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp, int id);
        int GetExistListCount(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int id);
        List<DonationRecNExpModel> GetExistList(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int oid, string odon_inc_exp);
        DonationDetail GetDonationDetailById(int id);
        decimal GetBalForward(int id, int financial_year);
        int GetBalAlert(string hospital, string fund_code, string section_code, string analytical_code, string value_date);
        decimal GetCostA(string inst_code, int show_year, int financial_year, string in_donor_name);
        decimal GetCostB(string inst_code, int show_year, int financial_year, string in_donor_name);
        decimal GetRefund(int id, int financial_year);
        List<DonationRefundDetail> GetDonationRefundDetailList(int id, int financial_year, string rec_type);
        int GetDonationRefundDetailListCount(int id, int financial_year, string rec_type);
        Refund GetRefund(int id, int financial_year, string rec_type);
        void UpdateDonationRefund(int id, int financial_year, string rec_type, int rec_cnt, string login_id, List<DonationRefundDetail> refund_detail);
        List<DonationRecNExpModel> GetDonationList(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated, out int total_cnt, out Decimal total_don_cur_mth, out Decimal total_don_ytd);
        DonationRecNExpModelCollection GetDonationCollection(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated, ref Decimal grand_total_don_ytd, ref Decimal net_designated_ytd, ref Decimal net_general_ytd);
        DonationRecNExpModelCollectionSet GetDonationCollectionSet(string inst_code, int show_year, int financial_year, string value_date, string in_donor_name);
        List<DonationByYearModel> GetDonationByYear(string inst_code, string in_donor_name, out Decimal total_cost);
        void DonationLinkRecord(int id, int oid);
        List<PreviousRecord> GetPreviousRecord(int id, int financial_year, bool curr_mth, string value_date, string cal_date);
        int GetPreviousRecordCount(int id, int financial_year, bool curr_mth, string value_date, string cal_date);
        void DeleteDonation(string login_id, int id);
        void UpdateDonationLinkID(int oid, int link_id);
        void UpdateDonationReserve(DateTime value_date, string Inst_code, int id, string login_id);
        void DeleteDonationHistory(int id, int financial_year);
        void UpdateDonationDetail(DonationRecNExpModel model, string login_id);
        void InsertNewDonation(DonationRecNExpModel model, string login_id);
        //TEST Create
        //bool CreateDonDonor(string don_kind_desc);
    }
    public class DonationRecNExpService : IDonationRecNExpService
    {
        private IDonTypeRepository DonTypeRepository;
        private IDonDonorRepository DonDonorRepository;
        private IDonPurposeRepository DonPurposeRepository;
        private IDonationDetailRepository DonationDetailRepository;
        private IDonationBalanceRepository DonationBalanceRepository;
        private IDonSupercatRepository DonSupercatRepository;
        private IDonCatRepository DonCatRepository;
        private IDonSubcatRepository DonSubcatRepository;
        private IDonSubsubcatRepository DonSubsubcatRepository;
        private IDonationHistoryRepository DonationHistoryRepository;

        public DonationRecNExpService(IDonTypeRepository _donTypeRepository, IDonDonorRepository _donDonorRepository,
            IDonPurposeRepository _donPurposeRepository, IDonationDetailRepository _donationDetailRepository,
            IDonationBalanceRepository _donationBalanceRepository, IDonSupercatRepository _donSupercatRepository,
            IDonCatRepository _donCatRepository, IDonSubcatRepository _donSubcatRepository,
            IDonSubsubcatRepository _donSubsubcatRepository, IDonationHistoryRepository _donationHistoryRepository
             )
        {
            DonTypeRepository = _donTypeRepository;
            DonDonorRepository = _donDonorRepository;
            DonPurposeRepository = _donPurposeRepository;
            DonationDetailRepository = _donationDetailRepository;
            DonationBalanceRepository = _donationBalanceRepository;
            DonSupercatRepository = _donSupercatRepository;
            DonCatRepository = _donCatRepository;
            DonSubcatRepository = _donSubcatRepository;
            DonSubsubcatRepository = _donSubsubcatRepository;
            DonationHistoryRepository = _donationHistoryRepository;
        }
        #region Drop Down List Data 
        public List<SelectListItem> GetDonTypeList(bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donType = new List<DonType>(DonTypeRepository.GetAllDonType());
            if (donType != null)
            {
                result = donType.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetDonDonorList(bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donDonor = DonDonorRepository.GetAllDonDonor();
            if (donDonor.Count() > 0)
            {
                result = donDonor.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = " --- " + Resource.PleaseSelect + " --- "}); }
            return result;
        }

        public List<SelectListItem> GetDonDescList(string hospital, int financial_year, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donDesc = DonationDetailRepository.GetDonDesc(hospital, financial_year); 
            if (donDesc.Count() > 0)
            {
                result = donDesc.Select(s => new SelectListItem() { Value = HttpUtility.HtmlEncode(s).Replace("&#39;", "\'"), Text = s }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetDonPurposeList(bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donPurpose = DonPurposeRepository.GetAllDonPurpose();
            if (donPurpose.Count() > 0)
            {
                result = donPurpose.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetDonSuperCat(bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donSuperCat = DonSupercatRepository.GetAllDonSupercat();
            if (donSuperCat.Count() > 0)
            {
                result = donSuperCat.Select(s => new SelectListItem() { Value = s.SupercatId.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = " --- " + Resource.PleaseSelect + " --- " }); }
            return result;
        }

        public List<SelectListItem> GetDonCatBySuperCatId(int supercat_id, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donCat = DonCatRepository.GetDonCatBySuperCatId(supercat_id);
            if (donCat.Count() > 0)
            {
                result = donCat.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetDonSubcatByCatId(int cat_id, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donSubCat = DonSubcatRepository.GetDonSubcatByCatId(cat_id);
            if (donSubCat.Count() > 0)
            {
                result = donSubCat.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }

        public List<SelectListItem> GetDonSubsubCatBySubCatId(int subcat_id, bool blankRow)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            var donSubsubCat = DonSubsubcatRepository.GetDonSubsubcatBySubCatId(subcat_id);
            if (donSubsubCat.Count() > 0)
            {
                result = donSubsubCat.Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Description }).ToList();
            }
            if (blankRow) { result.Insert(0, new SelectListItem() { Value = "", Text = "" }); }
            return result;
        }
        #endregion

        public decimal GetCMperGL(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp)
        {
            decimal result = new decimal();
            var donBalance = DonationBalanceRepository.GetDonationBalance(hospital, fund_code, section_code, analytical_code, value_date);
            if (donBalance.Count() > 0)
            {
                if (don_inc_exp == "E")
                { result = donBalance.First().Expenditure; }
                else if (don_inc_exp == "I")
                { result = donBalance.First().Income; }
            }
            return result;
        }

        public decimal GetCMInput(string hospital, string fund_code, string section_code, string analytical_code, string value_date, string don_inc_exp, int id)
        {
            return DonationDetailRepository.GetCMInput(hospital, fund_code, section_code, analytical_code, value_date, don_inc_exp, id);
        }

        public int GetExistListCount(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int id)
        {
            return DonationDetailRepository.GetExistListCount(inst_code, fund_code, analytical_code, section_code, financial_year, financial_year2, value_date, id);
        }
        public List<DonationRecNExpModel> GetExistList(string inst_code, string fund_code, string analytical_code, string section_code, int financial_year, int financial_year2, string value_date, int oid, string odon_inc_exp)
        {
            List<DonationRecNExpModel> result = new List<DonationRecNExpModel>();
            var ds  = DonationDetailRepository.GetExistList(inst_code, fund_code, analytical_code, section_code, financial_year, financial_year2, value_date, oid);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationRecNExpModel model = new DonationRecNExpModel();
                    model.Id = Convert.ToInt32(dr["id"]);
                    model.OId = oid;
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
                    model.ODon_inc_exp = odon_inc_exp;
                    result.Add(model);
                }
            }
            return result;
        }
        public DonationDetail GetDonationDetailById(int id)
        {
            return DonationDetailRepository.GetDonationDetailById(id);
        }

        public decimal GetBalForward(int id, int financial_year)
        {
            return DonationHistoryRepository.GetBalForward(id, financial_year);
        }

        public int GetBalAlert(string hospital, string fund_code, string section_code, string analytical_code, string value_date)
        {
            return DonationBalanceRepository.GetBalAlert(hospital, fund_code, section_code, analytical_code, value_date);
        }

        public decimal GetCostA(string inst_code, int show_year, int financial_year, string in_donor_name)
        {
            return DonationHistoryRepository.GetCostA(inst_code, show_year, financial_year, in_donor_name);
        }

        public decimal GetCostB(string inst_code, int show_year, int financial_year, string in_donor_name)
        {
            return DonationHistoryRepository.GetCostB(inst_code, show_year, financial_year, in_donor_name);
        }

        public decimal GetRefund(int id, int financial_year)
        {
            return DonationHistoryRepository.GetRefund(id, financial_year);
        }

        public List<DonationRefundDetail> GetDonationRefundDetailList(int id, int financial_year, string rec_type)
        {
            List<DonationRefundDetail> result = new List<DonationRefundDetail>();
            var ds = DonationHistoryRepository.GetRefundDetail(id, financial_year, rec_type);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationRefundDetail item = new DonationRefundDetail();
                    item.Cost = Convert.ToInt32(Convert.ToDecimal((dr["cost"] is DBNull) ? 0 : dr["cost"]));
                    item.Input_for = DateTime.Parse(dr["input_for"].ToString());
                    item.Refund_date_yyyymmdd = item.Input_for.ToString("yyyyMMdd");//dr["refund_date_yyyymmdd"].ToString();
                    item.Refund_date_ddmmyy = item.Input_for.ToString("dd/MM/yyyy");//dr["refund_date_ddmmyy"].ToString();
                    item.Original_don_date = DateTime.Parse(dr["original_don_date"].ToString());
                    item.Original_don_date_yyyymmdd = item.Original_don_date.ToString("yyyyMMdd");//dr["original_don_date_yyyymmdd"].ToString();
                    item.Original_don_date_ddmmyy = item.Original_don_date.ToString("dd/MM/yyyy");//dr["original_don_date_ddmmyy"].ToString();
                    if (item.Original_don_date_ddmmyy == item.Refund_date_ddmmyy)
                        item.Refund_date_ddmmyy = "";
                    result.Add(item);
                }
            }
            return result;
        }

        public int GetDonationRefundDetailListCount(int id, int financial_year, string rec_type)
        {
            return DonationHistoryRepository.GetRefundDetail(id, financial_year, rec_type).Tables[0].Rows.Count;
        }

        public Refund GetRefund(int id, int financial_year, string rec_type)
        {
            Refund model = new Refund();
            model.Refund_Detail = GetDonationRefundDetailList(id, financial_year, rec_type);
            model.Refund_Detail_Cnt = GetDonationRefundDetailListCount(id, financial_year, rec_type);
            return model;
        }

        public void UpdateDonationRefund(int id, int financial_year, string rec_type, int rec_cnt, string login_id, List<DonationRefundDetail> refund_detail)
        {
            DonationHistoryRepository.UpdateDonationRefund(id, financial_year, rec_type, rec_cnt, login_id, refund_detail);
        }

        public List<DonationRecNExpModel> GetDonationList(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated, out int total_cnt, out Decimal total_don_cur_mth, out Decimal total_don_ytd)
        {
            total_cnt = 0;
            total_don_cur_mth = 0;
            total_don_ytd = 0;
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
                   model.Eqt_desc = Convert.ToInt16((dr["eqt_desc"] is DBNull)?0: dr["eqt_desc"]);
                   model.Recurrent_con = dr["recurrent_con"].ToString();
                   model.Recurrent_cost = Convert.ToDecimal((dr["recurrent_cost"] is DBNull) ? 0 : dr["recurrent_cost"]);
                   model.Don_kind_desc = dr["don_kind_desc"].ToString();
                   model.Don_cur_mth = Convert.ToDecimal(dr["don_cur_mth"]);
                   model.Don_YTD = Convert.ToDecimal((dr["don_ytd"] is DBNull) ? 0 : dr["don_ytd"]);
                   model.Out_comm = Convert.ToDecimal((dr["out_comm"] is DBNull) ? 0 : dr["out_comm"]); 
                   model.General_donation = Convert.ToInt32((dr["general_donation"] is DBNull) ? 0 : dr["general_donation"]);
                    if (model.Don_cur_mth != 0 || model.Don_YTD != 0 || in_donor_name != "")
                   {
                       result.Add(model);
                       total_cnt = total_cnt + 1;
                       total_don_cur_mth = total_don_cur_mth + model.Don_cur_mth;
                       total_don_ytd = total_don_ytd + model.Don_YTD;
                   }
               }
            
            }

            return result;
        }

        public DonationRecNExpModelCollection GetDonationCollection(string inst_code, int show_year, int financial_year, string value_date, int cur_year_only, string in_donor_name, string inc_exp, string designated, ref Decimal grand_total_don_ytd, ref Decimal net_designated_ytd, ref Decimal net_general_ytd)
        {
            DonationRecNExpModelCollection model = new DonationRecNExpModelCollection();
            var total_cnt = 0;
            Decimal total_don_cur_mth = 0;
            Decimal total_don_ytd = 0;
            model.Collection = GetDonationList(inst_code, show_year, financial_year, value_date, cur_year_only, in_donor_name, inc_exp, designated, out total_cnt, out total_don_cur_mth, out total_don_ytd);
            model.Total_cnt = total_cnt;
            model.Total_don_cur_mth = total_don_cur_mth;
            model.Total_don_ytd = total_don_ytd;
            grand_total_don_ytd = grand_total_don_ytd + total_don_ytd;
            if (designated == "Y")
                net_designated_ytd = net_designated_ytd + total_don_ytd;
            else
                net_general_ytd = net_general_ytd + total_don_ytd;
            model.Old_don_inc_exp = "";
            model.Old_trust = -1;
            model.Old_general_donation = -1;
            return model;
        }

        public DonationRecNExpModelCollectionSet GetDonationCollectionSet(string inst_code, int show_year, int financial_year, string value_date, string in_donor_name)
        {
            DonationRecNExpModelCollectionSet model = new DonationRecNExpModelCollectionSet();
            Decimal grand_total_don_ytd = 0;
            Decimal net_designated_ytd = 0;
            Decimal net_general_ytd = 0;
            model.Set = new List<DonationRecNExpModelCollection>{
                GetDonationCollection(inst_code, show_year, financial_year, value_date, 0, in_donor_name, "I", "Y", ref grand_total_don_ytd, ref net_designated_ytd, ref net_general_ytd),
                GetDonationCollection(inst_code, show_year, financial_year, value_date, 0, in_donor_name, "E", "Y", ref grand_total_don_ytd, ref net_designated_ytd, ref net_general_ytd),
                GetDonationCollection(inst_code, show_year, financial_year, value_date, 0, in_donor_name, "I", "N", ref grand_total_don_ytd, ref net_designated_ytd, ref net_general_ytd),
                GetDonationCollection(inst_code, show_year, financial_year, value_date, 0, in_donor_name, "E", "N", ref grand_total_don_ytd, ref net_designated_ytd, ref net_general_ytd)
            };
            model.Grand_total_don_ytd = grand_total_don_ytd;
            model.Net_designated_ytd = net_designated_ytd;
            model.Net_general_ytd = net_general_ytd;
            return model;
        }

        public List<DonationByYearModel> GetDonationByYear(string inst_code, string in_donor_name, out Decimal total_cost)
        {
            total_cost = 0;
            List<DonationByYearModel> result = new List<DonationByYearModel>();
            var ds = DonationHistoryRepository.GetDoantionByYear(inst_code, in_donor_name);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DonationByYearModel item = new DonationByYearModel();
                    item.Don_date = Convert.ToInt32(dr["temp_don_date"]);
                    item.Cost = Convert.ToDecimal((dr["cost"] is DBNull) ? 0 : dr["cost"]);
                    
                    total_cost = total_cost + item.Cost;
                    result.Add(item);
                }
            }
            return result;

        }

        public void DonationLinkRecord(int id, int oid)
        {
            DonationDetailRepository.DonationLinkRecord(id, oid);
        }

        public List<PreviousRecord> GetPreviousRecord(int id, int financial_year, bool curr_mth, string value_date, string cal_date)
        {
            List<PreviousRecord> cm_result = new List<PreviousRecord>();
            List<PreviousRecord> pre_result = new List<PreviousRecord>();
            var ds = DonationHistoryRepository.GetDonationHistoryByIdFinYr(id, financial_year);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    PreviousRecord item = new PreviousRecord();
                    item.Cost = Convert.ToDecimal((dr["cost"] is DBNull) ? 0 : dr["cost"]);
                    var temp_input_for = dr["input_for"].ToString();
                    var temp_org_don_date = dr["original_don_date"].ToString();
                    item.Original_don_date_date = DateTime.ParseExact(temp_org_don_date,"yyyymmdd",CultureInfo.InvariantCulture);
                    item.Original_don_date = item.Original_don_date_date.ToString("dd/mm/yyyy");
                    //item.Original_don_date = temp_org_don_date.Substring(6, 2) + '/' + temp_org_don_date.Substring(4, 2) + '/' + temp_org_don_date.Substring(2, 2);
                    if (temp_org_don_date != temp_input_for)
                    { item.Input_for = temp_input_for.Substring(6, 2) + '/' + temp_input_for.Substring(4, 2) + '/' + temp_input_for.Substring(2, 2); }
                    if (temp_input_for.Substring(0,6) == value_date.Substring(0, 6) || temp_input_for.Substring(0, 6) == cal_date.Substring(0, 6))
                        cm_result.Add(item);
                    else
                        pre_result.Add(item);
                }
            }
            if (curr_mth)
                return cm_result;
            else
                return pre_result;
        }

        public int GetPreviousRecordCount(int id, int financial_year, bool curr_mth, string value_date, string cal_date)
        {
            return GetPreviousRecord(id, financial_year, curr_mth, value_date, cal_date).Count;
        }

        public void DeleteDonation(string login_id, int id)
        {
            DonationHistoryRepository.DeleteDonation(login_id, id);
        }

        public void UpdateDonationLinkID(int oid, int link_id)
        {
            DonationDetailRepository.UpdateDonationLinkID(oid, link_id);
        }

        public void UpdateDonationReserve(DateTime value_date, string inst_code, int id, string login_id)
        {
            DonationDetailRepository.UpdateDonationReserve(value_date, inst_code, id, login_id);
        }
        public void DeleteDonationHistory(int id, int financial_year)
        {
            DonationHistoryRepository.DeleteDonationHistory(id, financial_year);
        }

        public void UpdateDonationDetail(DonationRecNExpModel model, string login_id)
        {
            DonationDetailRepository.UpdateDonationDetail(model, login_id);
            DonationDetailRepository.UpdateDonationLinkDetail(model.Id);
            DonationHistoryRepository.InsertDonationHistory(model.Id, model.CM_record, model.Previous_record, login_id, model.Don_type);
        }

        public void InsertNewDonation(DonationRecNExpModel model, string login_id)
        {
            DonationDetailRepository.InsertDonationDetail(model, login_id);
            DonationHistoryRepository.InsertDonationHistoryForNewDonation(model.CM_record, model.Previous_record, login_id);
        }

        //TEST Create
        //public bool CreateDonDonor(string don_kind_desc)
        //{
        //    return DonDonorRepository.CreateDonor(don_kind_desc);

        //}
    }
}
