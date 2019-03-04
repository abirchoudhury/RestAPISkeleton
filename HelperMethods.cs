using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProject
{
    public static class HelperMethods
    {
        public static string GetConnectionString(string authKey)
        {
            var connectionStringRow = System.Configuration.ConfigurationManager.ConnectionStrings[authKey];
            if (connectionStringRow == null || string.IsNullOrEmpty(connectionStringRow.ConnectionString))
            {
                throw new UnauthorizedAccessException("Connection string not found");
            }
            return connectionStringRow.ConnectionString;
        }

        public static Models.APIResponseV1<string> ConvertToAPIResponseV1(this Exception ex)
        {
            BASIC.TAP.RestService.Models.APIResponseV1<string> d = new TAP.RestService.Models.APIResponseV1<string>();
            d.data = ex.Message;
            d.status = "error";
            return d;
        }
    }
}