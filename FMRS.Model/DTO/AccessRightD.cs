using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class AccessRightD
    {
        public string LoginId { get; set; }
        public string UserGroup { get; set; }
        public string AdminD { get; set; }
        public string AsoiRpt { get; set; }
        public string Closing { get; set; }
        public string Donation { get; set; }
        public string ReportD { get; set; }
        public string NonPjtReport { get; set; }
        public string InputBy { get; set; }
        public DateTime InputDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
