using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FMRS.DAL.Repository
{
    public interface IDonTypeRepository : IBaseRepository<DonType>
    {
        List<DonType> GetAllDonType();
        List<DonType> GetDonTypeById(int id);
    }
    public class DonTypeRepository : BaseRepository<DonType>, IDonTypeRepository
    {
        private FMRSContext Context;
        private DbSet<DonType> donType;
        public DonTypeRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donType = Context.Set<DonType>();
        }

        public List<DonType> GetAllDonType()
        {
            return Context.DonType.OrderBy(d => d.DisplayOrder).ToList();
        }

        public List<DonType> GetDonTypeById(int id)
        {
            return Context.DonType.Where(d => d.Id == id).ToList();
        }
    }
}
