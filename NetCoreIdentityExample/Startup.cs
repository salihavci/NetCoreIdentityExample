using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreIdentityExample.Helpers.Validations;
using NetCoreIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreIdentityExample
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc();
            //-------------------------------Sql Server Connection-----------------------------
            services.AddDbContext<AppIdentityDbContext>(
                options=>options.UseSqlServer("name=ConnectionStrings:DefaultConnection").UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            //-------------------------------Identity Options----------------------------------
            services.AddIdentity<AppUser,AppRole>(m=> {
                m.Password.RequiredLength = 8;
                m.Password.RequireDigit = false;
                m.Password.RequireNonAlphanumeric = false;
                m.Password.RequireLowercase = false;
                m.Password.RequireUppercase = false;
                m.Password.RequiredUniqueChars = 0;
                m.User.RequireUniqueEmail = true;
                m.User.AllowedUserNameCharacters = "abcçdefgðhýijklmnoöpqrsþtuüvwxyzABCÇDEFGÐHIÝJKLMNOÖPQRSÞTUÜVWXYZ0123456789-._";
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>().AddErrorDescriber<CustomIdentityErrorDescriber>().AddDefaultTokenProviders();
            //------------------------------Add Cookie----------------------------------------
            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "LoggedUser";
            cookieBuilder.HttpOnly = false;
            //cookieBuilder.Expiration = System.TimeSpan.FromDays(60); //.Net Core 3.1'de çalýþýyor
            //cookieBuilder.SameSite = SameSiteMode.Strict; //Sadece sistem üzerindeki cookieleri kabul et (Kendi sitemiz için)
            cookieBuilder.SameSite = SameSiteMode.Lax;
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Always = Eðer istek HTTPS'den geldiyse cookie bilgisini HTTPS üzerinden gönderir. SameAsRequest = Ýstek nerden geldiyse oradan gönderir. None = Ýstek nerden gelirse gelsin HTTP'ye gönderir
            services.ConfigureApplicationCookie(m =>
            {
                m.LoginPath = new PathString("/Home/Login");
                m.LogoutPath = new PathString("/Member/Logout");
                m.AccessDeniedPath = new PathString("/Member/AccessDenied"); //Kullanýcýnýn kendi rolü dýþýndaki roldeki sayfaya eriþmeye çalýþtýðýnda vereceði hatanýn sayfasý
                m.Cookie = cookieBuilder;
                m.SlidingExpiration = true; //Eðer true olursa tekrar cookie süresi uzayacak.
                m.ExpireTimeSpan = TimeSpan.FromDays(60);
            });
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
            app.UseAuthentication();
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
