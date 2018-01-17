using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NackademinUppgift07.Models;

namespace NackademinUppgift07
{
    public class Startup
    {
		private IConfiguration Configuration { get; }

	    public Startup(IConfiguration configuration)
	    {
		    Configuration = configuration;
	    }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddMvc();

	        services.AddDbContext<TomasosContext>(options =>
		        options.UseSqlServer(Configuration.GetConnectionString("Tomasos")));

	        services.AddSession();

	        services.AddDistributedMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

	        app.UseSession();

	        app.UseStaticFiles();

	        app.UseMvc(router =>
			{

				router.MapRoute("default", "{action}", new
		        {
			        controller = "Tomasos",
		        });

				router.MapRoute("maträtt", "{beskrivning?}", new
				{
					controller = "Tomasos",
					action = "Index",
				});
	        });
        }
    }
}
