using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaaS.Services.Example.Api.Storage.Persistence.Dao;
using TaaS.Services.Example.Api.Storage.Persistence.Database;
using TaaS.Services.Example.Api.Storage.Persistence.Query;

namespace TaaS.Services.Example.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddDbContext<PrintersDatabaseContext>(
                    options => options.UseInMemoryDatabase("Main"));
            //options => options.UseSqlServer(_configuration.GetConnectionString("PrinterDatabase")));

            services.AddScoped<IPrintersDao, PrintersDao>();
            services.AddScoped<IGetPreviewPrintersQuery, GetPreviewPrintersQuery>();
            services.AddAutoMapper(typeof(Startup).Assembly);
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}