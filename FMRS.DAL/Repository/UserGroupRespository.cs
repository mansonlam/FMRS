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
    public interface IUserGroupRespository : IBaseRepository<UserGroup>
    {
        List<UserGroup> GetUserGroup();
        string GetUserGroupDesc(string user_group);
        DataSet GetUserGroupDescFromUserGpHosp(string user_group);
    }

    public class UserGroupRespository : BaseRepository<UserGroup>, IUserGroupRespository
    {
        private FMRSContext Context;
        private DbSet<UserGroup> userGroup;
        public UserGroupRespository(FMRSContext _context) : base(_context)
        {
            Context = _context;
            userGroup = Context.Set<UserGroup>();
        }

        public List<UserGroup> GetUserGroup()
        {
            return Context.UserGroup.ToList();
        }

        public string GetUserGroupDesc(string user_group)
        {
            return Context.UserGroup.Where(u => u.UserGroup1 == user_group).Select(u => u.Description).First();
        }

        public DataSet GetUserGroupDescFromUserGpHosp(string user_group)
        {
            DataSet ds = new DataSet();
            var sql = "select distinct g.user_group, g.description from user_group g, user_group_hosp T1 ";
            sql = sql + " where g.user_group = T1.user_group ";
            sql = sql + " and not exists (select * from user_group_hosp T2 ";
            sql = sql + " where T2.user_group = T1.user_group ";
            sql = sql + " and T2.hospital_code not in (select hospital_code from user_group_hosp T3 ";
            sql = sql + " where T3.user_group = '" + user_group + "'))";
            sql = sql + " order by g.user_group ";
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
