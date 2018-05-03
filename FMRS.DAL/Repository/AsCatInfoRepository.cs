using FMRS.Model.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace FMRS.DAL.Repository
{
    public interface IAsCatInfoRepository
    {
        string GetCatName(string cat_no);
    }

    public class AsCatInfoRepository : IAsCatInfoRepository
    {
        private FMRSContext Context;
        public AsCatInfoRepository(FMRSContext _context)
        {
            Context = _context;
        }

        public string GetCatName(string cat_no)
        {
            string result = "";
            DataSet ds = new DataSet();
            var sql = " select distinct cat_name from as_cat_info where cat_id = " + cat_no ;
            using (SqlConnection sqlConn = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                {
                    SqlDataAdapter adp = new SqlDataAdapter(sqlCmd);
                    adp.Fill(ds);
                    if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                    {
                        result = ds.Tables[0].Rows[0]["cat_name"].ToString();
                    }
                }
            }
            return result;
        }
    }
}
