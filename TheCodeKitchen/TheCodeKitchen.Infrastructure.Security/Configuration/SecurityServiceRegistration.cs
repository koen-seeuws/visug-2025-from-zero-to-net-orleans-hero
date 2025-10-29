using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TheCodeKitchen.Infrastructure.Extensions;

namespace TheCodeKitchen.Infrastructure.Security.Configuration;

public static class SecurityServiceRegistration
{
    public static void AddPasswordHashingServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
    }


    public static void AddJwtSecurityServices(this IServiceCollection services,
        IConfiguration configuration, string sectionKey = "JwtSecurity")
    {
        var jwtSecuritySecurityOptions =
            configuration.BindAndValidateConfiguration<JwtSecurityOptions, JwtSecurityOptionsValidator>(sectionKey);

        var key = Encoding.UTF8.GetBytes(jwtSecuritySecurityOptions.Secret);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddSingleton(jwtSecuritySecurityOptions);
        services.AddScoped<ISecurityTokenService, JwtTokenService>();
    }
}