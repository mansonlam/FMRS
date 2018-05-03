using FMRS.Common.DataSource;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IHospitalRepository : IBaseRepository<Hospital>
    {
        string GetPeBySpecInd(string inst_code);
        DataSet GetBudgetClusterHospList(string user_inst_code);
    }
    public class HospitalRepository : BaseRepository<Hospital>, IHospitalRepository
    {
        private FMRSContext Context;
        private DbSet<Hospital> hospital;
        public HospitalRepository(FMRSContext _context, IFMRSUserPrincipal _user) : base(_context)
        {
            Context = _context;
            hospital = Context.Set<Hospital>();
        }

        public string GetPeBySpecInd(string inst_code)
        {
            return Context.Hospital.Where(h => h.HospitalCode == inst_code).Select(h=> h.PeBySpecInd).FirstOrDefault();
        }
        public DataSet GetBudgetClusterHospList(string user_inst_code)
        {
            DataSet ds = new DataSet();
            var sql = "select T1.hospital_code hospital , T1.Cluster Cluster";
            sql = sql + " from hospital T1, hospital T2 ";
            sql = sql + " where T1.cce = 'Y' ";
            sql = sql + " and T1.Cluster = T2.Cluster ";
            sql = sql + " and T2.hospital_code = '" + user_inst_code + "' ";

            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                }
            }
            return ds;
        }

    }
}
