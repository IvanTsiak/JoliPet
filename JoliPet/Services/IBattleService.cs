using JoliPet.DTOs;

namespace JoliPet.Services;

public interface IBattleService
{
    Task<BattleResultDto> ExecuteBattleAsync(int attackerId, int defenderId);
}