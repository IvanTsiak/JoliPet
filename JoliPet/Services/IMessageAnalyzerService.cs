namespace JoliPet.Services;

public interface IMessageAnalyzerService
{
    Task<int> CalculateMessageWeightAsync(string message);
}