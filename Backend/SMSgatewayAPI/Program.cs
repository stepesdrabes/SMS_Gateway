using System;
using DAL;
using IdGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SMSgatewayAPI.Hubs;
using SMSgatewayAPI.Managers;
using SMSgatewayAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFrameworkSqlite().AddDbContext<DatabaseContext>();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SMSgateway",
        Version = "v1"
    });
});

builder.Services.AddCors();

// Date since when should snowflake IDs generate
var snowflakeEpoch = new DateTime(2021, 1, 1, 0, 0, 0);

builder.Services.AddSingleton(new SnowflakeGenerator(0, snowflakeEpoch));
builder.Services.AddSingleton(new DevicesManager());
builder.Services.AddSingleton(new SessionTokenManager());

builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<IStatisticsService, StatisticsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(
        options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "SMSgateway v1"));
}

app.UseCors(policyBuilder => policyBuilder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

using (var context = new DatabaseContext())
{
    context.Database.EnsureCreated();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ConnectionHub>("/connectionHub");
    endpoints.MapHub<WebHub>("/webHub");
});

app.Run();