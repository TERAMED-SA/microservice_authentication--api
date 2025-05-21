using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using microservice_authentication__api.src.Application.Common.Response;
using microservice_authentication__api.src.Application.Interfaces;
using MySqlX.XDevAPI.Common;

namespace microservice_authentication__api.src.Infrastructure.External
{
    public class ExternalApiBase : IExternalApi
    {
        public async Task<Result<object>> GetExternalApiAsync(string apiUrl, string token)
        {
            try
            {
                using var client = new HttpClient();

                // Adiciona o token no header Authorization
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Result<object>.Failure(
                        400,
                        "Failed to fetch data from external API.",
                        errorContent
                    );
                }

                var content = await response.Content.ReadAsStringAsync();
                return Result<object>.Success(content, 200, "Sucesso");
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(
                    500,
                    "Failed to fetch data from external API.",
                    ex.Message
                );
            }
        }

        public async Task<Result<object>> PostExternalApiAsync(string apiUrl, object payload, string token)
        {
            try
            {
                using var client = new HttpClient();

                // Adiciona o token no header Authorization
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Converte o payload para JSON
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(apiUrl, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Result<object>.Failure(
                        400,
                        "Failed to send data to external API.",
                        errorContent
                    );
                }

                var content = await response.Content.ReadAsStringAsync();
                return Result<object>.Success(content, 200, "Sucesso");
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(
                    500,
                    "Failed to send data to external API.",
                    ex.Message
                );
            }
        }
    }
}