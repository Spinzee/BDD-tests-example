using Data;
using Infrastructure;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

namespace WebSite
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
            services.AddControllersWithViews();

            // use option pattern to define a custom type that hold your configuration settings (statically typed)
            // while being restricted to only your actual relevant configuration.
            services.Configure<DatabaseOptions>(Configuration.GetSection("ConnectionStrings")); 
            
            
            //Transient lifetime services(AddTransient) are created each time they're requested from the service container.
            //Scoped lifetime services(AddScoped) are created once per client request(connection).
            //Singleton lifetime services(AddSingleton) are created the first time they're requested 
            // Register your services as transient wherever possible.
            // You generally don’t care about multi-threading and memory leaks and you know the service has a short life
            // if using singleton must consider lifetime of injected services to singleton service
            // and you need to deal with multi-threading and potential memory leak problems (as they are not disposed of until end of application)
            // Singleton services are generally designed to keep an application state. A cache is a good example of application states

            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IProfileAPI, ProfileAPI>();
            services.AddTransient<IProductService, ProductsService>();
            services.AddTransient<IProfileRepository, ProfileRepository>();
            services.AddTransient<IPasswordService, PasswordService>();
        }
                
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
