using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class DonSubsubcat
    {
        public short SubcatId { get; set; }
        public short Id { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public short? Specify { get; set; }
    }
}
