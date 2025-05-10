using DocumentManagement.Application.Services;
using Moq;
using Xunit;
using Microsoft.Extensions.Options;
using DocumentManagement.Application.DTOs;
using DocumentManagement.Domain.Entities;

public class AuthServiceTests
{
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        _mockJwtSettings.Setup(x => x.Value).Returns(new JwtSettings
        {
            Key = "YourSecretKeyisfsdfiedsfdsfjsafasfsdffwea",
            ExpiresInMinutes = 30,
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        });
        _authService = new AuthService(_mockJwtSettings.Object);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnToken()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Username = "testuser", Role = "Admin" };
        
        // Act
        var token = _authService.GenerateJwtToken(user);
        
        // Assert
        Assert.NotNull(token);
        Assert.IsType<string>(token);
    }
}
