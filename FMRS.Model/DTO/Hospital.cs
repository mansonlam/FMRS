using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class Hospital
    {
        public string HospitalCode { get; set; }
        public string Description { get; set; }
        public string HeadofficeInd { get; set; }
        public string PeBySpecInd { get; set; }
        public string Cluster { get; set; }
        public string DdGp { get; set; }
        public string ShowHpsInd { get; set; }
        public int? FlashPgNo { get; set; }
        public string AnnualCostingInd { get; set; }
        public string Cce { get; set; }
    }
}
