using BlueDot.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using BlueDot.Models.Identity;

namespace PasswordMigrationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            //PasswordMigration.StartPasswordMigration();
            ClaimMigration.StartClaimMigration();   
        }
        
    }
}
