namespace JoliPet.Services;

public interface ICurrentUserService
{
    int GetCurrentUserId();
}

public class CurrentUserService : ICurrentUserService
{
    public int GetCurrentUserId()
    {
        return 1;
    }
}