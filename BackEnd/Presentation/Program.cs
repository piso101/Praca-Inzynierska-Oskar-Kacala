using BackEnd.Application.Contracts.Auth;
using BackEnd.Domain.Repositories;
using BackEnd.Domain.Shared;
using BackEnd.Extensions;
using BackEnd.Infrastructure.Services.Auth;

namespace BackEndPortfolioSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services Registration
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithJwt();
            builder.Services.AddCustomDbContext(builder.Configuration);
            builder.Services.AddCustomRepositories();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCustomCors();
            builder.Services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);
            builder.Services.AddScoped<JwtTokenService>();
            #endregion 

            var app = builder.Build();

            #region Middleware Configuration
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
            app.UseCors("AllowDev");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}