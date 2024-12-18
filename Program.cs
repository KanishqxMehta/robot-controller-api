using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using robot_controller_api.Persistence;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// 4.1P:
// builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandADO>();
// builder.Services.AddScoped<IMapDataAccess, MapADO>();
// 4.2C:
// builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
// builder.Services.AddScoped<IMapDataAccess, MapRepository>()
builder.Services.AddDbContext<RobotContext>(options => options.UseNpgsql("Host=localhost;Database=sit331;Username=Kanishq_Mehta;Password=Kani@2004"));

// 4.3D:
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandEF>();
builder.Services.AddScoped<IMapDataAccess, MapEF>();
builder.Services.AddScoped<IUserAccess, UserEF>();
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions,BasicAuthenticationHandler>("BasicAuthentication", default);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "New backend service that provides resources for the Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Kanishq Mehta",
            Email = "kanishq5001.be22@chitkara.edu.in"
        },
    });
       var xmlFilename = $" {Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
       options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "robot-controller-api.xml"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-flattop.css"));
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
