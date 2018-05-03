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
    public interface IDonSubcatRepository : IBaseRepository<DonSubcat>
    {
        List<DonSubcat> GetAllDonSubcat();
        List<DonSubcat> GetDonSubcatByCatId(int catId);
        DonSubcat GetDonSubcatByCatIdDesc(string desc, int catId);
        DonSubcat GetDonSubcatByCatIdSpec(int catId);
    }
    public class DonSubcatRepository : BaseRepository<DonSubcat>, IDonSubcatRepository
    {
        private FMRSContext Context;
        private DbSet<DonSubcat> donSubcat;
        public DonSubcatRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donSubcat = Context.Set<DonSubcat>();
        }

        public List<DonSubcat> GetAllDonSubcat()
        {
            return Context.DonSubcat.OrderBy(d => d.CatId).ThenBy(d => d.DisplayOrder).ToList();
        }

        public List<DonSubcat> GetDonSubcatByCatId(int catId)
        {
            return Context.DonSubcat.Where(d => d.CatId == catId).OrderBy(d => d.CatId).ThenBy(d => d.DisplayOrder).ToList();
        }

        public DonSubcat GetDonSubcatByCatIdDesc(string desc, int catId)
        {
            return Context.DonSubcat.Where(d => d.Description == desc && d.CatId == catId).ToList().FirstOrDefault();
        }

        public DonSubcat GetDonSubcatByCatIdSpec(int catId)
        {
            return Context.DonSubcat.Where(d => d.CatId == catId && d.Specify == 1).ToList().FirstOrDefault();
        }
    }
}
