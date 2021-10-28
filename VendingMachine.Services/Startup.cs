using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using VendingMachine.Services.Application;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Abstractions.Repositories;
using VendingMachine.Services.Application.Mappers;
using VendingMachine.Services.Infrastructure;
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

            RegisterMapper(services);
            RegisterRepositories(services);
            RegisterBlls(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "VendingMachine.Services", Version = "v1" });
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
            services.AddTransient<IUserBll, UserBll>();
        }
    }
}
