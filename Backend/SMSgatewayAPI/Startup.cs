using System;
using DAL;
using IdGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SMSgatewayAPI.Hubs;
using SMSgatewayAPI.Managers;
using SMSgatewayAPI.Services;

namespace SMSgatewayAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();
            services.AddControllers();
            services.AddSignalR();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SMSgateway",
                    Version = "v1"
                });
            });


            services.AddCors();

            // Date since when should snowflake IDs generate
            var snowflakeEpoch = new DateTime(2021, 1, 1, 0, 0, 0);

            services.AddSingleton(new SnowflakeGenerator(0, snowflakeEpoch));
            services.AddSingleton(new DevicesManager());
            services.AddSingleton(new SessionTokenManager());

            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IStatisticsService, StatisticsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
                application.UseSwagger();
                application.UseSwaggerUI(
                    options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "SMSgateway v1"));
            }

            application.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials());

            //application.UseHttpsRedirection();
            application.UseRouting();
            application.UseAuthorization();

            using (var context = new DatabaseContext())
            {
                context.Database.EnsureCreated();
            }

            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ConnectionHub>("/connectionHub");
                endpoints.MapHub<WebHub>("/webHub");
            });
        }
    }
}