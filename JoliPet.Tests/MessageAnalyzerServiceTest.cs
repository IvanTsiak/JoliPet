using JoliPet.Models;
using JoliPet.Services;
using Microsoft.EntityFrameworkCore;

namespace JoliPet.Tests;

public class MessageAnalyzerServiceTest
{
    private JoliPetContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<JoliPetContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new JoliPetContext(options);
    }

    [Fact]
    public async Task CalculateMessageWeightAsync_ShouldReturnCorrectWeight()
    {
        var context = GetInMemoryDbContext();
        context.Words.Add(new Word
        {
            Text = "бусінка",
            Level = 2
        });
        context.Words.Add(new Word
        {
            Text = "поганий",
            Level = -1
        });
        await context.SaveChangesAsync();

        var service = new MessageAnalyzerService(context);

        string message = "Ти ще та бусінка, хоча кухар з тебе поганий";
        int weight = await service.CalculateMessageWeightAsync(message);
        
        Assert.Equal(1, weight);
    }

    [Fact]
    public async Task CalculateMessageWeightAsync_ShouldCountGood()
    {
        var context = GetInMemoryDbContext();
        context.Words.Add(new Word
        {
            Text = "Солоденький",
            Level = 2
        });
        await context.SaveChangesAsync();
        
        var  service = new MessageAnalyzerService(context);

        string message = "Солоденький солоденький СоЛоДЕНЬКИЙ солоденький, солоденький!";
        int weight = await service.CalculateMessageWeightAsync(message);
        
        Assert.Equal(10, weight);
    }
}