using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OOL_API.Data;
using OOL_API.Models;
using OOL_API.Services;
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
            services.AddControllers();

            ConfigureDatabase(services);

            ConfigureCors(services);

            ConfigureSwagger(services);

            ConfigurePictureStorage(services);

            ConfigureJwt(services);

            ConfigureSettings(services);
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
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
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
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

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