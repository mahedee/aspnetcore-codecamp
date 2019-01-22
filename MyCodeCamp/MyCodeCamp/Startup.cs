using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

            services.AddDbContext<EFDataContext>(ServiceLifetime.Scoped);
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddTransient<CampDbInitializer>();
            services.AddTransient<CampIdentityInitializer>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper();

            //Configure Identity
            services.AddIdentity<CampUser, IdentityRole>()
                .AddEntityFrameworkStores<EFDataContext>()
            .AddDefaultTokenProviders();



            //Token based authentication
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;
                option.RequireHttpsMetadata = false;
                option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Tokens:Audience"],
                    ValidIssuer = Configuration["Tokens:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                };
            });


            

            services.AddMvc(opt =>
            {
                if (!_env.IsProduction())
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
            services.AddCors(cfg =>
            {
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




            /*
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


                */


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



            //Token based authentication
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();

            app.UseMvc(config =>
            {
            });

            seeder.Seed().Wait();
            identitySeeder.Seed().Wait();





            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }
    }
}
