
using microservice_authentication__api.src.Application.Common.Response;

namespace microservice_authentication__api.src.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result<string>> SendSmsAsync(string to, string body);
    }
}