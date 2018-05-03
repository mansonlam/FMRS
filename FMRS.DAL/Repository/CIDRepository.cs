using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface ICIDRepository
    {
        void CID_FUNC_delink_userproj(string domain_user, string loginId);
    }
    public class CIDRepository : ICIDRepository
    {
        private FMRSContext Context;
        public IConfiguration Configuration { get; }
        public CIDRepository(FMRSContext _context) 
        {
            Context = _context;
        }

        public void CID_FUNC_delink_userproj(string domain_user, string loginId)
        {
            using (SqlConnection sqlConn = new SqlConnection(Configuration.GetConnectionString("CIDConnection")))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.CID_FUNC_delink_userproj", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@domain_user", domain_user);
                    sqlCmd.Parameters.AddWithValue("@user_id", " ");
                    sqlCmd.Parameters.AddWithValue("@proj_id", "FRMS");
                    sqlCmd.Parameters.AddWithValue("@upd_by", loginId);
                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();
                }
            }
        }
       

    }
}
