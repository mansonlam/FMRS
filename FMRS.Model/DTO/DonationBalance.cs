using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class DonationBalance
    {
        public string Hospital { get; set; }
        public string Fund { get; set; }
        public string Section { get; set; }
        public string Analytical { get; set; }
        public DateTime InputFor { get; set; }
        public decimal Income { get; set; }
        public decimal Expenditure { get; set; }
        public decimal ReserveBalBegin { get; set; }
        public decimal ReserveBalEnd { get; set; }
        public string CrBy { get; set; }
        public DateTime CrDt { get; set; }
        public string UpdtBy { get; set; }
        public DateTime UpdtDt { get; set; }
    }
}
