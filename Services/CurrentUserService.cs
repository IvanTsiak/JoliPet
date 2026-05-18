using System.Security.Claims;

namespace JoliPet.Services;

public interface ICurrentUserService
{
    int GetCurrentUserId();
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;
    
    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    
    public int GetCurrentUserId()
    {
        var user = _accessor.HttpContext?.User;

        if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
        }
        
        throw new InvalidOperationException("User not authenticated");
    }
}