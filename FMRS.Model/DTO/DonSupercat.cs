using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class DonSupercat
    {
        public short SupercatId { get; set; }
        public string Description { get; set; }
        public short? DisplayOrder { get; set; }
    }
}
