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
            // Arrange: Set up the login DTO with valid credentials
            var loginDto = new LoginDto
            {
                Username = "login",
                Password = "login"
            };

            // Act: Call the Login method of the controller with the loginDto
            var result = _authController.Login(loginDto);

            // Assert: Check if the result is an OkObjectResult
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result);

            // Assert: Ensure that the result has a non-null value
            Xunit.Assert.NotNull(okResult.Value);

            // Assert: Check if the result contains a token property
            var tokenProperty = okResult.Value.GetType().GetProperty("token");
            Xunit.Assert.NotNull(tokenProperty);

            // Assert: Ensure the token is not null
            var token = tokenProperty.GetValue(okResult.Value) as string;
            Xunit.Assert.NotNull(token);

            // Act: Validate the token using JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("eZkz8l3XYuV8Zt9tRpw89nA9DbYy5PZ5TrJ1mTgK1yU="); // Your secret key
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "ContactManagement", // Your issuer value
                ValidateAudience = true,
                ValidAudience = "ContactManagement", // Your audience value
                ValidateLifetime = true
            };

            // Act: Validate the token
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            // Assert: Ensure the principal is not null
            Xunit.Assert.NotNull(principal);

            // Assert: Check if the token's expiration time is in the future
            Xunit.Assert.True(validatedToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange: Set up the login DTO with invalid credentials
            var loginDto = new LoginDto
            {
                Username = "invalid",
                Password = "invalid123"
            };

            // Act: Call the Login method of the controller with the invalid credentials
            var result = _authController.Login(loginDto);

            // Assert: Check if the result is an UnauthorizedObjectResult
            var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);

            // Assert: Ensure the response contains the appropriate error message
            Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_NullCredentials_ReturnsUnauthorized()
        {
            // Arrange: Set up the login DTO with null credentials
            var loginDto = new LoginDto
            {
                Username = null,
                Password = null
            };

            // Act: Call the Login method with null credentials
            var result = _authController.Login(loginDto);

            // Assert: Check if the result is an UnauthorizedObjectResult
            var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);

            // Assert: Ensure the response contains the appropriate error message
            Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }


        [Fact]
        public void Login_EmptyCredentials_ReturnsUnauthorized()
        {
            // Arrange: Set up the login DTO with empty credentials
            var loginDto = new LoginDto
            {
                Username = "",
                Password = ""
            };

            // Act: Call the Login method with empty credentials
            var result = _authController.Login(loginDto);

            // Assert: Check if the result is an UnauthorizedObjectResult
            var unauthorizedResult = Xunit.Assert.IsType<UnauthorizedObjectResult>(result);

            // Assert: Ensure the response contains the appropriate error message
            Xunit.Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }
    }
}

