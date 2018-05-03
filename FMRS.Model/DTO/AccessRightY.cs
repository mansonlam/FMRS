using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class AccessRightY
    {
        public string LoginId { get; set; }
        public string UserGroup { get; set; }
        public string AdminY { get; set; }
        public string AsoiInput { get; set; }
        public string AsoiRpt { get; set; }
        public string Closing { get; set; }
        public string ReportY { get; set; }
        public string NonPjtReport { get; set; }
        public string FarAccess { get; set; }
        public string InputBy { get; set; }
        public DateTime InputDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string FvInput { get; set; }
        public string FvCluster { get; set; }
        public string FvUserAdmin { get; set; }
    }
}
