using BlueDot.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace PasswordMigrationTool
{
    public static class PasswordMigration
    {
        public static void StartPasswordMigration()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("databaseConnection.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();
                
                List<UserLogin> list = new List<UserLogin>();

                string classicStringConnection = configuration.GetConnectionString("ClassicDbConnection");
                string selectQueryString = "Select UserLoginId, LoginId, UserId, FirmId From dbo.UserLogin Where encryptedPassword IS NOT NULL";
                string decryptQueryString = "Exec dbo.spEncryptDecrypt @UserLoginId, 'D';";
                using (SqlConnection connection = new SqlConnection(classicStringConnection))
                {
                    SqlCommand command = new SqlCommand(selectQueryString, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UserLogin userLogin = new UserLogin();
                        userLogin.Id = (int)reader["UserLoginId"];
                        userLogin.UserName = (string)reader["LoginId"];
                        userLogin.UserId = (int)reader["UserId"];
                        userLogin.FirmId = (int)reader["FirmId"];
                        userLogin.TwoFactorEnabled = false;
                        userLogin.SecurityStamp = Guid.NewGuid().ToString();

                        list.Add(userLogin);
                    }

                    PasswordHasher<UserLogin> hash = new PasswordHasher<UserLogin>();
                    foreach (UserLogin user in list)
                    {
                        command = new SqlCommand(decryptQueryString, connection);
                        command.Parameters.AddWithValue("@UserLoginId", user.Id);

                        reader.Close();
                        reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            if (reader.IsDBNull(0) == false)
                            {
                                string passTmp = (string)reader[0];
                                string newPass = hash.HashPassword(user, passTmp);
                                user.PasswordHash = newPass;
                                if (hash.VerifyHashedPassword(user, newPass, passTmp) != PasswordVerificationResult.Success)
                                {
                                    Log(user, "Hash password process failed");
                                    user.PasswordHash = null;
                                }
                            }
                            Log(user, "DecryptPassword failed. No password set");
                        }
                    }

                    connection.Close();
                }


                string coreStringConnection = configuration.GetConnectionString("CoreDbConnection"); ;
                string updateQueryString = "Update dbo.UserLogin Set Password = @Password, SecurityStamp = @SecurityStamp Where UserLoginId = @Id";
                using (SqlConnection connection = new SqlConnection(coreStringConnection))
                {
                    int Ok = 0;
                    connection.Open();
                    foreach (UserLogin user in list)
                    {
                        SqlCommand command = new SqlCommand(updateQueryString, connection);

                        if (user.PasswordHash == null)
                        {
                            continue;
                        }
                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@Password", user.PasswordHash);
                        command.Parameters.AddWithValue("@SecurityStamp", user.SecurityStamp);

                        int rows = command.ExecuteNonQuery();

                        if (rows == 1) Ok++;

                    }

                    string msg = "Total Users with encryptPassword column with Data " + list.Count + ".\n Total migrated passwords " + Ok;
                    LogInfo(msg);

                    connection.Close();
                }

                Console.WriteLine("Execution Finished");
                Console.Read();
            }
            catch (Exception ex)
            {
                LogException(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        private static void LogException(string msg)
        {
            StreamWriter w = File.AppendText("log.txt");
            w.WriteLine("-------------------------------");
            w.Write("\r\n Error Exception : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine(msg);
            w.WriteLine("-------------------------------");
            w.Close();
        }

        private static void Log(IdentityUser user, String msg)
        {
            StreamWriter w = File.AppendText("log.txt");
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine("Error Description :");
            w.WriteLine("UserLoginID :{0}", user.Id);
            w.WriteLine("Error :{0}", msg);
            w.WriteLine("-------------------------------");
            w.Close();
        }

        private static void LogInfo(String msg)
        {
            StreamWriter w = File.AppendText("log.txt");
            w.WriteLine("-------------------------------");
            w.Write("\r\nStadistics : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
            w.WriteLine(msg);
            w.WriteLine("-------------------------------");
            w.Close();
        }
    }
}
