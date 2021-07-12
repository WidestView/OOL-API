using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OOL_API.Data;
using OOL_API.Services;

namespace OOL_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDatabase(services);

            ConfigureCors(services);

            ConfigureSwagger(services);
            
            services.AddControllers();
            
            services.AddScoped<PhotoShootPictureStorage, PhotoShootPictureStorage>();
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
                        Url = new Uri("https://github.com/WidestView/OOL-API/blob/main/LICENSE"),
                    }
                });
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<StudioContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
        }

        private void ConfigureCors(IServiceCollection services)
        {
            string[] allowedUrls = Configuration.GetSection("AllowedCorsUrls").Get<List<string>>().ToArray();

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
