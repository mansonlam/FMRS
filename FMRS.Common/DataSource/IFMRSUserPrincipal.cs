using System;

namespace FMRS.Common.DataSource
{
    /// <summary>
    /// FMRS User info
    /// </summary>
    public interface IFMRSUserPrincipal
    {
        int UserId { get; set; }
        string Username { get; set; }
        string LoginId { get; set; }
        string Domain { get; set; }
        string Cluster { get; set; }
        string InstCode { get; set; }
        string FMRS_SYS_D { get; set; }
        string FMRS_SYS_M { get; set; }
        string FMRS_SYS_Y { get; set; }
        string UserGroup_Y { get; set; }
        string UserGroup_M { get; set; }
        string UserGroup_D { get; set; }
        string Current_Year { get; set; }
        string Current_Month { get; set; }
        string Current_Day { get; set; }
        string Current_Date { get; set; }
        string Value_Date { get; set; }
        string Value_Date2 { get; set; }
        string Gl_End_Day { get; set; }
        string DonationPeriod { get; set; }
        //User Privilege
        //string Privilege_Report { get; set; }
        string Privilege_Report_D { get; set; }
        string Privilege_Report_M { get; set; }
        string Privilege_Report_Y { get; set; }
        //string Privilege_Admin { get; set; }
        string Privilege_Admin_D { get; set; }
        string Privilege_Admin_M { get; set; }
        string Privilege_Admin_Y { get; set; }
        //string Privilege_Closing_Report { get; set; }
        string Privilege_Closing_Report_D { get; set; }
        string Privilege_Closing_Report_M { get; set; }
        string Privilege_Closing_Report_Y { get; set; }
        //string Privilege_Non_Pjt_Report { get; set; }
        string Privilege_Non_Pjt_Report_D { get; set; }
        string Privilege_Non_Pjt_Report_M { get; set; }
        string Privilege_Non_Pjt_Report_Y { get; set; }
        //string Privilege_Asoi_Rpt { get; set; }
        string Privilege_Asoi_Rpt_D { get; set; }
        string Privilege_Asoi_Rpt_M { get; set; }
        string Privilege_Asoi_Rpt_Y { get; set; }
        //User Privilege Financial Closing
        string Privilege_Asoi_Input { get; set; }
        //User Privilege Project Management
        string Privilege_Cbv_Report { get; set; }
        string Privilege_Cbv_Funding { get; set; }
        string Privilege_Cwrf_Funding { get; set; }
        string Privilege_Cwrf { get; set; }
        string Privilege_Cwrf_Submenu { get; set; }
        string Privilege_Cluster_Admin { get; set; }
        //User Privilege Donation
        string Privilege_Donation { get; set; }
        //Unknow if useful
        string Privilege_Pe_Adjust { get; set; }
        string Privilege_Re_Budget { get; set; }
        string Privilege_Far_Access { get; set; }

        string Privilege_Proj_for_Bud { get; set; }
        string Privilege_Rebudget_Bud { get; set; }
        //string Don_Haho { get; set; }
        //string Don_Hosp { get; set; }
        //string Don_Bss { get; set; }    
        string Fv_Domain_Id { get; set; }
        string Fv_User_Admin { get; set; }
        string p13 { get; set; }
        int Financial_Year { get; set; }
        string p13_Actual { get; set; }
        string Other_Wk_Agent { get; set; }
        string Report_Date { get; set; }
        string Report_Period { get; set; }
        string Year_End { get; set; }
        string Financial_Year_Ind { get; set; }
        string Period_Ended { get; set; }
        string Period_End_Date { get; set; }
        string Actual_Cnt { get; set; }
        string Donation_Period { get; set; }
        string Input_Period { get; set; }
        string Adj_Period { get; set; }
        string Far_Period { get; set; }
        string Schedule_Cbv_Comment_Period { get; set; }
        string Interim_Closing_Period { get; set; }
        string Year_End_Closing_Period { get; set; }
        string F52_Interim_Closing_Period { get; set; }
        string F52_Year_End_Closing_Period { get; set; }
        string Other_Gov_Agent_8100mx_Period { get; set; }
        string Re_Budget_Period { get; set; }
        string Pe_By_Spec_Ind { get; set; }
    }
    public class FMRSUserPrincipal : IFMRSUserPrincipal
    {
       public int UserId { get; set; } // CKC004
        public string Username { get; set; } // Chan Tai Man
        public string LoginId { get; set; } // CORP\CKC004
        public string Domain { get; set; } // CORP
        public string Cluster { get; set; }
        public string InstCode { get; set; }
        public string FMRS_SYS_D { get; set; }
        public string FMRS_SYS_M { get; set; }
        public string FMRS_SYS_Y { get; set; }
        public string UserGroup_Y { get; set; }
        public string UserGroup_M { get; set; }
        public string UserGroup_D { get; set; }
        public string Current_Year { get; set; }
        public string Current_Month { get; set; }
        public string Current_Day { get; set; }
        public string Current_Date { get; set; }
        public string Value_Date { get; set; }
        public string Value_Date2 { get; set; }
        public string Gl_End_Day { get; set; }
        public string DonationPeriod { get; set; }
        //User Privilege
        //public string Privilege_Report { get; set; }
        public string Privilege_Report_D { get; set; }
        public string Privilege_Report_M { get; set; }
        public string Privilege_Report_Y { get; set; }
        //public string Privilege_Admin { get; set; }
        public string Privilege_Admin_D { get; set; }
        public string Privilege_Admin_M { get; set; }
        public string Privilege_Admin_Y { get; set; }
        //public string Privilege_Closing_Report { get; set; }
        public string Privilege_Closing_Report_D { get; set; }
        public string Privilege_Closing_Report_M { get; set; }
        public string Privilege_Closing_Report_Y { get; set; }
        //public string Privilege_Non_Pjt_Report { get; set; }
        public string Privilege_Non_Pjt_Report_D { get; set; }
        public string Privilege_Non_Pjt_Report_M { get; set; }
        public string Privilege_Non_Pjt_Report_Y { get; set; }
        //public string Privilege_Asoi_Rpt { get; set; }
        public string Privilege_Asoi_Rpt_D { get; set; }
        public string Privilege_Asoi_Rpt_M { get; set; }
        public string Privilege_Asoi_Rpt_Y { get; set; }
        //User Privilege Financial Closing
        public string Privilege_Asoi_Input { get; set; }
        //User Privilege Project Management
        public string Privilege_Cbv_Report { get; set; }
        public string Privilege_Cbv_Funding { get; set; }
        public string Privilege_Cwrf_Funding { get; set; }
        public string Privilege_Cwrf { get; set; }
        public string Privilege_Cwrf_Submenu { get; set; }
        public string Privilege_Cluster_Admin { get; set; }
        //User Privilege Donation
        public string Privilege_Donation { get; set; }
        //Unknow if useful
        public string Privilege_Pe_Adjust { get; set; }
        public string Privilege_Re_Budget { get; set; }
        public string Privilege_Far_Access { get; set; }

        public string Privilege_Proj_for_Bud { get; set; }
        public string Privilege_Rebudget_Bud { get; set; }

        public string Fv_Domain_Id { get; set; }
        public string Fv_User_Admin { get; set; }
        public string p13 { get; set; }
        public int Financial_Year { get; set; }
        public string p13_Actual { get; set; }
        public string Other_Wk_Agent { get; set; }
        public string Report_Date { get; set; }
        public string Report_Period { get; set; }
        public string Year_End { get; set; }
        public string Financial_Year_Ind { get; set; }
        public string Period_Ended { get; set; }
        public string Period_End_Date { get; set; }
        public string Actual_Cnt { get; set; }
        public string Donation_Period { get; set; }
        public string Input_Period { get; set; }
        public string Adj_Period { get; set; }
        public string Far_Period { get; set; }
        public string Schedule_Cbv_Comment_Period { get; set; }
        public string Interim_Closing_Period { get; set; }
        public string Year_End_Closing_Period { get; set; }
        public string F52_Interim_Closing_Period { get; set; }
        public string F52_Year_End_Closing_Period { get; set; }
        public string Other_Gov_Agent_8100mx_Period { get; set; }
        public string Re_Budget_Period { get; set; }
        public string Pe_By_Spec_Ind { get; set; }
        //public string Privilege_Deposit { get; set; }
        //public string Privilege_Fx_Rate { get; set; }
        //public string Privilege_Subvention { get; set; }
        //public string Privilege_Cwrf_Hpd { get; set; }
        //public string Privilege_Cwrf_Cwd { get; set; }
        //public string Privilege_Cwrf_Ho { get; set; }
        //public string Privilege_Income_Proj { get; set; }
        //public string Privilege_Proj { get; set; }
        //public string Privilege_Annual_Costing { get; set; }
        //public string Privilege_Ce_Report { get; set; }
        //public string Privilege_Cpr_Report { get; set; }
        //public string Privilege_Pending_Ce_Approval { get; set; }
        //public string Privilege_Cash_Position { get; set; }
        //public string Don_Haho { get; set; }
        //public string Don_Hosp { get; set; }
        //public string Don_Bss { get; set; }
    }
}
