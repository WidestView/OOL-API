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

        public bool ResetDatabaseOnBoot { get; }

        public string DefaultUserPassword { get; }
        public string DefaultUserLogin { get; }
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

            DefaultUserLogin = configuration["DefaultUser:Login"]
                               ?? throw Missing("DefaultUser.Login");

            DefaultUserPassword = configuration["DefaultUser:Password"]
                                  ?? throw Missing("DefaultUser.Password");
        }

        public string JwtIssuer { get; }
        public string JwtKey { get; }
        public string[] AllowedCorsUrls { get; }
        public string DefaultConnectionString { get; }

        public bool ResetDatabaseOnBoot { get; }

        public string DefaultUserPassword { get; }

        public string DefaultUserLogin { get; }


        private KeyNotFoundException Missing(string key)
        {
            return new KeyNotFoundException($"Missing {key} in appsettings.json");
        }
    }
}