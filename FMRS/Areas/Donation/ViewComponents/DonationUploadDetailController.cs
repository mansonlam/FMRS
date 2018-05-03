using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FMRS.Model.DTO;

namespace FMRS.Areas.Donation.ViewComponents
{
    [ViewComponent(Name = "DonationUploadDetail")]
    public class DonationUploadDetail : ViewComponent
    {
        public IViewComponentResult Invoke(DonationUploadViewModel model)
        {

            return View(model);
        }
    }
}