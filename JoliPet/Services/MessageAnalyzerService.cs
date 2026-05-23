using JoliPet.Models;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Services;

public class MessageAnalyzerService : IMessageAnalyzerService
{
    private readonly JoliPetContext _context;
    
    public MessageAnalyzerService(JoliPetContext context)
    {
        _context = context;
    }

    public async Task<int> CalculateMessageWeightAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return 0;
        }

        var wordsInMessage = message.ToLower()
            .Split(new[] { ' ', '.', ',', '!', '?', '\n', '\r', '-', ':' }, StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .ToList();

        if (wordsInMessage.Count == 0)
        {
            return 0;
        }

        var foundWords = await _context.Words
            .Where(w => wordsInMessage.Contains(w.Text.ToLower()))
            .ToListAsync();

        var allMessageWords = message.ToLower()
            .Split(new[] { ' ', '.', ',', '!', '?', '\n', '\r', '-', ':' }, StringSplitOptions.RemoveEmptyEntries);
        
        int totalWeight = 0;

        foreach (var word in allMessageWords)
        {
            var dbWord = foundWords.FirstOrDefault(w => w.Text.ToLower() == word);

            if (dbWord != null)
            {
                totalWeight += dbWord.Level;
            }
        }
        
        return totalWeight;
    }
}