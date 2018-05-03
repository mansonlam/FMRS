using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace FMRS.DAL.Repository
{
    public interface IUserSpecialtyRespository : IBaseRepository<UserSpecialty>
    {
        List<string> GetUserSpecialtyByLoginID(string loginId);
        void DeleteUserSpecialtyByLoginId(string loginId);
        void InsertUserSpecialtyByLoginIdSpecialty(string loginId, string specialty);
    }

    public class UserSpecialtyRespository : BaseRepository<UserSpecialty>, IUserSpecialtyRespository
    {
        private FMRSContext Context;
        private DbSet<UserSpecialty> userSpecialty;
        public UserSpecialtyRespository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            userSpecialty = Context.Set<UserSpecialty>();
        }
        public List<string> GetUserSpecialtyByLoginID(string loginId)
        {
            return Context.UserSpecialty.Where(u => u.LoginId == loginId).Select(u => u.Specialty).ToList();
        }

        public void DeleteUserSpecialtyByLoginId(string loginId)
        {
            var sql = "delete user_specialty where user_name = '" + loginId.Replace("\\", "/") + "'";
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    sqlCmd.Connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertUserSpecialtyByLoginIdSpecialty(string loginId, string specialty)
        {
            var sql = "insert into user_specialty (user_name, specialty) values ('" + loginId.Replace("\\", "/") + "', " + specialty + ")";
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
