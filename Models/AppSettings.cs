using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace OOL_API.Models
{
#nullable enable
    public interface IAppSettings
    {
        public string JwtIssuer { get; }

        public string JwtKey { get; }

        public string[] AllowedCorsUrls { get; }

        public string DefaultConnectionString { get; }
    }

    public class AppSettings : IAppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            JwtIssuer = configuration["Jwt:Issuer"] ??
                        throw Missing("Jwt.Issuer");

            JwtKey = configuration["Jwt:Key"] ??
                     throw Missing("Jwt.Key");

            AllowedCorsUrls = configuration.GetSection("AllowedCorsUrls")
                                  ?.Get<List<string>>()
                                  ?.ToArray()
                              ?? throw Missing("AllowedCorsUrls");


            DefaultConnectionString = configuration.GetConnectionString("DefaultConnection")
                                      ?? throw Missing("AllowedCorsUrls");
        }

        public string JwtIssuer { get; }
        public string JwtKey { get; }
        public string[] AllowedCorsUrls { get; }
        public string DefaultConnectionString { get; }

        private KeyNotFoundException Missing(string key)
        {
            return new KeyNotFoundException($"Missing {key} in appsettings.json");
        }
    }
}