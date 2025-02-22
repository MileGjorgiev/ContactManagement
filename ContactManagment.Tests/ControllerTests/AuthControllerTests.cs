using Microsoft.AspNetCore.Mvc;
using Xunit;
using ContactManagement.API.Controllers.V1;
using ContactManagement.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ContactManagment.Tests.ControllerTests
{


    public class AuthControllerTests
    {
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authController = new AuthController();
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "mile",
                Password = "mile123"
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);
            Xunit.Assert.NotNull(okResult.Value);

            // Use reflection to access the token property
            var tokenProperty = okResult.Value.GetType().GetProperty("token");
            Xunit.Assert.NotNull(tokenProperty); // Ensure the property exists

            var token = tokenProperty.GetValue(okResult.Value) as string;
            Xunit.Assert.NotNull(token);

            // Validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("my32byteverysecretkey12345678901");
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "YourIssuer",
                ValidateAudience = true,
                ValidAudience = "YourAudience",
                ValidateLifetime = true
            };

            // Ensure the token is valid
            SecurityToken validatedToken; // Explicitly declare the type
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            Xunit.Assert.NotNull(principal);
            Xunit.Assert.True(validatedToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "invalid",
                Password = "invalid123"
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
            Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_NullCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = null,
                Password = null
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
            Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_EmptyCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "",
                Password = ""
            };

            // Act
            var result = _authController.Login(loginDto);

            // Assert
            var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);
            Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }
    }
}

