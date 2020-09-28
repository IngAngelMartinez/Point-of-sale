using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public Response(string message, object errors = null)
        {
            Succeeded = false;
            Message = message;
            Errors = errors;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public object Errors  { get; set; }
        public T Data         { get; set; }
    }
}
