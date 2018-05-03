using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class DonationReserveModel
    {
        public int Id { get; set; }
        public int Detail_id { get; set; }
        public string Hospital { get; set; }
        public string Fund { get; set; }
        public string Section { get; set; }
        public string Analytical { get; set; }
        public string Don_desc { get; set; }
        public string Donor_name { get; set; }
        public int Receive_dt { get; set; }
        public int Don_cat_id { get; set; }
        public string Don_cat { get; set; }
        public decimal Reserve_bal_begin { get; set; }
        public decimal Income { get; set; }
        public decimal Expenditure { get; set; }
        public decimal Gl_bal_begin { get; set; }
        public decimal Gl_bal_end { get; set; }

    }

    public class COA
    {
        public string Hospital { get; set; } = "";
        public string Fund { get; set; } = "";
        public string Section { get; set; } = "";
        public string Analytical { get; set; } = "";
    }
    public class DonationReserveModelCollection
    {
        public List<DonationReserveModel> Collection { get; set; }
        public COA Coa { get; set; }
        public int Record_cnt { get; set; }
        public decimal Reserve_bal_begin_sub { get; set; }
        public decimal Income_sub { get; set; }
        public decimal Expenditure_sub { get; set; }
        public decimal Cal_reserve_bal_sub { get; set; } // Reserve_bal_begin_sub + Income_sub + Expenditure_sub
        public decimal Gl_bal_begin { get; set; }
        public decimal Reserve_bal_var_begin_sub { get; set; }//gl_bal_begin - reserve_bal_begin_sub
        public decimal Gl_bal_end { get; set; }
        public decimal Reserve_bal_var_end_sub { get; set; }//gl_bal_end - reserve_bal_begin_sub - income_sub - expenditure_sub
        public DonationReserveModel Default
        {
            get { return new DonationReserveModel(); }
        }
    }

    public class DonationReserveModelCollectionSet
    {
        public List<DonationReserveModelCollection> Set { get; set; }
        public decimal Total_reserve_bal_begin { get; set; }
        public decimal Total_income { get; set; }
        public decimal Total_expenditure { get; set; }
        public decimal Total_cal_reserve_bal { get; set; }//total_reserve_bal_begin + total_income + total_expenditure
        public decimal Total_gl_bal_begin { get; set; }
        public decimal Total_reserve_bal_var_begin { get; set; }//total_gl_bal_begin - total_reserve_bal_begin
        public decimal Total_gl_bal_end { get; set; }
        public decimal Total_reserve_bal_var_end { get; set; }  //total_gl_bal_end - total_reserve_bal_begin - total_income - total_expenditure
        public DonationReserveModelCollection Default
        {
            get { return new DonationReserveModelCollection(); }
        }
    }
}
