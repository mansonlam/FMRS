using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class DonationDetail
    {
        public int Id { get; set; }
        public string Hospital { get; set; }
        public short? Trust { get; set; }
        public string Fund { get; set; }
        public string Section { get; set; }
        public string Analytical { get; set; }
        public int? LinkId { get; set; }
        public short? DonorId { get; set; }
        public string DonorName { get; set; }
        public string DonorType { get; set; }
        public string DonIncExp { get; set; }
        public string DonType { get; set; }
        public short DonPurpose { get; set; }
        public short DonSupercat { get; set; }
        public short DonCat { get; set; }
        public short DonSubcat { get; set; }
        public short DonSubsubcat { get; set; }
        public string DonSpecific { get; set; }
        public decimal? DonCurMth { get; set; }
        public string DonKindDesc { get; set; }
        public string MajDon1 { get; set; }
        public string MajDon2 { get; set; }
        public string MajDon3 { get; set; }
        public short? EqtDesc { get; set; }
        public string RecurrentCon { get; set; }
        public string Reimb { get; set; }
        public decimal? RecurrentCost { get; set; }
        public string InputBy { get; set; }
        public DateTime InputAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
