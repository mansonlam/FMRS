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
    public interface IDonSubsubcatRepository : IBaseRepository<DonSubsubcat>
    {
        List<DonSubsubcat> GetAllDonSubsubcat();
        List<DonSubsubcat> GetDonSubsubcatBySubCatId(int subcat_id);
        DonSubsubcat GetDonSubsubcatBySubCatIdDesc(string desc, int subcat_id);
    }
    public class DonSubsubcatRepository : BaseRepository<DonSubsubcat>, IDonSubsubcatRepository
    {
        private FMRSContext Context;
        private DbSet<DonSubsubcat> donSubsubcat;
        public DonSubsubcatRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donSubsubcat = Context.Set<DonSubsubcat>();
        }

        public List<DonSubsubcat> GetAllDonSubsubcat()
        {
            return Context.DonSubsubcat.OrderBy(d => d.SubcatId).ThenBy(d => d.DisplayOrder).ToList();
        }

        public List<DonSubsubcat> GetDonSubsubcatBySubCatId(int subcat_id)
        {
            return Context.DonSubsubcat.Where(d => d.SubcatId == subcat_id).OrderBy(d => d.SubcatId).ThenBy(d => d.DisplayOrder).ToList();
        }

        public DonSubsubcat GetDonSubsubcatBySubCatIdDesc(string desc, int subcat_id)
        {
            return Context.DonSubsubcat.Where(d => d.Description == desc && d.SubcatId == subcat_id).ToList().FirstOrDefault();
        }
    }
}
