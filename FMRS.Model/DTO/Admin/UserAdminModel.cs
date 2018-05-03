using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class UserAdminModel
    {
        public string User_group { get; set; }
        public List<UserInfoViewModel> User_list { get; set; }
    }
    public class UserAdminCollection
    {
        public List<UserAdminModel> Collection { get; set; }
    }

    public class UserInfoViewModel
    {
        public UserInfo UserInfo { get; set; }
        public string LastLogin_string { get; set; }
    }
}
