using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface ICwrfAccessControlRepository : IBaseRepository<CwrfAccessControl>
    {
        List<CwrfAccessControl> GetCwrfAccessControlByUserName(string user_name);
        void DeleteCwrfAccessControlByLoginId(string loginId);
        void InsertCwrfAccessControlByLoginId(string loginId, int tran_id);
        void InsertCwrfAccessControlFromCbvTranType(string loginId);
    }
    public class CwrfAccessControlRepository : BaseRepository<CwrfAccessControl>, ICwrfAccessControlRepository
    {
        private FMRSContext Context;
        private DbSet<CwrfAccessControl> cwrfAccessControl;

        public CwrfAccessControlRepository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            cwrfAccessControl = Context.Set<CwrfAccessControl>();
        }

        public List<CwrfAccessControl> GetCwrfAccessControlByUserName(string user_name)
        {
            return Context.CwrfAccessControl.Where(c => c.UserName == user_name.Replace("\\","/").ToLower()).ToList();
        }

        public void DeleteCwrfAccessControlByLoginId(string loginId)
        {
            var sql = "delete cwrf_access_control where user_name = '" + loginId.Replace("\\", "/") + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertCwrfAccessControlByLoginId(string loginId, int tran_id)
        {
            var sql = "insert into cwrf_access_control (user_name, tran_id) values ('" + loginId.Replace("\\", "/") + "', " + tran_id + ")";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertCwrfAccessControlFromCbvTranType(string loginId)
        {
            var sql = "insert into cwrf_access_control (user_name, tran_id) ";
            sql = sql + "select '" + loginId.Replace("\\", "/") + "', tran_id ";
            sql = sql + "from cbv_tran_type ";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
