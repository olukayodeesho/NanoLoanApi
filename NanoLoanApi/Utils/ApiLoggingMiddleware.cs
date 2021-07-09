using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json;

namespace NanoLoanApi.Utils
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var originalBodyStream = httpContext.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                try
                {
                    //_apiLogService = apiLogService;
                    var request = httpContext.Request;
                    if (request.Path.StartsWithSegments(new PathString("/loan")) || request.Path.StartsWithSegments(new PathString("/quickteller")))
                    {
                        var stopWatch = Stopwatch.StartNew();
                        var requestTime = DateTime.UtcNow;
                        var requestBodyContent = await ReadRequestBody(request);
                        //var originalBodyStream = httpContext.Response.Body;
                        //using (var responseBody = new MemoryStream())
                        //{
                        var response = httpContext.Response;
                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);
                        await responseBody.CopyToAsync(originalBodyStream);

                        SafeLog(requestTime,
                            stopWatch.ElapsedMilliseconds,
                            response.StatusCode,
                            request.Method,
                            request.Host + request.Path,
                            request.QueryString.ToString(),
                            requestBodyContent,
                            responseBodyContent);
                        //}
                    }
                    else
                    {
                        await _next(httpContext);
                    }
                }
                catch (Exception ex)
                {
                    var errorGuid = Guid.NewGuid().ToString();
                    TrapException(ex, httpContext, errorGuid);

                    var response = httpContext.Response;
                    response.ContentType = "application/json";

                    switch (ex)
                    {
                        //case BusinessException:
                        //    response.StatusCode = (int)HttpStatusCode.BadRequest;
                        //    break;
                        default:
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            break;
                    }

                    var errorResponse = new
                    {
                        message = "An unexpected exception has occurred. Please contact support with this ID : " + errorGuid,
                        statusCode = response.StatusCode,
                        status = "error"

                    };

                    var errorJson = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                    response.ContentType = "application/json";
                    await response.WriteAsync(errorJson, Encoding.UTF8);
                    response.Body.Seek(0, SeekOrigin.Begin);    //IMPORTANT!
                    await responseBody.CopyToAsync(originalBodyStream); //IMPORTANT!
                    return;
                }
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private void SafeLog(DateTime requestTime,
                            long responseMillis,
                            int statusCode,
                            string method,
                            string path,
                            string queryString,
                            string requestBody,
                            string responseBody)
        {
           

             Utils.Util.LogMessage(JsonConvert.SerializeObject(new RequestResponseLog
            {
                RequestTime = requestTime,
                ResponseInMillis = responseMillis,
                StatusCode = statusCode,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                ResponseBody = responseBody
            }));
        }

        private void TrapException(Exception ex, HttpContext httpContext, string errorRetrievalId)
        {
            ex = ex.GetOriginalException();
            string userAuth = "";
            string requestorUserName = "";
            try { userAuth = httpContext.Request.HttpContext.User.Identity.IsAuthenticated.ToString(); } catch { userAuth = "Retrieval Attempt Failed"; }
            try { requestorUserName = httpContext.Request.HttpContext.User.Identity.Name.ToString(); } catch { requestorUserName = "Retrieval Attempt Failed"; }

            Utils.Util.LogError(JsonConvert.SerializeObject(new GlobalErrorLog
            {
                DateCreated = DateTime.Now,
                ExceptionSource = ex.Source,
                ExceptionType = ex.GetType().ToString(),
                ExceptionUrl = httpContext.Request.Path.ToString(),
                Message = ex.Message,
                ExceptionStackTrace = ex.StackTrace,
                UniqueRetrievalId = errorRetrievalId,
                QueryString = httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : "",
                ServerName = httpContext.Request.Host.HasValue ? httpContext.Request.Host.Value : "",
                UserHostName = httpContext.Request.Host.HasValue ? httpContext.Request.Host.Value : "",
                UserIp = httpContext.Request.Host.HasValue ? httpContext.Request.Host.Host : "",
                UserAuthenticated = userAuth,
                UserName = requestorUserName
            }));
        }
    }


    public static class ExceptionExtensions
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {

        }
    }
    public class RequestResponseLog
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string QueryString { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? RequestTime { get; set; }
        public long? ResponseInMillis { get; set; }
    }
    public class GlobalErrorLog
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionSource { get; set; }
        public string ExceptionUrl { get; set; }
        public DateTime? DateCreated { get; set; }
        public string UniqueRetrievalId { get; set; }
        public string QueryString { get; set; }
        public string ServerName { get; set; }
        public string UserIp { get; set; }
        public string UserHostName { get; set; }
        public string UserAuthenticated { get; set; }
        public string UserName { get; set; }
        public string ExceptionStackTrace { get; set; }
    }
}
