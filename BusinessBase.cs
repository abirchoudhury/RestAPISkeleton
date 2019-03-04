using BusinessImplementation;
using RESTfool.DBManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTfool.BusinessImplementation
{
    public class BusinessBase : IDisposable
    {

        
        public IDBManager DB
        {
            get; set;
        }
        public PermissionsManager PermissionManager
        {
            get
            {
                if (_permissionMgr == null)
                {
                    _permissionMgr = new PermissionsManager(DB, WebSession);
                }
                return _permissionMgr;
            }
        }
        public DataEntities.Interfaces.IWebSession WebSession
        {
            get; set;
        }

        
        private PermissionsManager _permissionMgr = null;
        private bool _disposed;

        public bool _connectionIntiatedHere = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_connectionIntiatedHere && DB != null)
                    {
                        DB.Dispose();
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
