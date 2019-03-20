using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Services
{
    public interface IStoredProcedureService
    {
        void Execute(string sql, SqlParameter[] parameters);
        void Execute(string sql, SqlParameter[] parameters, out object result);

    }
}
