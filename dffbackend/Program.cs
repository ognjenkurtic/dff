using System.Reflection;
using dffbackend.Filters;
using dffbackend.Models;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.File("Logs/dffapp.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
string connectionString = builder.Configuration["ConnectionStrings:Mysql"];
builder.Services.AddDbContext<DffContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// add identity here if needed

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Register interfaces

builder.Services.AddControllers();
builder.Services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DffContext>();
    dataContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "dff-api v1");
            c.RoutePrefix = string.Empty;
            c.EnableFilter();
        });
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
