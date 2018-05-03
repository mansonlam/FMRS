using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class AccessRight
    {
        public string LoginId { get; set; }
        public string AccessType { get; set; }
        public string Privilege { get; set; }
    }
}
