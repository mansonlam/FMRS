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
    public interface IDonSupercatRepository : IBaseRepository<DonSupercat>
    {
        List<DonSupercat> GetAllDonSupercat();
        DonSupercat GetSuperCatByDesc(string desc);
    }
    public class DonSupercatRepository : BaseRepository<DonSupercat>, IDonSupercatRepository
    {
        private FMRSContext Context;
        private DbSet<DonSupercat> donSupercat;
        public DonSupercatRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            donSupercat = Context.Set<DonSupercat>();
        }

        public List<DonSupercat> GetAllDonSupercat()
        {
            return Context.DonSupercat.OrderBy(d => d.DisplayOrder).ToList();
        }

        public DonSupercat GetSuperCatByDesc(string desc)
        {
            return Context.DonSupercat.Where(d => d.Description == desc).ToList().FirstOrDefault();
        }

    }
}
