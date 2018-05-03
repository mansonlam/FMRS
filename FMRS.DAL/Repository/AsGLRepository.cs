using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsGLRepository
    {
        string GetPeriodFromERPGL();
    }

    public class AsGLRepository : IAsGLRepository
    {
        private FMRSContext Context;
        public AsGLRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public string GetPeriodFromERPGL()
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = " select max(period)as period from as_gl" ;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = ds.Tables[0].Rows[0]["period"].ToString();
                    }
                }
            }
            return result;
        }
    }
}
