using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class AccessRightM
    {
        public string LoginId { get; set; }
        public string UserGroup { get; set; }
        public string AdminM { get; set; }
        public string AsoiRpt { get; set; }
        public string Closing { get; set; }
        public string Cbv { get; set; }
        public string CbvFunding { get; set; }
        public string ClusterAdminM { get; set; }
        public string Cwrf { get; set; }
        public string CwrfFunding { get; set; }
        public string CwrfSubmenu { get; set; }
        public string ReportM { get; set; }
        public string NonPjtReport { get; set; }
        public string CwrfHpd { get; set; }
        public string CwrfCwd { get; set; }
        public string CwrfHo { get; set; }
        public string CwrfStatus { get; set; }
        public string CbvOriUpdate { get; set; }
        public string InputBy { get; set; }
        public DateTime InputDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Project { get; set; }
    }
}
