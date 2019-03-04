using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class UserCompanyAccess
    {
        public UserCompanyAccess()
        {

        }

        #region Public Properties
        public int? UserID
        {
            get; set;
        }

        public int? CompanyID
        {
            get; set;
        }

        public bool? Active
        {
            get; set;
        }

        #endregion

        public static UserCompanyAccess Parse(System.Data.DataRow dr)
        {
            UserCompanyAccess data = new UserCompanyAccess();

            if (dr["UserID"] != System.DBNull.Value) data.UserID = int.Parse(dr["UserID"].ToString());
            if (dr["CompanyID"] != System.DBNull.Value) data.CompanyID = int.Parse(dr["CompanyID"].ToString());
            if (IsValidBool(dr["Active"]))
            {
                if (dr["Active"] != System.DBNull.Value) data.Active = bool.Parse(dr["Active"].ToString());
            }

            return data;
        }

        public static bool IsValidBool(object b)
        {
            bool flag;
            if (bool.TryParse(b.ToString(), out flag))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
