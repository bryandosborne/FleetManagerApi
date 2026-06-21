using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FleetManagerApi.Endpoints
{
    public static class SecurityEndpoints
    {
        public static void MapSecurityEndpoints(this IEndpointRouteBuilder routes, IConfiguration config)
        {
            // POST: /api/login
            routes.MapPost("/api/login", (LoginRequest request) =>
            {
                // Simulate a simple dispatcher credential check
                if (request.Username == "admin" && request.Password == "password123")
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);

                    // Build the claims (the user's identity bag)
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, request.Username),
                            new Claim(ClaimTypes.Role, "Dispatcher")
                        }),
                        Expires = DateTime.UtcNow.AddHours(2),
                        Issuer = config["Jwt:Issuer"],
                        Audience = config["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    // Return the clean token string back to the user
                    return Results.Ok(new { Token = tokenString });
                }

                return Results.Unauthorized();
            })
            .WithName("Login")
            .AllowAnonymous(); // 🔓 Everyone can access this route to log in!
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
