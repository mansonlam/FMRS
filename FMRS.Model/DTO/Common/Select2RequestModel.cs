using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class Select2RequestModel
    {
        public string Term { get; set; }
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public long RecordsTotal { get; set; }

        public Select2RequestModel()
        {
            RecordsPerPage = 20;
        }

        public int GetFirstRecord()
        {
            return ((this.Page == 0 ? 1 : this.Page) - 1) * this.RecordsPerPage;
        }
        public bool HasMore()
        {
            return ((this.Page == 0 ? 1 : this.Page) * this.RecordsPerPage) < this.RecordsTotal;
        }
    }
}
