using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CustomersAPIServices.Models;
using Microsoft.EntityFrameworkCore;


namespace CustomersAPIServices
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
            services.AddMvc();

            //Добавить вручную
            //var connection = @"Server=tcp:rsoi.database.windows.net,1433;Initial Catalog=Customers;Persist Security Info=False;User ID=pkiselev;Password=\vp~{\Ah2q!jSt6d;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var connection = @"server=46.254.21.136; port=3306; database=p460741_lab; user=p460741_pavel; password=2M8p8B0c; charset=utf8";
            //services.AddDbContext<CustomersContext>(options => options.UseSqlServer(connection));
            services.AddDbContext<CustomersContext>(options => options.UseMySQL(connection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
