using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class EnquireUserViewModel
    {
        public string Modules { get; set; }
        public string Function_type { get; set; }
        public string LastLogin_string { get; set; }
        public string Specialty_list { get; set; }
        public FVUser FV_User { get; set; }

        public UserInfo AdminUserInfo { get; set; }
        public string AdminUserHospital { get; set; }
        public string All_specialty { get; set; }
        public string UserSpecCode { get; set; }
        public FVUser AdminFV_User { get; set; }
        public AccessRightD AdminRightD { get; set; }
        public AccessRightM AdminRightM { get; set; }
        public AccessRightY AdminRightY { get; set; }
    }

    public class FVUser
    {
        public string FV_user_role { get; set; } = "";
        public string FV_user_cluster { get; set; } = "";
        public string FV_user_admin2 { get; set; } = "";
    }
}
