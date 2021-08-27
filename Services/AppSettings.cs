using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace OOL_API.Services
{
#nullable enable
    public interface IAppSettings
    {
        public string JwtIssuer { get; }

        public string JwtKey { get; }

        public string[] AllowedCorsUrls { get; }

        public string DefaultConnectionString { get; }

        public bool ResetDatabaseOnBoot { get; set; }
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

            ResetDatabaseOnBoot = bool.Parse(configuration["ResetDatabaseOnBoot"] ?? "true");
        }

        public string JwtIssuer { get; }
        public string JwtKey { get; }
        public string[] AllowedCorsUrls { get; }
        public string DefaultConnectionString { get; }

        public bool ResetDatabaseOnBoot { get; set; }

        private KeyNotFoundException Missing(string key)
        {
            return new KeyNotFoundException($"Missing {key} in appsettings.json");
        }
    }
}