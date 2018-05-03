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
    public interface IDonPurposeRepository : IBaseRepository<DonPurpose>
    {
        List<DonPurpose> GetAllDonPurpose();
        List<DonPurpose> GetDonPurposeById(int id);
    }
    public class DonPurposeRepository : BaseRepository<DonPurpose>, IDonPurposeRepository
    {
        private FMRSContext Context;
        private DbSet<DonPurpose> donPurpose;
        public DonPurposeRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donPurpose = Context.Set<DonPurpose>();
        }

        public List<DonPurpose> GetAllDonPurpose()
        {
            return Context.DonPurpose.OrderBy(d => d.DisplayOrder).ToList();
        }

        public List<DonPurpose> GetDonPurposeById(int id)
        {
            return Context.DonPurpose.Where(d => d.Id == id).ToList();
        }


    }
}
