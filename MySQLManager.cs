using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RESTfool.DBManager
{
    public static class DBManagerUtils
    {

        public static void DebugLog(string message)
        {
            //System.Diagnostics.Debug.WriteLine(message + "\t" + memberName + "\r\n \t" + sourceFilePath + "[" + sourceLineNumber + "]");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(message);
            }
        }
    }
    public class MySQLManager : IDBManager, IDisposable
    {
        public MySQLManager(string connection_string, string RequestingClass = "Unknown")
        {
            _conn_str = connection_string;
            conn = new MySqlConnection(_conn_str);
            if (RequestingClass == "Unknown")
            {
                RequestingClass = Environment.NewLine + Environment.StackTrace;
            }
            //  System.Diagnostics.Debug.WriteLine("### DB Manager Started ###, " + System.Runtime.CompilerServices.CallerMemberNameAttribute);
            _requestingClass = RequestingClass;
            DBManagerUtils.DebugLog("##### DB Manager Started    [" + RequestingClass + "] ####");

        }

        public string RequestingClass
        {
            get
            {
                return _requestingClass;
            }
            set
            {
                _requestingClass = value;
            }
        }


        public string ConnectionString
        {
            get
            {
                return _conn_str;
            }
            set
            {
                _conn_str = value;
                conn.ConnectionString = _conn_str;
            }
        }

        private string _conn_str = @"jdbc:mysql://localhost:3306";
        private MySqlConnection conn;

        private MySqlTransaction _Transaction = null;
        private bool _disposed;
        private string _requestingClass = "";
        private bool _timeRequestsToDebuggerOutput = System.Diagnostics.Debugger.IsAttached;
        private System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
        private int? _maxAllowedPacket = null;
        public bool PrintRequestTimeToDebuggerOutput
        {
            get
            {
                return _timeRequestsToDebuggerOutput;
            }
            set
            {
                _timeRequestsToDebuggerOutput = value;
            }

        }

        public int MaxAllowedPacket
        {
            get
            {
                if (_maxAllowedPacket.HasValue == false)
                {
                    _maxAllowedPacket = GetMaxAllowedPacket();
                }
                return _maxAllowedPacket.GetValueOrDefault(32 * 1024 * 1024);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (TransactionStarted)
                    {
                        throw new Exception("Cannot dispose DBManager when a transaction is pending.");
                    }
                    if (conn.State != ConnectionState.Broken && conn.State != ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Dispose();
                    conn = null;
                    GC.Collect();
                    // System.Diagnostics.Debug.WriteLine("### DB Manager Disposed ###");
                    DBManagerUtils.DebugLog("##### DB Manager Disposing  [" + _requestingClass + "] #####");

                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }


        public bool TransactionStarted
        {
            get
            {
                return _Transaction != null;
            }

        }
        private void ConnectDB()
        {
            DBManagerUtils.DebugLog("##### DB Manager Connecting [" + _requestingClass + "] ####");
            if (conn == null)
            {
                conn = new MySqlConnection(_conn_str);
            }

            if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            //if (!conn.Ping())
            //{
            //    conn.Open();
            //}
        }

        private int? GetMaxAllowedPacket()
        {
            string sql = "SHOW VARIABLES LIKE 'max_allowed_packet';";
            DataTable dt = this.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count == 1)
            {
                return int.Parse(dt.Rows[0]["Value"].ToString());
            }
            return null;
        }
        public int ExecuteNonQueryReturnNumRows(string sql, Dictionary<string, object> Parameters)
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }
            ConnectDB();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            foreach (KeyValuePair<string, object> p in Parameters)
            {
                cmd.Parameters.AddWithValue(p.Key, p.Value);
            }
            int r = cmd.ExecuteNonQuery();
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
            }
            return r;
        }

        public int ExecuteNonQueryReturnLastID(string sql, Dictionary<string, object> Parameters)
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }
            ConnectDB();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            foreach (KeyValuePair<string, object> p in Parameters)
            {
                cmd.Parameters.AddWithValue(p.Key, p.Value);
            }
            cmd.ExecuteNonQuery();

            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
            }
            return int.Parse(cmd.ToString());
        }

        public int ExecuteNonQueryReturnLastID(string sql)
        {
            return ExecuteNonQueryReturnLastID(sql, new Dictionary<string, object>());
        }

        public int ExecuteNonQueryReturnNumRows(string sql)
        {
            return ExecuteNonQueryReturnNumRows(sql, new Dictionary<string, object>());
        }

        public DataTable ExecuteDataTable(string sql)
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }
            DataTable ret = new DataTable();
            ConnectDB();
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            da.Fill(ret);
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
            }
            return ret;

        }

        public DataTable ExecuteDataTable(string sql, Dictionary<string, object> dic_parms)
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }
            DataTable ret = new DataTable();
            ConnectDB();
            MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
            da.SelectCommand.CommandTimeout = 100;
            foreach (KeyValuePair<string, object> pair in dic_parms)
            {
                da.SelectCommand.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            da.Fill(ret);
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
            }
            return ret;
        }




        public object ExecuteScalar(string sql)
        {
            ConnectDB();
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows == true)
                    {
                        if (reader.Read())
                        {
                            return reader[0];


                        }
                    }
                }
            }
            return null;
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> Parameters)
        {
            ConnectDB();
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                foreach (KeyValuePair<string, object> p in Parameters)
                {
                    cmd.Parameters.AddWithValue(p.Key, p.Value);
                }
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows == true)
                    {
                        if (reader.Read())
                        {
                            return reader[0];
                        }
                    }
                }
            }
            return null;
        }

        public int SaveChanges(string sql, DataTable changes)
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }
            int count = 0;
            ConnectDB();

            using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
            {

                MySql.Data.MySqlClient.MySqlCommandBuilder builder = new MySqlCommandBuilder(da);
                if (changes != null)
                {
                    da.UpdateCommand = builder.GetUpdateCommand();
                    da.InsertCommand = builder.GetInsertCommand();
                    da.DeleteCommand = builder.GetDeleteCommand();


                    count = da.Update(changes);
                    conn.Close();
                    if (_timeRequestsToDebuggerOutput)
                    {
                        _stopWatch.Stop();
                        DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
                    }
                    return count;
                }
                else
                {
                    return 0;
                }
            }

        }



        public void ChangeDatabase(string Database)
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }

            ConnectDB();
            MySqlCommand cmd = new MySqlCommand(string.Format("USE `{0}`;", Database), conn);
            cmd.ExecuteNonQuery();
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
            }
        }

        public void BeginTransaction()
        {
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Start();
            }

            ConnectDB();
            _Transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            if (_timeRequestsToDebuggerOutput)
            {
                _stopWatch.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
            }
        }

        public void CommitTransaction()
        {
            if (_Transaction != null)
            {
                if (_timeRequestsToDebuggerOutput)
                {
                    _stopWatch.Start();
                }
                _Transaction.Commit();
                _Transaction = null;
                if (_timeRequestsToDebuggerOutput)
                {
                    _stopWatch.Stop();
                    DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
                }
            }
        }

        public void RollbackTransaction()
        {
            if (_Transaction != null)
            {
                if (_timeRequestsToDebuggerOutput)
                {
                    _stopWatch.Start();
                }
                _Transaction.Rollback();
                _Transaction = null;
                if (_timeRequestsToDebuggerOutput)
                {
                    _stopWatch.Stop();
                    DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, _stopWatch.Elapsed.TotalSeconds));
                }
            }
        }

        public static DataTable GetDBsForServer(string serverip)
        {
            DataTable result = new DataTable();
            try
            {

                //string command = "SELECT SCHEMA_NAME as 'Database' FROM information_schema.SCHEMATA WHERE LEFT(SCHEMA_NAME,3) = 'lms' OR LEFT(SCHEMA_NAME, 4) = 'prod';";
                //string command = "SELECT SCHEMA_NAME as 'Database' FROM information_schema.SCHEMATA";

                ///////////////////////////////////////////////////////////////////////////////////
                // Alternate LINQ based way that is easier to maintain but appears to be slower
                ///////////////////////////////////////////////////////////////////////////////////

                //List<string> DatabasePrefixes = new List<string>();
                //DatabasePrefixes.Add("lms");
                //DatabasePrefixes.Add("prod");
                //DatabasePrefixes.Add("demo");
                //DatabasePrefixes.Add("csg");
                //result = (from DataRow dr in result.Rows where DatabasePrefixes.Contains(dr.Field<string>("Database").Substring(0, 4).Replace("_", "")) orderby dr.Field<string>("Database") select dr).CopyToDataTable();

                ///////////////////////////////////////////////////////////////////////////////////
                // End Alternate LINQ based way
                ///////////////////////////////////////////////////////////////////////////////////

                string command = "SELECT SCHEMA_NAME as 'Database' FROM information_schema.SCHEMATA WHERE LEFT(SCHEMA_NAME,3) = 'lms' OR LEFT(SCHEMA_NAME, 4) = 'prod' OR LEFT(SCHEMA_NAME, 4) = 'demo' OR LEFT(SCHEMA_NAME,3) = 'csg' OR LEFT(SCHEMA_NAME,3) = 'sys' OR LEFT(SCHEMA_NAME,3) = 'dev';";
                string conn = "SERVER=" + serverip + ";DATABASE=information_schema;UID=lms;pwd=lms;";
                using (MySql.Data.MySqlClient.MySqlConnection connection = new MySqlConnection(conn))
                {
                    MySql.Data.MySqlClient.MySqlDataAdapter da = new MySqlDataAdapter(command, connection);
                    da.Fill(result);
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void BatchInsertRows(string InsertStmtWithNoParameters, List<object> objectToSave, Dictionary<string, string> FieldMapping = null)
        {

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            if (_timeRequestsToDebuggerOutput)
            {
                sw.Start();
            }
            Dictionary<string, object> dic_parms = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(InsertStmtWithNoParameters);
            object ob = null;
            if (objectToSave.Count > 0)
            {
                ob = objectToSave.FirstOrDefault() as object;
            }
            Dictionary<string, string> parmList = new Dictionary<string, string>();
            if (FieldMapping == null)
            {
                PropertyInfo[] pInfos = ob.GetType().GetProperties();
                foreach (var pInfo in pInfos)
                {
                    if (pInfo.CanWrite)
                    {
                        parmList.Add(pInfo.Name, pInfo.Name);
                    }
                }
            }
            else
            {
                parmList = FieldMapping;
            }


            int i = 1;
            foreach (var r in objectToSave)
            {
                object o = r as object;
                sb.Append(" (");
                int c = 1;
                foreach (var kvp in parmList)
                {
                    string parmName = kvp.Key + "_" + i.ToString();
                    sb.Append(parmName);
                    if (c < parmList.Count)
                    {
                        sb.Append(", ");
                    }

                    object value = kvp.Value;
                    dic_parms.Add(parmName, value);
                    c++;
                }

                sb.Append(" )");
                if (i < objectToSave.Count)
                {
                    sb.Append(",");
                }

                i++;
            }
            string sql = sb.ToString().Trim();
            this.ExecuteNonQueryReturnNumRows(sql, dic_parms);
            if (_timeRequestsToDebuggerOutput)
            {
                sw.Stop();
                DBManagerUtils.DebugLog(string.Format("@@ {0} took {1} seconds.", System.Reflection.MethodBase.GetCurrentMethod().Name, sw.Elapsed.TotalSeconds));
            }


        }



        public void CreateIDTempTable(List<int> IDList, string TempTableName)
        {
            string sql = "";
            //string sql = @"DROP TEMPORARY TABLE IF EXISTS " + TempTableName;
            //ExecuteNonQueryReturnNumRows(sql);

            //sql = @"  CREATE TEMPORARY TABLE IF NOT EXISTS " + TempTableName + @"
            //          (
            //            ID INT
            //          )";
            //ExecuteNonQueryReturnNumRows(sql);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("DROP TEMPORARY TABLE IF EXISTS " + TempTableName + ";");
            sb.AppendLine("CREATE TEMPORARY TABLE IF NOT EXISTS " + TempTableName + "(ID INT);");
            sb.AppendFormat("INSERT INTO {0} VALUES ", TempTableName);


            foreach (int ID in IDList)
            {
                sb.AppendFormat("({0}), ", ID);
            }

            sql = sb.ToString();
            if (sql.Trim().EndsWith(","))
            {
                sql = sql.Substring(0, sql.Length - 2);
            }
            sql += ";";
            ExecuteNonQueryReturnNumRows(sql);
        }
    }
}
