using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace FMRS.DAL.Repository
{
    public interface IDonDonorRepository : IBaseRepository<DonDonor>
    {
        List<DonDonor> GetAllDonDonor();
        int GetDonorCnt(string donor_name);
        DonDonor GetDonorByDesc(string desc);
        //TEST
        //bool CreateDonor(string don_kind_desc);
    }
    public class DonDonorRepository : BaseRepository<DonDonor>, IDonDonorRepository
    {
        private FMRSContext Context;
        private DbSet<DonDonor> donDonor;
        public DonDonorRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donDonor = Context.Set<DonDonor>();
        }

        public List<DonDonor> GetAllDonDonor()
        {
            return Context.DonDonor.OrderBy(d => d.DisplayOrder).ToList();
        }

        public int GetDonorCnt(string donor_name)
        {
            return Context.DonDonor.Where(d => d.Description == donor_name).ToList().Count();
        }

        public DonDonor GetDonorByDesc(string desc)
        {
            return Context.DonDonor.Where(d => d.Description == desc).FirstOrDefault();
        }
        //TEST
        //public bool CreateDonor(string don_kind_desc)
        //{
        //    var sql = "Insert into don_donor (description, display_order) select 'TEST " + don_kind_desc + "' as description, max(display_order)+5 as display_order from don_donor ";
        //    using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
        //    {
        //        using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
        //        {
        //            try
        //            {
        //                sqlConn.Open();
        //                sqlCmd.ExecuteNonQuery();
        //                sqlConn.Close();
        //            }
        //            catch (Exception ex)
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
    }
}
