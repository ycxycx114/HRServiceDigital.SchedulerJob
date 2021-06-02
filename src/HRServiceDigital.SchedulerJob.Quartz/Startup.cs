using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz
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

            services.AddScoped<IDbConnection>(db => new SqlConnection(Configuration.GetConnectionString("QuartzDb")));

            // base configuration from appsettings.json
            services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
            // if you are using persistent job store, you might want to alter some options
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true; // default: false
                options.Scheduling.OverWriteExistingData = true; // default: true
            });

            // see Quartz.Extensions.DependencyInjection documentation about how to configure different configuration aspects
            services.AddQuartz(q =>
                {
                    // your configuration here

                    q.UseMicrosoftDependencyInjectionJobFactory();
                    
                    q.UseSimpleTypeLoader();

                    q.UsePersistentStore(config =>
                    {
                        config.UseProperties = true;
                        config.RetryInterval = TimeSpan.FromSeconds(15);
                        config.UseSqlServer(sqlServer =>
                        {
                            sqlServer.ConnectionString = Configuration.GetConnectionString("QuartzDb");
                        });
                        config.UseJsonSerializer();
                    });

                    q.UseDedicatedThreadPool(tp => tp.MaxConcurrency = 10);
                });

            // Quartz.Extensions.Hosting hosting
            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("quartz", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Quartz Scheduler Job",
                    Description = "Quartz schedule job WebApi platform."
                });
            });

            Bootstrap.ConfigMap();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("Index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseRouting();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("quartz/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
