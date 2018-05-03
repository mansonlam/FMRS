using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class UserInfo
    {
        public string LoginId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PwdExpiry { get; set; }
        public string InstCode { get; set; }
        public string UserGroup { get; set; }
        public DateTime? LastLogin { get; set; }
        public string HoUser { get; set; }
        public string DomainUser { get; set; }
        public string DomainInst { get; set; }
        public string FinancialClosing { get; set; }
        public string ProjectManagement { get; set; }
        public string Donation { get; set; }
        public string InputBy { get; set; }
        public DateTime? InputDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
