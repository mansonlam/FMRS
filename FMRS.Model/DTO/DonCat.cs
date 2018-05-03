using System;
using System.Collections.Generic;

namespace FMRS.Model.DTO
{
    public partial class DonCat
    {
        public short Id { get; set; }
        public short? SupercatId { get; set; }
        public string Description { get; set; }
        public short? DisplayOrder { get; set; }
        public string Designated { get; set; }
    }
}
