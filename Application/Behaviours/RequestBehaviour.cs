using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Behaviours
{
    public class RequestBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<RequestBehaviour<TRequest, TResponse>> _logger;
        public RequestBehaviour(ILogger<RequestBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            string requestResponse = $"\n\tRequest: {typeof(TRequest).Name}\n\n\t" +
                                     $"Propeties\n";

            Type typeRequest = request.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(typeRequest.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(request, null);
                requestResponse+= $"\t\t{prop.Name} : {propValue}\n";
            }
            var response = await next();
            //Response
            _logger.LogInformation(requestResponse);
            return response;
        }
    }

}
