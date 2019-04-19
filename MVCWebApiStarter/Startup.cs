using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using MVCWebApiStarter.Data;
using MVCWebApiStarter.Repositories;

namespace MVCWebApiStarter
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .Build();
                    });
            });

            services.AddDbContext<AppDbContext>(
                options => options.UseSqlite(Configuration.GetConnectionString("DefaultConn"))
            );

            // https://nicolas.guelpa.me/blog/2017/01/11/dotnet-core-data-protection-keys-repository.html
            services.AddSingleton<IXmlRepository, DataProtectionKeyRepository>();
            var built = services.BuildServiceProvider();
            services.AddDataProtection()
                .AddKeyManagementOptions(options => options.XmlRepository = built.GetService<IXmlRepository>());

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication()
            .AddCookie(options =>
            {
                options.SlidingExpiration = true;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
#if DEBUG
                options.RequireHttpsMetadata = false; // For Dev Reasons
#else
                options.RequireHttpsMetadata = true;
#endif
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Version = "v1",
                        Title = "MVCWebApiStarter",
                        Description = "A simple ASP.NET Core Web API & MVC starter with Authentication.",
                        TermsOfService = "None",
                        Contact = new Contact
                        {
                            Name = "Heet '81NARY' Mehta",
                            Email = "thebinaryguy@brownwolfstudio.com",
                            Url = "https://twitter.com/iamthebinaryguy"
                        },
                        License = new License
                        {
                            Name = "Use under MIT",
                            Url = "https://spdx.org/licenses/MIT.html"
                        }
                    }
                );

                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = "apiKey"
                    }
                );

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    {
                        "Bearer",
                        Enumerable.Empty<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                app.UseDeveloperExceptionPage();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MVCWebApiStarter");
                });

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors("EnableCORS");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
