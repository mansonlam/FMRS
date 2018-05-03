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
    public interface IDonCatRepository : IBaseRepository<DonCat>
    {
        List<DonCat> GetAllDonCat();
        List<DonCat> GetDonCatBySuperCatId(int supercat_id);
        DonCat GetDonCatByDesc(string desc);
    }
    public class DonCatRepository : BaseRepository<DonCat>, IDonCatRepository
    {
        private FMRSContext Context;
        private DbSet<DonCat> donCat;
        public DonCatRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donCat = Context.Set<DonCat>();
        }

        public List<DonCat> GetAllDonCat()
        {
            return Context.DonCat.OrderBy(d => d.SupercatId).ThenBy(d => d.DisplayOrder).ToList();
        }

        public List<DonCat> GetDonCatBySuperCatId(int supercat_id)
        {
            return Context.DonCat.Where(d => d.SupercatId == supercat_id).OrderBy(d => d.SupercatId).ThenBy(d => d.DisplayOrder).ToList();
        }

        public DonCat GetDonCatByDesc(string desc)
        {
            return Context.DonCat.Where(d => d.Description == desc).ToList().FirstOrDefault();
        }
    }
}
