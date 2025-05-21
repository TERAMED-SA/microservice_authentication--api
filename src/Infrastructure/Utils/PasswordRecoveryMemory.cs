using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Infrastructure.Utils
{
    public static class PasswordRecoveryMemory
    {
        // Armazena username => (Código gerado, Token do Identity, Expiração)
        private static readonly Dictionary<string, (string Code, string Token, DateTime Expires)> _codes = new();

        public static void Store(string username, string code, string token, int minutes = 10)
        {
            _codes[username] = (code, token, DateTime.UtcNow.AddMinutes(minutes));
        }

        public static (string Token, bool Valid) Validate(string username, string code)
        {
            if (_codes.TryGetValue(username, out var entry))
            {
                if (entry.Code == code && entry.Expires > DateTime.UtcNow)
                {
                    _codes.Remove(username); // remove após uso
                    return (entry.Token, true);
                }
            }
            return (null, false);
        }
    }
}