
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

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

            var app = builder.Build();
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
            app.UseCors(CorsPolicyName);
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
