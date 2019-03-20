using BlueDot.Data;
using BlueDot.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models;
using Services.Impls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PasswordMigrationTool
{
    public static class ClaimMigration
    {
        public async static void StartClaimMigration()
        {
            Console.WriteLine("Claim Migration Execution Started");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("databaseConnection.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            Dictionary<string, string> valuePairs = GetNewOldClaim();

            string coreStringConnection = configuration.GetConnectionString("CoreDbConnection"); ;
            string selectQueryString = "Select * From dbo.UserLogin";
            using (SqlConnection connection = new SqlConnection(coreStringConnection))
            {
                SqlCommand command = new SqlCommand(selectQueryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    foreach(KeyValuePair<string,string> item in valuePairs)
                    {
                        int UserId = (int)reader["UserLoginId"];
                        bool? actualVal = (bool?)reader[item.Key];
                        if(actualVal.HasValue && actualVal.Value)
                        {
                            await AddUserClaim(connection, UserId, item.Value, "1");
                        }
                    }
                }
                connection.Close();
            }
            Console.WriteLine("Claim Migration Execution Finished");
            Console.Read();
        }

        public static async Task AddUserClaim(SqlConnection connection, int UserId, string ClaimType, string ClaimValue)
        {
            string QueryString = "Insert into  UserClaim (UserId, ClaimType, ClaimValue) values (@UserId, @ClaimType, @ClaimValue)";
            SqlCommand command = new SqlCommand(QueryString, connection);
            command.Parameters.AddWithValue("@UserId", UserId);
            command.Parameters.AddWithValue("@ClaimType", ClaimType);
            command.Parameters.AddWithValue("@ClaimValue", ClaimValue);
            await command.ExecuteNonQueryAsync();
        }
        
        public static Dictionary<string, string> GetNewOldClaim()
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            return valuePairs;
        }
    }
}
