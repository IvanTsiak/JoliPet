using System.Security.Claims;
using JoliPet.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace JoliPet.Tests;

public class CurrentUserServiceTest
{
    [Fact]
    public void GetCurrentUserId_ShouldReturnId_WhenUserIsAuthenticated()
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        context.User = new ClaimsPrincipal(identity);
        
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        
        var service = new CurrentUserService(mockHttpContextAccessor.Object);
        
        int userId = service.GetCurrentUserId();
        
        Assert.Equal(123, userId);
    }
}