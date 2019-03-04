using RESTfool.DBManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessImplementation
{
    public class PermissionsManager
    {
        private IDBManager DB = null;
        private DataEntities.Interfaces.IWebSession WebSession = null;        
        private BusinessImplementation.UserCompanyAccess _userCompanyAccessBAL = null;
        public PermissionsManager(IDBManager db, DataEntities.Interfaces.IWebSession session)
        {
            DB = db;
            WebSession = session;
            _userCompanyAccessBAL = new BusinessImplementation.UserCompanyAccess(db);
        }

        /// <summary>
        /// Checks to see if the provided user ID has permission to access data by
        /// comparing websession IDs and SessionType. Used to prevent an employee or user from accessing another employee's data.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool AssertUserAccessByID(int? userID, int? employeeID)
        {
            if (WebSession != null && userID.Value == WebSession.PrincipalID && userID > 0)
            {
                if (WebSession.Type == DataEntities.LookupData.SessionType.User)
                {
                    return true;
                }
            }
            else if (WebSession != null && employeeID.Value == WebSession.PrincipalID && employeeID > 0)
            {
                if (WebSession.Type == DataEntities.LookupData.SessionType.Employee)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if the session type is User. 
        /// </summary>
        /// <returns></returns>
        public bool IsUserSession()
        {
            return WebSession == null || WebSession.Type == DataEntities.LookupData.SessionType.User;
        }

        /// <summary>
        /// Checks to see if the session type is Employee
        /// </summary>
        /// <returns></returns>
        public bool IsEmployeeSession()
        {
            return WebSession == null || WebSession.Type == DataEntities.LookupData.SessionType.Employee;
        }

        /// <summary>
        /// Checks to see if the current session has access to the specified employee id.
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public bool AssertEmployeeAccess(int? employeeID)
        {
            //TODO: Manager group / company access checking for employee access.

            if (WebSession == null)
            {
                return true;
            }

            if (IsUserSession())
            {
                return true;
            }

            if (IsEmployeeSession())
            {
                return employeeID == WebSession.PrincipalID;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the parameters that are passed can be found on the usercompanyaccess table. 
        /// Pass 0 as an argument in appropriate position to validate either userID OR companyID. 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public bool AssertUserCompanyAccess(int? userID, int? companyID)
        {
            //SuperAdmin can access everything.
            if ((WebSession != null && WebSession.IsSuperAdmin) || userID.GetValueOrDefault() == 1)
            {
                return true;
            }

            //TODO: Move to usercompanyaccess DAL
            return _userCompanyAccessBAL.GetUserCompanyAccessByID(userID, companyID);
        }

        /// <summary>
        /// Checks to see if the current session is a user session and that session has access to the specified company.
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public bool AssertUserCompanyAccess(int? companyID)
        {
            if (WebSession == null)
            {
                return true;
            }
            if (WebSession != null && WebSession.Type != DataEntities.LookupData.SessionType.User)
            {
                return false;
            }
            return AssertUserCompanyAccess(WebSession.PrincipalID, companyID);
        }

        /// <summary>
        /// Checks to see if the current session is an employee session and that employee has access to the specified company.
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public bool AssertEmployeeCompanyAccess(int? companyID)
        {
            if (WebSession == null)
            {
                return true;
            }
            if (WebSession != null && WebSession.Type != DataEntities.LookupData.SessionType.Employee)
            {
                return false;
            }

            var employeeSession = new { CompanyID = 1 }; //WebSession as DataEntities.Employee.EmployeeSessionData;
            return employeeSession.CompanyID == companyID;
        }

        /// <summary>
        /// Checks to see if the specified employeeID has access to the specified company. 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public bool AssertEmployeeCompanyAccess(int? employeeID, int? companyID)
        {
            string sql = "SELECT * FROM employee WHERE  employeeID = @p1 AND CompanyId = IFNULL(@p2, CompanyID)";
            Dictionary<string, object> dicParms = new Dictionary<string, object>();
            dicParms.Add("@p1", employeeID.GetValueOrDefault());
            dicParms.Add("@p2", companyID.GetValueOrDefault());

            System.Data.DataTable dt = DB.ExecuteDataTable(sql, dicParms);
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// Checks to see if the current session (User or Employee) has access to the specifed company.
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>

        public bool AssertCompanyAccess(int? companyID)
        {
            if (WebSession == null)
            {
                return true;
            }
            if (WebSession.Type == DataEntities.LookupData.SessionType.User)
            {
                return AssertUserCompanyAccess(companyID);
            }
            if (WebSession.Type == DataEntities.LookupData.SessionType.Employee)
            {
                return AssertEmployeeCompanyAccess(companyID);
            }
            return false;
        }

    }
}
