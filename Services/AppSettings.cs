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

        public string DefaultUserCpf { get; }

        public string DefaultUserEmail { get; }

        public string SuperUserCpf { get; }

        public string SuperUserEmail { get; }

        public string SuperUserPassword { get; }

        public bool RequireAuth { get; }
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

            DefaultUserCpf = configuration["DefaultUser:Login"]
                             ?? throw Missing("DefaultUser.Login");

            DefaultUserEmail = configuration["DefaultUser:Email"]
                               ?? throw Missing("DefaultUser.Email");

            DefaultUserPassword = configuration["DefaultUser:Password"]
                                  ?? throw Missing("DefaultUser.Password");

            RequireAuth = bool.Parse(configuration["RequireAuth"] ?? "false");

            SuperUserCpf = configuration["SudoUser:Login"]
                           ?? throw Missing("SudoUser.Login");

            SuperUserEmail = configuration["SudoUser:Email"]
                             ?? throw Missing("SudoUser:Email");

            SuperUserPassword = configuration["SudoUser:Password"]
                                ?? throw Missing("SudoUser.Password");
        }

        public string JwtIssuer { get; }
        public string JwtKey { get; }
        public string[] AllowedCorsUrls { get; }
        public string DefaultConnectionString { get; }

        public bool ResetDatabaseOnBoot { get; }

        public string DefaultUserPassword { get; }

        public string DefaultUserEmail { get; }

        public string DefaultUserCpf { get; }

        public bool RequireAuth { get; }

        public string SuperUserCpf { get; }

        public string SuperUserEmail { get; }

        public string SuperUserPassword { get; }


        private KeyNotFoundException Missing(string key)
        {
            return new KeyNotFoundException($"Missing {key} in appsettings.json");
        }
    }
}