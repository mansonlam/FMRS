using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class DonSubcat
    {
        public short CatId { get; set; }
        public short Id { get; set; }
        public string Description { get; set; }
        public short? DisplayOrder { get; set; }
        public short? Specify { get; set; }
    }
}
