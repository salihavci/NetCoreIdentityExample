using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreIdentityExample.Helpers.ClaimProvider;
using NetCoreIdentityExample.Helpers.RequirementProvider;
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

            //-------------------------------Add Policy And Claims-----------------------------
            services.AddAuthorization(m => {
                m.AddPolicy("BursaPolicy", p => p.RequireClaim("City","Bursa")); //Sadece City yazarak t�m City'leri kabul etmi� olursun.
                m.AddPolicy("ViolancePolicy", p => p.RequireClaim("Violance"));
                m.AddPolicy("ExchangePolicy", p => p.AddRequirements(new ExpireDateExchangeRequirement())); //30 g�nl�k zorunlulu�u eklemi� olduk.
            });


            //------------------------------Add Authentication Methods-------------------------

            services.AddAuthentication().AddFacebook(m=> {
                m.AppId = Configuration["Authentication:Facebook:AppId"];
                m.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            }).AddGoogle(p=> {
                p.ClientId = Configuration["Authentication:Google:AppId"];
                p.ClientSecret = Configuration["Authentication:Google:AppSecret"];
            }).AddMicrosoftAccount(o=> { 
                o.ClientId = Configuration["Authentication:Microsoft:AppId"];
                o.ClientSecret = Configuration["Authentication:Microsoft:AppSecret"];
            });

            //------------------------------Add Policy Handle ---------------------------------
            services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();

            //-------------------------------Identity Options----------------------------------
            services.AddIdentity<AppUser,AppRole>(m=> {
                m.Password.RequiredLength = 8;
                m.Password.RequireDigit = false;
                m.Password.RequireNonAlphanumeric = false;
                m.Password.RequireLowercase = false;
                m.Password.RequireUppercase = false;
                m.Password.RequiredUniqueChars = 0;
                m.User.RequireUniqueEmail = true;
                m.User.AllowedUserNameCharacters = "abc�defg�h�ijklmno�pqrs�tu�vwxyzABC�DEFG�HI�JKLMNO�PQRS�TU�VWXYZ0123456789-._";
            }).AddEntityFrameworkStores<AppIdentityDbContext>().AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>().AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddDefaultTokenProviders();
            //------------------------------Add Cookie-----------------------------------------
            CookieBuilder cookieBuilder = new CookieBuilder();
            cookieBuilder.Name = "LoggedUser";
            cookieBuilder.HttpOnly = false;
            //cookieBuilder.Expiration = System.TimeSpan.FromDays(60); //.Net Core 3.1'de �al���yor
            //cookieBuilder.SameSite = SameSiteMode.Strict; //Sadece sistem �zerindeki cookieleri kabul et (Kendi sitemiz i�in)
            cookieBuilder.SameSite = SameSiteMode.Lax;
            cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Always = E�er istek HTTPS'den geldiyse cookie bilgisini HTTPS �zerinden g�nderir. SameAsRequest = �stek nerden geldiyse oradan g�nderir. None = �stek nerden gelirse gelsin HTTP'ye g�nderir
            services.ConfigureApplicationCookie(m =>
            {
                m.LoginPath = new PathString("/Home/Login");
                m.LogoutPath = new PathString("/Member/Logout");
                m.AccessDeniedPath = new PathString("/Member/AccessDenied"); //Kullan�c�n�n kendi rol� d���ndaki roldeki sayfaya eri�meye �al��t���nda verece�i hatan�n sayfas�
                m.Cookie = cookieBuilder;
                m.SlidingExpiration = true; //E�er true olursa tekrar cookie s�resi uzayacak.
                m.ExpireTimeSpan = TimeSpan.FromDays(60);
            });

            //----------------------------Dependency Injection---------------------------------
            services.AddScoped<IClaimsTransformation, ClaimProvider>(); //Her request'te bu nesneyi olu�tur. (Transient = Her kar��la�t���nda olu�tur, Singleton = Uygulama bir kere aya�a kalkt��� zaman olut�tur ve lifecycle dolana kadar kals�n)
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
