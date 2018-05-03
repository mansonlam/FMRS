using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class DonationUploadViewModel
    {
        public string Inst_code { get; set; }
        public string Rows_Checked { get; set; }
        public string Remark { get; set; }
        public string Error_list { get; set; } = "";
        public int Valid_only { get; set; } = 0;
        public string Action { get; set; }
        public List<DonationUploadRecord> Record_list { get; set; }
        public string Record_array { get; set; }
        public string Sql { get; set; }
        public int Rec_cnt { get; set; }
        public int Break_line { get; set; }
        public int Valid_rec_cnt { get; set; }
        public int Invalid_rec_cnt { get; set; }

    }

    public class DonationUploadRecord
    {
        public string Hospital { get; set; } = "";
        public int Trust { get; set; } = 0;
        public string Fund { get; set; } = "";
        public string Section { get; set; } = "";
        public string Analytical { get; set; } = "";
        public string Donor_type { get; set; } = "";
        public string Donor_name { get; set; } = "";
        public int Donor_id { get; set; } = 0;
        public string Don_inc_exp { get; set; } = "";
        public int Don_type { get; set; } = 0;
        public string Don_type_c { get; set; } = "";
        public DateTime Don_date { get; set; }
        public string Don_date_c { get; set; } = "";
        public int Don_purpose { get; set; } = 0;
        public string Don_purpose_c { get; set; } = "";
        public int Don_super_cat { get; set; } = 0;
        public string Don_super_cat_desc { get; set; } = "";
        public int Don_cat { get; set; } = 0;
        public string Don_cat_desc { get; set; } = "";
        public int Don_subcat { get; set; } = 0;
        public string Don_subcat_desc { get; set; } = "";
        public int Don_subsubcat { get; set; } = 0;
        public string Don_subsubcat_desc { get; set; } = "";
        public string Don_specific { get; set; } = "";
        public string Maj_don1 { get; set; } = "";
        public string Maj_don2 { get; set; } = "";
        public string Maj_don3 { get; set; } = "";
        public string Reimb { get; set; } = "";
        public string Recurrent_con { get; set; } = "";
        public int Recurrent_cost { get; set; } = 0;
        public string Recurrent_cost_c { get; set; } = "0";
        public string Don_kind_desc { get; set; } = "";
        public decimal Don_cur_mth { get; set; } = 0;
        public string Don_cur_mth_c { get; set; } = "0";
        public int Out_comm { get; set; } = 0;
        public string Out_comm_c { get; set; } = "0";
        public int Outstanding_SP { get; set; } = 0;
        public int Reserve_bal { get; set; } = 0;
        public decimal R_begin { get; set; } = 0;
        public decimal R_end { get; set; } = 0;
        public int Record_error { get; set; } = 0;
        public string Err_msg { get; set; } = "";

    }
}
