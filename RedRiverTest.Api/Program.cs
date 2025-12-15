using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace RedRiverTest.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- SERVICES ----------

            builder.Services.AddControllers();

            // CORS – tillåter Angular-klienten
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularClient", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // JWT-inställningar från appsettings.json
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var key = jwtSection["Key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("Jwt:Key saknas i appsettings.json");
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // ---------- MIDDLEWARE ----------

            app.UseHttpsRedirection();

            app.UseCors("AllowAngularClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
