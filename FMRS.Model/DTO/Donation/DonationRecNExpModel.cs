using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class DonationRecNExpModel
    {
        public int Id { get; set; }
        public int OId { get; set; }
        public int Delete_function { get; set; }
        public string Don_case1 { get; set; }
        public string Don_case2 { get; set; }
        public string Don_case3 { get; set; }
        public int Don_supercat { get; set; }
        public int Don_cat { get; set; }
        public string Don_cat_desc { get; set; }
        public int Don_subcat { get; set; }
        public int Don_subsubcat { get; set; }
        public string Don_desc { get; set; }
        public short Don_purpose { get; set; }
        public string Don_purpose_desc { get; set; }
        public string Don_specific { get; set; }
        public string Don_type { get; set; }
        public string Don_type_desc { get; set; }
        public string Don_date { get; set; }
        public string Donor_name { get; set; }
        public string Donor_name_exist { get; set; }
        public short? Eqt_desc { get; set; }
        public string Exist_program { get; set; }
        public string Exist_record { get; set; }
        public int Ischange_ind { get; set; }
        public int Ischange_desc_ind { get; set; }
        public string Maj_don { get; set; }
        public string New_program { get; set; }
        public string Reimb { get; set; }
        public int Input_month { get; set; }
        public string Inst_code { get; set; }
        public string Hospital { get; set; }
        public string Fund { get; set; }
        public string Section { get; set; }
        public string Analytical { get; set; }
        public string Don_inc_exp { get; set; }
        public string ODon_inc_exp { get; set; }
        public int Exist_record_cnt { get; set; }
        public List<DonationRecNExpModel> Link_rec { get; set; }
        public short? Trust { get; set; }
        public string Donor_type { get; set; }
        public short? ODonor_id { get; set; }
        public int? Link_ind { get; set; }
        public int? Link_id { get; set; }
        public string Link { get; set; }
        public DateTime Input_at_val { get; set; }
        public string Recurrent_con { get; set; }
        public decimal? Recurrent_cost { get; set; }
        public string Don_kind_desc { get; set; }
        public string Maj_don1 { get; set; }
        public string Maj_don2 { get; set; }
        public string Maj_don3 { get; set; }
        public string Link_diff_type { get; set; }
        public Decimal BalForward { get; set; }
        public Decimal Don_cur_mth { get; set; }
        public Decimal Don_YTD { get; set; }
        public Decimal Out_comm { get; set; }
        public int General_donation { get; set; }
        public Decimal CM_per_GL { get; set; }
        public Decimal CM_Input { get; set; }
        public List<PreviousRecord> Previous_record { get; set; }
        public List<PreviousRecord> CM_record { get; set; }
        public int Previous_record_cnt { get; set; }
        public int CM_record_cnt { get; set; }
        public Refund Refund { get; set; }
        public string Action { get; set; }
    }

    public class PreviousRecord
    {
        public Decimal Cost { get; set; }
        public string Original_don_date { get; set; }
        public DateTime Original_don_date_date { get; set; }
        public string Input_for { get; set; }
    }

    public class Refund
    {
        public List<DonationRefundDetail> Refund_Detail { get; set; }
        public int Refund_Detail_Cnt { get; set; }
        public string Rec_type { get; set; }
        public Decimal Refund_total_cost { get; set; }
        public string Refund_original_don_date { get; set; }
    }

    public class DonationRefundDetail
    {
        public DateTime Input_for { get; set; }
        public string Refund_date_yyyymmdd { get; set; }
        public string Refund_date_ddmmyy { get; set; }
        public DateTime Original_don_date { get; set; }
        public string Original_don_date_yyyymmdd { get; set; }
        public string Original_don_date_ddmmyy { get; set; }
        public int Cost { get; set; }
    }

    public class DonationRecNExpModelCollection
    {
        public List<DonationRecNExpModel> Collection { get; set; }
        public int Total_cnt { get; set; }
        public Decimal Total_don_cur_mth { get; set; }
        public Decimal Total_don_ytd { get; set; }
        public string Old_don_inc_exp { get; set; }
        public int Old_trust { get; set; }
        public int Old_general_donation { get; set; }

        public DonationRecNExpModel Default
        {
            get { return new DonationRecNExpModel(); }
        }
    }

    public class DonationByYearModel
    {
        public int Don_date { get; set; }
        public Decimal Cost { get; set; }
    }

    public class DonationRecNExpModelCollectionSet
    {
        public List<DonationRecNExpModelCollection> Set { get; set; }
        public Decimal Grand_total_don_ytd { get; set; }
        public Decimal Net_designated_ytd { get; set; }
        public Decimal Net_general_ytd { get; set; }
        public List<DonationByYearModel> Donation_by_year_list { get; set; }
        public Decimal Total_cost { get; set; }
        public DonationRecNExpModelCollection Default
        {
            get { return new DonationRecNExpModelCollection(); }
        }
    }
}
