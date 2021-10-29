using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using VendingMachine.Services.Application;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Application.Mappers;
using VendingMachine.Services.Infrastructure;
using VendingMachine.Services.Infrastructure.EFDbContext;
using VendingMachine.Services.Mapper;

namespace VendingMachine.Services
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
            services.AddControllers();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(options => options.SlidingExpiration = true)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = "http://localhost:30725",
                        ValidAudience = "http://localhost:30725",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Token:Issuer"))),

                    };
                });
                
            
            RegisterDatabase(services);
            RegisterMapper(services);
            RegisterRepositories(services);
            RegisterBlls(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VendingMachine.Services", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Description = "Please insert JWT token into field"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VendingMachine.Services v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserApiProfile));
            services.AddAutoMapper(typeof(UserAppDomainProfile));
        }

        private void RegisterRepositories(IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
        }

        private void RegisterBlls(IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IUserService, UserService>();
        }

        private void RegisterDatabase(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("VendingMachine");
            services.AddDbContext<VendingMachineContext>(options => options.UseSqlServer(connection));
        }
    }
}
