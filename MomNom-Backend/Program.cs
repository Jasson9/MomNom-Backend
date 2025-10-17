using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MomNom_Backend;
using MomNom_Backend.Model.Db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // ... other Swagger options

    //options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Document the Set-Cookie header for your login endpoint
    //options.OperationFilter<SetCookieHeaderOperationFilter>(); // Custom filter (see below)
    //options.OperationFilter<CustomHeaderSwaggerAttribute>();
}); 

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<MomNomContext>(options =>options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.6-mysql")));
builder.Services.AddDbContext<MomNomContext>();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Login";
//        options.Cookie.Name = "sessionId"; // Choose a descriptive name
//        //options.AccessDeniedPath = "/Account/AccessDenied"; // Path for access denied
//        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Cookie expiration
//    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.Use(async (context, next) =>
//{
//    Console.WriteLine("Before");
//    await next.Invoke();
//    Console.WriteLine("After");
//});

app.UseMiddleware<CustomMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
