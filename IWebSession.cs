using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities.Interfaces
{
    public interface IWebSession
    {
        string GUID { get; set; }
        LookupData.SessionType Type { get; set; }


        /// <summary>
        /// Returns the user or employee ID
        /// </summary>
        int PrincipalID
        {
            get;
        }

        int? SessionID
        {
            get; set;
        }

        DateTime SessionExpirationTime
        {
            get;
        }

        bool IsSuperAdmin
        {
            get;
        }

    }
}
