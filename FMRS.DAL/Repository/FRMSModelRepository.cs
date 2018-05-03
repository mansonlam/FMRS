using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IFRMSModelRepository
    {
        bool FrmsCheckAnyUser(string loginId);
    }
    public class FRMSModelRepository : IFRMSModelRepository
    {
        private FMRSContext Context;

        public FRMSModelRepository(FMRSContext _context) 
        {
            Context = _context;
        }

        public bool FrmsCheckAnyUser(string loginId)
        {
            bool result = false;
            DataSet ds = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("FRMS_model..frms_check_any_user", sqlConn))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@domain_user", loginId);

                    sqlConn.Open();
                    using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                    {
                        sqlAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
       

    }
}
