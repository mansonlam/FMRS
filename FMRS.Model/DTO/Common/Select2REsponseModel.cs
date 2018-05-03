using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class Select2ResponseModel
    {
        public Select2ResponseModel()
        {
            Pagination = new Select2Pagination();
        }
        public int Total_count { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public Select2Pagination Pagination { get; set; }
        public List<Select2ItemModel> Results { get; set; }
    }
    public class Select2Pagination
    {
        public bool More { get; set; }
    }
}
