using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FMRS.Common.Resources;

namespace FMRS.Models.Account
{
    public class LoginModel
    {
        [Required]
        [Display(ResourceType = typeof(Resource), Name = "UserName")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resource), Name = "Domain")]
        public string Domain { get; set; }

        [Display(ResourceType = typeof(Resource), Name = "ChangePassword")]
        public bool ChangePassowrd { get; set; }

    }
}
