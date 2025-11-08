using BackEnd.Infrastructure.Persistence;
using BackEnd.Infrastructure.Persistence.Repositories;
using BackEnd.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using BackEnd.Application.Contracts.Auth;
using BackEnd.Infrastructure.Services.Auth;
using BackEnd.Domain.Shared;
using Npgsql;
using Npgsql.NameTranslation;

namespace BackEndPortfolioSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            // JWT Auth
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-secret-change";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "app";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "app";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true
                };
            });

            builder.Services.AddSingleton<JwtTokenService>();

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


