using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FMRS.Model.DTO;

namespace FMRS.Areas.Donation.ViewComponents
{
    [ViewComponent(Name = "DonationReserveSubList")]
    public class DonationReserveSubList : ViewComponent
    {
        public IViewComponentResult Invoke(int record_cnt, DonationRecNExpModel model)
        {
            if (record_cnt == 0)
            { return View("NoRecord"); }
            else
            return View(model);
        }
    }
}