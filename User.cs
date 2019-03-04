using System.Collections.Generic;
using System.Data;
using RESTfool.DALImplementation;
using RESTfool.DBManager;

namespace DALImplementation
{
    public class User : DALBase
    {
        #region DALBase
        public User(IDBManager db)
        {
            base.DB = db;
        }

        public User(string connectionString)
        {
            base.CreateDBManager(connectionString);
            DB.RequestingClass = this.GetType().ToString();
        }
        #endregion

        #region User
        /// <summary>
        /// Returns a list of all users available. 
        /// </summary>
        /// <returns></returns>
        public List<DataEntities.User> GetAllUsers()
        {
            string sql = "SELECT * FROM Users";
            Dictionary<string, object> dicParms = new Dictionary<string, object>();
            var lst = new List<DataEntities.User>();
            DataTable dt = DB.ExecuteDataTable(sql, dicParms);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var get = DataEntities.User.Parse(dr);
                    lst.Add(get);
                }
            }

            return lst;
        }

        /// <summary>
        /// Returns a user by ID.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public DataEntities.User GetUserByID(int userID)
        {
            string sql = @"SELECT * FROM Users WHERE UserID = @P1";
            Dictionary<string, object> dicParms = new Dictionary<string, object>();
            dicParms.Add("@P1", userID);
            var user = new DataEntities.User();
            DataTable dt = DB.ExecuteDataTable(sql, dicParms);
            if (dt != null && dt.Rows.Count == 1)
            {
                user = DataEntities.User.Parse(dt.Rows[0]);
                return user;
            }
            return null;
        }

        /// <summary>
        /// Saves or updates the specified user.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public int SaveUser(DataEntities.User d)
        {
            string sql = "";
            Dictionary<string, object> dicParms = new Dictionary<string, object>();


            dicParms.Add("@UserName", d.UserName);
            dicParms.Add("@UserPassword", d.UserPassword);
            dicParms.Add("@Name", d.Name);
            dicParms.Add("@Email", d.Email);
            dicParms.Add("@Phone", d.Phone);

            if (!d.UserID.HasValue)
            {
                sql = @"
                        INSERT INTO Users (UserID, UserName, UserPassword, Namee, Email, Phone)
                        VALUES (@UserID, @UserName, @UserPassword, @Namee, @Email, @Phone)
                       ";
                return DB.ExecuteNonQueryReturnLastID(sql, dicParms);
            }
            else
            {
                dicParms.Add("@UserID", d.UserID);
                sql = @"
                        UPDATE Users SET UserID=@UserID, UserName=@UserName, UserPassword=@UserPassword, Name=@Name, Email=@Email, Phone=@Phone
                        WHERE UserID = @UserID
                       ";
                return DB.ExecuteNonQueryReturnNumRows(sql, dicParms);
            }
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int DeleteUser(int userID)
        {
            string sql = "DELETE FROM User WHERE UserID=@P1";
            Dictionary<string, object> dicParms = new Dictionary<string, object>();
            dicParms.Add("@P1", userID);
            return DB.ExecuteNonQueryReturnNumRows(sql, dicParms);
        }

        #endregion


    }
}





