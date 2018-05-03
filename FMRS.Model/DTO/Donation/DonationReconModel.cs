using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class DonationReconModel
    {
        public int Recon_type { get; set; }
        public string Inst_code { get; set; }
        public List<DonationMovement> Movement_list { get; set; }
        public decimal Total { get; set; }
        public decimal Cost { get; set; }
        public List<DonationRecNExpModel> DonationList { get; set; }
    }

    public class DonationMovement
    {
        public string Fund { get; set; }
        public string Account { get; set; }
        public decimal Cost { get; set; }
    }
}
