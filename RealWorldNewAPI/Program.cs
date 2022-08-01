using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealWorldNewAPI.Data;
using Common;
using RealWorldNewAPI;
using RealWorldNew.BAL;
using RealWorldNewAPI.Controllers;
using RealWorldNew.DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.UI;
using AutoMapper;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using RealWorldNew.BAL.Services;
using RealWorldNew.DAL.Interfaces;
using RealWorldNew.Common;
using RealWorldNew.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddCors(option =>
{
    option.AddPolicy("Front", builder =>
    {
        builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
    });
});

var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
});


builder.Services.AddDbContext<ApplicationDbContext>
    (o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ApplicationDbContext>();



var identityServer = builder.Services
    .AddIdentityServer()
    .AddApiAuthorization<User, ApplicationDbContext>();


    identityServer.AddDeveloperSigningCredential();


builder.Services.AddAuthentication()
    .AddIdentityServerJwt();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ASP.NET 6 Web API",
        Description = "Authentication and Authorization in ASP.NET 5 with JWT and Swagger"
    });  
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        new string[] {}
        }
    });
});

builder.Services.AddScoped<IPackingService, PackingService>();
builder.Services.AddScoped<ICommentRepositories, CommentRepositories>();
builder.Services.AddScoped<IFollowRepositories, FollowRepositories>();
builder.Services.AddScoped<IArticleRepositories, ArticleRepositories>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseCors("Front");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        o.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();
