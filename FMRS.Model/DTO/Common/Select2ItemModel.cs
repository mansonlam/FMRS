using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class Select2ItemModel
    {
        public string Id { get; set; }

        public string Text { get; set; }

        private List<Select2ItemModel> Children { get; set; }
    }
}
