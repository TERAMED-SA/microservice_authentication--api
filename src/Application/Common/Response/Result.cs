using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Application.Common.Response
{
    public class Result<T>
    {
        public bool Error { get; private set; }
        public int Code { get; private set; }
        public string? Description { get; private set; }
        public string? Message { get; private set; }
        public T? Data { get; private set; }

        public static Result<T> Success(T? data, int code, string message, string? description = null) => new()
        {
            Error = false,
            Code = code,
            Message = message,
            Description = description,
            Data = data
        };

        public static Result<T> Failure(int code, string message, string? description = null) => new()
        {
            Error = true,
            Code = code,
            Message = message,
            Description = description,
            Data = default
        };
    }
}