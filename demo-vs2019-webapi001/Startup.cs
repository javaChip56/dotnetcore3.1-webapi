using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

using demo_vs2019_webapi001.Models;
using demo_vs2019_webapi001.Queries;
using demo_vs2019_webapi001.Queries.Interfaces;

namespace demo_vs2019_webapi001
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
            services.AddControllers();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ClientDBContext>(options =>
                {
                    options.UseSqlServer(Configuration["ConnectionStrings:ClientDB"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                },
                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            services.AddTransient<IClientQueries>(sp =>
            {
                var context = sp.GetRequiredService<ClientDBContext>();
                return new ClientQueries(context);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    Configuration["SwaggerConfig:FriendlyName"] + Configuration["SwaggerConfig:Version"],
                    new OpenApiInfo
                    {
                        Title = Configuration["SwaggerConfig:Title"],
                        Version = Configuration["SwaggerConfig:Version"]
                    }
                        );
            });

            services.AddHealthChecks().AddCheck("Client Database", new Diagnostics.DatabaseHealthCheck(Configuration["ConnectionStrings:ClientDB"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    string.Format(
                        Configuration["SwaggerConfig:Endpoint"],
                        Configuration["SwaggerConfig:FriendlyName"] + Configuration["SwaggerConfig:Version"]),
                        Configuration["SwaggerConfig:Title"] + " " + Configuration["SwaggerConfig:Version"]);
            });

            app.UseHealthChecks(path: "/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
