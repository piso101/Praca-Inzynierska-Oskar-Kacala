using BackEnd.Application.Contracts.Auth;
using BackEnd.Domain.Repositories;
using BackEnd.Domain.Shared;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Infrastructure.Persistence.Repositories;
using BackEnd.Infrastructure.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Npgsql.NameTranslation;
using System.Security.Claims;
using System.Text;

namespace BackEndPortfolioSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger
            //builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BackEnd API",
                    Version = "v1",
                    Description = "API z JWT autoryzacją"
                });

                // 🔐 Dodaj definicję schematu JWT Bearer
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Podaj token JWT w formacie: Bearer {token}"
                });

                // 🔒 Wymuś użycie schematu na zabezpieczonych endpointach
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
                            Array.Empty<string>()
                        }
                    });
            });

            // DbContext (PostgreSQL) z mapowaniem enum 'user_role' na poziomie DataSource
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
            var enumTranslator = new NpgsqlSnakeCaseNameTranslator();
            dataSourceBuilder.MapEnum<UserRole>("user_role", enumTranslator);
            var dataSource = dataSourceBuilder.Build();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(dataSource));

            // AutoMapper (Auth profile)
            builder.Services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);

            // CORS dla frontendu (Vite: 5173)
            const string CorsPolicyName = "AllowDev";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:5173",
                        "http://127.0.0.1:5173",
                        "http://frontend:5173"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            // Rejestracja repozytoriów
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
            builder.Services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // === JWT Authentication ===
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "REDACTED_CREDENTIAL!!!";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "app";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "app";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role // <- musi zgadzać się z typem claim w tokenie
                };
            });

            builder.Services.AddAuthorization();

            // === Services ===
            builder.Services.AddScoped<JwtTokenService>();

            var app = builder.Build();
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}