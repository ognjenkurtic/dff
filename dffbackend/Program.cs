using System.Reflection;
using dffbackend.BusinessLogic.Signatures.Agents;
using dffbackend.Models;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;

var  corsPolicyName = "_dffCorsPolicy";
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

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register interfaces
builder.Services.AddScoped<ISignaturesAgent, SignaturesAgent>();

builder.Services.AddControllers();
// TODO: compiler throws warning AddFluentValidation is deprecated, however other methods won't work. Investigate this later on.
builder.Services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(ConfigureSwaggerGen);

builder.Services.AddCors(options => 
{
    options.AddPolicy(corsPolicyName, builder => 
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

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


app.UseCors(corsPolicyName);

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


void ConfigureSwaggerGen(SwaggerGenOptions c)
{
    c.AddSecurityDefinition("X-Factor-API-Key", new OpenApiSecurityScheme
    {
        Description = "Example: \"X-Factor-API-Key {token}\"",
        Name = "X-Factor-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference
        {
            Id = "X-Factor-API-Key",
            Type = ReferenceType.SecurityScheme
        }}, new List<string>() }
    });
}