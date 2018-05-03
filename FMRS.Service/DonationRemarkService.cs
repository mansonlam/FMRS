using FMRS.DAL.Repository;
using FMRS.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Service
{
    public interface IDonationRemarkService
    {
        string GetRemarkByHosp(string inst_code);
        void UpdateDonationRemark(DonationRemarkModel model, string login_id);
    }
    public class DonationRemarkService : IDonationRemarkService
    {
        private IDonationRemarkRepository DonationRemarkRepository;
        public DonationRemarkService(IDonationRemarkRepository _donationRemarkRepository)
        {
            DonationRemarkRepository = _donationRemarkRepository;
        }

        public string GetRemarkByHosp(string inst_code)
        {
            return DonationRemarkRepository.GetRemarkByHosp(inst_code);
        }

        public void UpdateDonationRemark(DonationRemarkModel model, string login_id)
        {
            DonationRemarkRepository.DeleteRemarkByHosp(model.Inst_code);
            DonationRemarkRepository.InsertRemarkByHosp(model.Inst_code, model.Remark, login_id);
        }
    }
}
