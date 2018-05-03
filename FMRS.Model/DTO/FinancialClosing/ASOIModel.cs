using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FMRS.Model.DTO
{
    public class ASOIModel
    {
        public string List_id { get; set; }
        public string Analytical_edit { get; set; }
        public string Section_edit { get; set; }
        public string Duplicate { get; set; }
        public string Update_type { get; set; }
        public string Submit_type { get; set; }
        public ASOISearchModel Sch { get; set; }
        public ASOIResultModel Detail { get; set; }
    }
    public class ASOISearchModel
    {
        public string Hosp_code { get; set; } = "";
        public string Cat_no { get; set; } = "";
        public string Search_corp { get; set; } = "";
        public string Sch_cat_no { get; set; } = "";
        public string Sch_allcat { get; set; } = "";
        public string Sch_analytical_start { get; set; } = "";
        public string Sch_analytical_end { get; set; } = "";
        public string Sch_section { get; set; } = "";
        public string Sch_description_location { get; set; } = "";
        public string Sch_nature_income { get; set; } = "";
        public string Sch_organizer_department { get; set; } = "";
        public string Sch_service_provided { get; set; } = "";
        public string Sch_start_date_begin { get; set; } = "";
        public string Sch_start_date_until { get; set; } = "";
        public string Sch_program_code { get; set; } = "";
        public string Sch_prog_subcat { get; set; } = "";
        public string Subcat_check { get; set; } = "";
        public string Nat_inc_check { get; set; } = "";
        public string Ser_prov_check { get; set; } = "";
        public string Cat_name { get; set; } = "";
        public string Period { get; set; } = "";
        public string Get_max_period { get; set; } = "";

        public List<ASOIResultModel> Result_list { get; set; } = new List<ASOIResultModel>();
    }

    public class ASOIResultModel
    {
        public string Id { get; set; }
        public string Cat { get; set; }
        public string Hosp_code { get; set; }
        public string Cat_name { get; set; } 
        public string Period { get; set; }
        public string Analytical { get; set; }
        public string Section { get; set; }
        public string Program_code { get; set; }
        public string Prog_sub_cat { get; set; }
        public string Prog_desc { get; set; }
        public string Nature_income { get; set; }
        public string Prog_organizer { get; set; }
        public string Service_provided { get; set; }
        public string Start_date { get; set; }
        public string End_date { get; set; }
        public string No_project { get; set; }
        public string Contract_signed { get; set; }
        public string Roll_over { get; set; }
        public decimal Ytd_income { get; set; }
        public decimal Ytd_pe { get; set; }
        public decimal Ytd_oc { get; set; }
        public decimal Ytd_sd { get; set; } 
        public decimal Cyp_income { get; set; }
        public decimal Cyp_pe { get; set; }
        public decimal Cyp_oc { get; set; }
        public decimal Cyp_sd { get; set; }
        public decimal Poa_income { get; set; }
        public decimal Poa_pe { get; set; }
        public decimal Poa_oc { get; set; }
        public decimal Poa_sd { get; set; }
        public string Remarks { get; set; }

        public int CountRp { get; set; }
        public List<SubList> Detail_list { get; set; }
    }

    public class SubList
    {
        public string Id { get; set; }
        public string Rp_id { get; set; }
        public string Prog_desc { get; set; } = "";
        public string Prog_organizer { get; set; } = "";
        public string Start_date { get; set; } = "";
        public string End_date { get; set; } = "";
        public decimal Income { get; set; } = 0;
        public decimal Pe { get; set; } = 0;
        public decimal Oc { get; set; } = 0;
        public string Remarks { get; set; } = "";
    }
}
