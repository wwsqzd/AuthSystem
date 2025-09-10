
// builder
using AuthSystem.Core.DTOs;
using AuthSystem.Core.Interfaces;
using AuthSystem.Core.Mappings;
using AuthSystem.Core.Services;
using AuthSystem.Core.Validation;
using AuthSystem.DataAccess.Context;
using AuthSystem.DataAccess.Repository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();


// Controllers
builder.Services.AddControllers();

builder.Services.AddOpenApi();

// link 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AbstractValidator<RegisterDTO>, RegisterDTOValidator>();
builder.Services.AddScoped<AbstractValidator<LoginDTO>, LoginDTOValidator>();

//builder.Services.AddValidatorsFromAssemblyContaining<LoginDTOValidator>();
//builder.Services.AddValidatorsFromAssemblyContaining<RegisterDTOValidator>();


// mapping
builder.Services.AddAutoMapper(typeof(MappingProfile));

// allow frontend fetch
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// Swagger
builder.Services.AddEndpointsApiExplorer();

// Bearer token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Token: Bearer {token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



var app = builder.Build();


// allow frontend fetch
app.UseCors("AllowFrontend");


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// swagger
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

// auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
