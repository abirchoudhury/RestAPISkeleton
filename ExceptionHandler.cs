using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProject
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {

        public override void OnException(ExceptionContext context)
        {
            if (!context.HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            }
            var exception = context.Exception;
            if (exception is System.UnauthorizedAccessException)
            {
                Logging.LogMessage(System.Reflection.MethodBase.GetCurrentMethod(), Logging.LogLevel.Error, "Unauthorized access exception: " + exception.Message);
            }
            else
            {
                Logging.LogException(System.Reflection.MethodBase.GetCurrentMethod(), Logging.LogLevel.Error, exception);
            }
            if (exception is UnauthorizedAccessException)
            {
                context.Result = new ContentResult
                {
                    Content = $"{exception.ConvertToAPIResponseV1().ToString()}",
                    ContentType = "application/json",
                    // change to whatever status code you want to send out
                    StatusCode = (int?)HttpStatusCode.Unauthorized
                };
            }
            else if (exception is ArgumentNullException || exception is ArgumentOutOfRangeException || exception is NullReferenceException || exception is NotImplementedException || exception is OperationCanceledException || exception is BusinessException)
            {
                context.Result = new ContentResult
                {
                    Content = $"{exception.ConvertToAPIResponseV1().ToString()}",
                    ContentType = "application/json",
                    // change to whatever status code you want to send out
                    StatusCode = (int?)HttpStatusCode.BadRequest
                };
            }
            else
            {
                context.Result = new ContentResult
                {
                    Content = $"{exception.ConvertToAPIResponseV1().ToString()}",
                    ContentType = "application/json",
                    // change to whatever status code you want to send out
                    StatusCode = (int?)HttpStatusCode.NotImplemented
                };
            }
        }
    }
}
