//using log4net;
//using log4net.Config;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;


//namespace APIProject
//{
//    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
//    {

//        private static readonly log4net.ILog log
//       = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//        public override void OnException(ExceptionContext context)
//        {
//            if (!context.HttpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
//            {
//                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
//                context.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
//            }
//            var exception = context.Exception;
//            if (exception is System.UnauthorizedAccessException)
//            {
//                Logging.LogMessage(System.Reflection.MethodBase.GetCurrentMethod(), Logging.LogLevel.Error, "Unauthorized access exception: " + exception.Message);
//            }
//            else
//            {
//                Logging.LogException(System.Reflection.MethodBase.GetCurrentMethod(), Logging.LogLevel.Error, exception);
//            }
//            if (exception is UnauthorizedAccessException)
//            {
//                context.Result = new ContentResult
//                {
//                    Content = $"{exception.ConvertToAPIResponseV1().ToString()}",
//                    ContentType = "application/json",
//                    // change to whatever status code you want to send out
//                    StatusCode = (int?)HttpStatusCode.Unauthorized
//                };
//            }
//            else if (exception is ArgumentNullException || exception is ArgumentOutOfRangeException || exception is NullReferenceException || exception is NotImplementedException || exception is OperationCanceledException || exception is BusinessException)
//            {
//                context.Result = new ContentResult
//                {
//                    Content = $"{exception.ConvertToAPIResponseV1().ToString()}",
//                    ContentType = "application/json",
//                    // change to whatever status code you want to send out
//                    StatusCode = (int?)HttpStatusCode.BadRequest
//                };
//            }
//            else
//            {
//                context.Result = new ContentResult
//                {
//                    Content = $"{exception.ConvertToAPIResponseV1().ToString()}",
//                    ContentType = "application/json",
//                    // change to whatever status code you want to send out
//                    StatusCode = (int?)HttpStatusCode.NotImplemented
//                };
//            }
//        }
//    }
//}

//namespace Microsoft.AspNetCore.Mvc.Filters
//{
//    public interface IExceptionFilter : IFilterMetadata
//    {
//        void OnException(ExceptionContext context);
//    }
//}