using microservice_authentication__api.src.Application.Common.Response;
using microservice_authentication__api.src.Application.Interfaces;
using RestSharp;

namespace microservice_authentication__api.src.Infrastructure.External.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly string url = Environment.GetEnvironmentVariable("NOTIFICATION_URL")
                         ?? throw new InvalidOperationException("A variável de ambiente 'NOTIFICATION_URL' não foi definida.");
        public async Task<Result<string>> SendSmsAsync(string to, string body)
        {
            try
            {

                var client = new RestClient(url);
                var request = new RestRequest("", Method.Post);
                request.AddJsonBody(new
                {
                    to,
                    body
                });
                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return Result<string>.Failure(500, "Erro",
                        $"Failed to fetch data from external API {response.Content}"
                    );
                }

                var content = response.Content;
                return Result<string>.Success(content, 200, "Sucesso");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(500, ex.Message);
            }
        }
    }
}