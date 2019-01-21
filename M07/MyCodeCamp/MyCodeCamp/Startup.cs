using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Model;
using Newtonsoft.Json;

namespace MyCodeCamp
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt =>
            {
                if(!_env.IsProduction())
                {
                    opt.SslPort = 44388;
                }
                opt.Filters.Add(new RequireHttpsAttribute());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                  .AddJsonOptions(opt =>
                  {
                      opt.SerializerSettings.ReferenceLoopHandling =
                        ReferenceLoopHandling.Ignore;
                  });


            //For CORS
            services.AddCors(cfg => {
                cfg.AddPolicy("Wildermuth", bldr =>
                 {
                     bldr.AllowAnyHeader()
                     .AllowAnyMethod()
                     .WithOrigins("http://mahedee.net");

                 });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader()
                    .WithMethods("GET")
                    .AllowAnyOrigin();

                });
            });


            services.AddDbContext<EFDataContext>(ServiceLifetime.Scoped);
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddTransient<CampDbInitializer>();
            services.AddTransient<CampIdentityInitializer>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper();

            //Configure Identity
            services.AddIdentity<CampUser, IdentityRole>()
                .AddEntityFrameworkStores<EFDataContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                    {
                        context.Response.StatusCode = 401;
                    }

                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 200)
                    {
                        context.Response.StatusCode = 403;
                    }

                    return Task.CompletedTask;
                };
            });



            //services.Configure<IdentityOptions>(config =>
            //{
            //    config.Cookies.ApplicationCookie.Events =
            //      new CookieAuthenticationEvents()
            //      {
            //          OnRedirectToLogin = (ctx) =>
            //          {
            //              if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            //              {
            //                  ctx.Response.StatusCode = 401;
            //              }

            //              return Task.CompletedTask;
            //          },
            //          OnRedirectToAccessDenied = (ctx) =>
            //          {
            //              if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
            //              {
            //                  ctx.Response.StatusCode = 403;
            //              }

            //              return Task.CompletedTask;
            //          }
            //      };
            //});


            // services.AddScoped<SchoolContext>(_ => new SchoolContext(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            CampDbInitializer seeder,
            CampIdentityInitializer identitySeeder,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentity();

            app.UseMvc(config =>
            {
                //config.MapRoute("MainAPIRoute","api/{controller}/{action}");
            });

            //app.UseCors(cfg => {
            //    cfg.AllowAnyHeader()
            //    .AllowAnyMethod()
            //    .AllowAnyOrigin();
            //});
            //_env = env;

            //app.UseMvc();

            seeder.Seed().Wait();
            identitySeeder.Seed().Wait();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }
    }
}
