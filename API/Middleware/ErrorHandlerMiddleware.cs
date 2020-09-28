using Application.Wrappers;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Stopwatch time;
        private readonly ILogger<Response<int>> logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<Response<int>> logger)
        {
            this.next = next;
            this.time = new Stopwatch();
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                time.Start();

                await next(context);

            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case ApiException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message += "\n" + string.Join("\n", e.Errors);
                        break;
                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = $"Error, contact Admin. {DateTime.Now}";
                        responseModel.Errors = error.Message;
                        logger.LogError(error.StackTrace.ToString());

                        break;
                }
                var result = JsonSerializer.Serialize(responseModel);

                await response.WriteAsync(result);
            }
            finally 
            {
                time.Stop();

                var request = context.Request;

                string logInfo = $"\n\tMethod: {request.Method}\n\t" +
                                 $"Source: {request.Scheme}/{request.Host.Value}{request.Path.Value}{request.QueryString.Value}\n\t" +
                                 $"Time: {time.ElapsedMilliseconds} ms\n\t";

                time.Reset();

                logger.LogInformation(logInfo);
            }
            
        }

    }

    
}
