using System;
using System.Collections.Generic;
using System.Data;

namespace RESTfool.DBManager
{
    public interface IDBManager : IDisposable
    {
        string RequestingClass
        {
            get; set;
        }
        string ConnectionString
        {
            get; set;
        }
        bool PrintRequestTimeToDebuggerOutput
        {
            get; set;
        }

        int ExecuteNonQueryReturnNumRows(string sql, Dictionary<string, object> Parameters);
        int ExecuteNonQueryReturnLastID(string sql, Dictionary<string, object> Parameters);
        int ExecuteNonQueryReturnLastID(string sql);
        int ExecuteNonQueryReturnNumRows(string sql);
        DataTable ExecuteDataTable(string sql);
        DataTable ExecuteDataTable(string sql, Dictionary<string, object> dic_parms);
        object ExecuteScalar(string sql);
        object ExecuteScalar(string sql, Dictionary<string, object> Parameters);
        int SaveChanges(string sql, DataTable changes);
        void ChangeDatabase(string Database);
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void BatchInsertRows(string InsertStmtWithNoParameters, List<object> objectToSave, Dictionary<string, string> FieldMapping = null);
        void CreateIDTempTable(List<int> IDList, string TempTableName);
    }
}


