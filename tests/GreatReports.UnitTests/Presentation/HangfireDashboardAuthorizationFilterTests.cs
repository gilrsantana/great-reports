using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GreatReports.Presentation.Filters;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace GreatReports.UnitTests.Presentation;

public class HangfireDashboardAuthorizationFilterTests
{
    private const string Secret = "supersecretkey12345678901234567890";
    private const string Issuer = "test-issuer";
    private const string Audience = "test-audience";
    private readonly HangfireDashboardAuthorizationFilter _filter;
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly Mock<HttpRequest> _httpRequestMock = new();
    private readonly Mock<IRequestCookieCollection> _cookiesMock = new();
    private readonly Mock<IQueryCollection> _queryMock = new();
    private readonly AspNetCoreDashboardContext _dashboardContext;

    public HangfireDashboardAuthorizationFilterTests()
    {
        _filter = new HangfireDashboardAuthorizationFilter(Secret, Issuer, Audience);

        _httpRequestMock.Setup(r => r.Cookies).Returns(_cookiesMock.Object);
        _httpRequestMock.Setup(r => r.Query).Returns(_queryMock.Object);
        _httpContextMock.Setup(c => c.Request).Returns(_httpRequestMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        _httpContextMock.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

        var jobStorageMock = new Mock<JobStorage>();
        _dashboardContext = new AspNetCoreDashboardContext(
            jobStorageMock.Object,
            new DashboardOptions(),
            _httpContextMock.Object);
    }

    [Fact]
    public void Authorize_ShouldReturnFalse_WhenNoTokenIsProvided()
    {
        // Arrange
        string? cookieValue = null;
        _cookiesMock.Setup(c => c.TryGetValue("HangfireToken", out cookieValue)).Returns(false);
        _queryMock.Setup(q => q["token"]).Returns(Microsoft.Extensions.Primitives.StringValues.Empty);

        // Act
        var result = _filter.Authorize(_dashboardContext);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Authorize_ShouldReturnTrue_WhenValidTokenInCookie_WithMaintainerRole()
    {
        // Arrange
        var token = GenerateTestJwt("Maintainer");
        string? cookieValue = token;
        _cookiesMock.Setup(c => c.TryGetValue("HangfireToken", out cookieValue)).Returns(true);

        // Act
        var result = _filter.Authorize(_dashboardContext);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Authorize_ShouldReturnTrue_WhenValidTokenInQuery_WithMaintainerRole()
    {
        // Arrange
        var token = GenerateTestJwt("Maintainer");
        string? cookieValue = null;
        _cookiesMock.Setup(c => c.TryGetValue("HangfireToken", out cookieValue)).Returns(false);
        _queryMock.Setup(q => q["token"]).Returns(token);

        // Act
        var result = _filter.Authorize(_dashboardContext);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Authorize_ShouldReturnFalse_WhenTokenHasInvalidRole()
    {
        // Arrange
        var token = GenerateTestJwt("Partner");
        string? cookieValue = token;
        _cookiesMock.Setup(c => c.TryGetValue("HangfireToken", out cookieValue)).Returns(true);

        // Act
        var result = _filter.Authorize(_dashboardContext);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Authorize_ShouldReturnFalse_WhenTokenIsExpired()
    {
        // Arrange
        var token = GenerateTestJwt("Maintainer", isExpired: true);
        string? cookieValue = token;
        _cookiesMock.Setup(c => c.TryGetValue("HangfireToken", out cookieValue)).Returns(true);

        // Act
        var result = _filter.Authorize(_dashboardContext);

        // Assert
        Assert.False(result);
    }

    private static string GenerateTestJwt(string role, bool isExpired = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: isExpired ? DateTime.UtcNow.AddMinutes(-5) : DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
