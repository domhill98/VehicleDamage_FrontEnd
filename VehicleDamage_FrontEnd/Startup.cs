using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleDamage_FrontEnd.Services.BEService;
using VehicleDamage_FrontEnd.Services.BlobService;
using VehicleDamage_FrontEnd.Services.DamageService;

namespace VehicleDamage_FrontEnd
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;


        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpClient("RetryConnection")
                ;

            if (_env.IsDevelopment())
            {
                //services.AddTransient<IDamageService, FakeDamageService>();
                //Uncomment this to use the live ML back end in dev mode
                services.AddScoped<IDamageService>(s =>
                {
                    var httpClientFactory = s.GetRequiredService<IHttpClientFactory>();
                    return new DamageService(httpClientFactory.CreateClient("RetryConnection"));
                });


                services.AddTransient<IBEService, FakeBEService>();
                _ = services.AddTransient<IBlobService, FakeBlobService>();

            }
            else
            {
                services.AddScoped<IDamageService>(s =>
                {
                    var httpClientFactory = s.GetRequiredService<IHttpClientFactory>();
                    return new DamageService(httpClientFactory.CreateClient("RetryConnection"));
                });

                //services.AddScoped<IBEService>(s =>
                //{
                //    var httpClientFactory = s.GetRequiredService<IHttpClientFactory>();
                //    return new BEService(httpClientFactory.CreateClient("RetryConnection"));
                //});

                //services.AddScoped<IBlobService>(s =>
                //{
                //    var httpClientFactory = s.GetRequiredService<IHttpClientFactory>();
                //    return new BlobService(httpClientFactory.CreateClient("RetryConnection"));
                //});



            }

        }

        private int FakeDamageService()
        {
            throw new NotImplementedException();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
