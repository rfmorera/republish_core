using Republish.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace Services.Impls
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly ApplicationDbContext _context;
        public StoredProcedureService(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Execute(string sql, SqlParameter[] parameters)
        {
            _context.Database.ExecuteSqlCommand(sql, parameters);
        }

        public void Execute(string sql, SqlParameter[] parameters, out object result)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                //foreach(SqlParameter sqlParameter in parameters)
                //{
                //    DbParameter dbParameter = command.CreateParameter();
                //    dbParameter.ParameterName = sqlParameter.ParameterName;
                //    dbParameter.Value = sqlParameter.Value;
                //    dbParameter.DbType = sqlParameter.DbType;
                //    command.Parameters.Add(dbParameter);
                //}
                command.Parameters.AddRange(parameters);
                _context.Database.OpenConnection();
                result = command.ExecuteScalar();
            }
        }
    }
}
