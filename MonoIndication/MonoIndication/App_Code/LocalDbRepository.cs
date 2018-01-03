using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Web;
using Dapper;

namespace MonoIndication
{
    public class LocalDbRepository
    {
        public List<UserInfoMembership> GetUserInformation()
        {
            string sql = @"SELECT u.UserId, u.UserName, u.Description, m.CreateDate, m.LastPasswordFailureDate
FROM   users AS u LEFT OUTER JOIN  webpages_Membership AS m ON u.UserId = m.UserId";
            List<UserInfoMembership> users=new List<UserInfoMembership>();
            try
            {
                using (IDbConnection connection =new SqlCeConnection(ConfigurationManager.ConnectionStrings["AuthConnection"].ConnectionString))
                {
                    users = connection.Query<UserInfoMembership>(sql).ToList();
                }
            }
            catch (Exception)
            {
                
            }
            return users;
        } 
    }
}