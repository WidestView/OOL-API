using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Services;
using QRCoder;
using static OOL_API.Services.PictureStorageFactory;

namespace OOL_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Settings = new AppSettings(configuration);
        }

        // public IConfiguration Configuration { get; }

        public IAppSettings Settings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureControllers(services);

            ConfigureDatabase(services);

            ConfigureCors(services);

            ConfigureSwagger(services);

            ConfigurePictureStorage(services);

            ConfigureJwt(services);

            ConfigureSettings(services);

            ConfigureHash(services);

            ConfigureAuth(services);

            ConfigureQR(services);
        }

        private void ConfigureQR(IServiceCollection services)
        {
            services.AddScoped<QRCodeGenerator>();
            services.AddScoped<QrHandler>();
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<CurrentUserInfo, CurrentUserInfo>();
        }

        private void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                if (Settings.RequireAuth)
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    config.Filters.Add(new AuthorizeFilter(policy));
                }
            });
        }

        private void ConfigureHash(IServiceCollection services)
        {
            services.AddScoped<IPasswordHash, Sha256PasswordHash>();
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            services.AddSingleton(Settings);
        }

        private void ConfigureJwt(IServiceCollection services)
        {
            var issuer = Settings.JwtIssuer;

            var key = Settings.JwtKey;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = issuer,
                            ValidAudience = issuer,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(key)
                            )
                        };
                    }
                );
        }

        private static void ConfigurePictureStorage(IServiceCollection services)
        {
            services.AddScoped<IPictureStorageDelegate, PictureStorageDelegate>();

            services.AddScoped(StorageOf<PhotoShootImage, Guid>(
                "Photoshoot_Images",
                img => img.Id
            ));

            services.AddScoped(StorageOf<Package, int>(
                "Package_Images",
                pkg => pkg.Id
            ));

            services.AddScoped(StorageOf<EquipmentDetails, int>(
                "EquipmentDetails_Images",
                details => details.Id
            ));

            services.AddScoped(
                StorageOf<User, string>(
                    "User_Images",
                    employee => employee.Cpf
                )
            );
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "OutOfLens API",
                    Description = "An API for fetching data to OutOfLens applications",
                    TermsOfService = new Uri("https://github.com/WidestView/OOL-API/blob/main/README.md"),
                    License = new OpenApiLicense
                    {
                        Name = "Use under GPL-v3",
                        Url = new Uri("https://github.com/WidestView/OOL-API/blob/main/LICENSE")
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Jwt Authorization Header"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                // allows nested classes to have the same name
                // but different schema ids
                c.CustomSchemaIds(type => (type.DeclaringType?.Name ?? "") + type.Name);

                c.EnableAnnotations();
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddScoped<DbInitializer>();

            services.AddDbContext<StudioContext>(options =>
                options.UseMySQL(Settings.DefaultConnectionString));
        }

        private void ConfigureCors(IServiceCollection services)
        {
            var allowedUrls = Settings.AllowedCorsUrls;

            services.AddCors(options => options.AddPolicy("ApiCorsPolicy",
                builder => { builder.WithOrigins(allowedUrls).AllowAnyMethod().AllowAnyHeader(); }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("ApiCorsPolicy");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OOL-API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
