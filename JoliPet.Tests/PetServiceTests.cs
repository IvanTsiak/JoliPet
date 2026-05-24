using System.Reflection.Metadata;
using JoliPet.Models;
using JoliPet.Services;
using JoliPet.Shared;
using Moq;

namespace JoliPet.Tests;

public class PetServiceTests
{
    [Fact]
    public async Task UpdateExperienceAsync_ShouldLevelUpAndNotify_WhenEnoughXpGained()
    {
        var mockNotifications = new Mock<INotificationService>();
        var service = new PetService(mockNotifications.Object);

        var pet = new Pet
        {
            Name = "Елеонора",
            Level = 1,
            Experience = 90,
            Status = Constants.StatusAlive,
            UserId = 1
        };

        await service.UpdateExperienceAsync(pet, 20);
        
        Assert.Equal(10, pet.Experience);
        Assert.Equal(2, pet.Level);
        
        mockNotifications.Verify(n =>
            n.AddNotificationAsync(pet.UserId, It.Is<string>(msg => msg.Contains("reached the level 2"))),
            Times.Once);
    }
}