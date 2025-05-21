using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using microservice_authentication__api.src.Application.Common.Response;

namespace microservice_authentication__api.src.Application.Interfaces
{
    public interface IExternalApi
    {
        Task<Result<object>> GetExternalApiAsync(string apiUrl, string token);
        Task<Result<object>> PostExternalApiAsync(string apiUrl, object payload, string token);
    }
}