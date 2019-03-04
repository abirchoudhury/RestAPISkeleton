using System;

namespace RESTfool.DALImplementation
{
    public class DALBase : IDisposable
    {
        private bool _disposed;
        public RESTfool.DBManager.IDBManager DB
        {
            get; set;
        }

        public void CreateDBManager(string connectionString)
        {
            try
            {
                string className = "Unknown";
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    className = "DALImplementation." + this.GetType().Name;
                }

                DB = new RESTfool.DBManager.MySQLManager(connectionString, className);
                _connectionIntiatedHere = true;
            }
            catch (Exception) //ex)
            {
                if (System.Diagnostics.Debugger.IsAttached && BreakOnMethodException)
                {
                    System.Diagnostics.Debugger.Break();
                }
                //ExpensesTracker.Logging.LogException(System.Reflection.MethodBase.GetCurrentMethod(), Logging.LogLevel.Error, ex);
                //throw new BusinessException("Error connecting to database", ex);
            }
        }


        private bool _connectionIntiatedHere = false;

        private bool _breakOnMethodException = true;
        public bool BreakOnMethodException
        {
            get
            {
                return _breakOnMethodException;
            }
            set
            {
                _breakOnMethodException = value;
            }
        }



        public bool PrintDatabaseRequestTimeToDebuggerOutput
        {
            get
            {

                return DB.PrintRequestTimeToDebuggerOutput;
            }
            set
            {
                DB.PrintRequestTimeToDebuggerOutput = value;

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connectionIntiatedHere)
                    {
                        if (DB != null)
                        {
                            DB.Dispose();
                        }
                    }
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





    }
}
