using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FMRS.Models
{
    public class AppConfiguration
    {
        public bool ClientValidationEnabled { get; set; }
        public string FMRS_idmanDomain { get; set; }
        public string CID_location { get; set; }
        public string CID_dup_ID { get; set; }
        public string cidLinkServer { get; set; }
        public string BSD_Url { get; set; }
        public string SSRSSERVER { get; set; }
        public string SSRS_folder { get; set; }
        public string Report_folder { get; set; }

    }
}
