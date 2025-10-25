using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Object;

namespace MomNom_Backend.Handler
{
    public class DailyCalorieHandler
    {
        private readonly MomNomContext _context;

        public DailyCalorieHandler(MomNomContext context)
        {
            _context = context;
        }

        public async Task<List<DailyLog>> GetDailyCalorieLogAll(MsUser user, DateOnly date)
        {
            List<DailyLog> dailyLogs = await _context.TrDailyCalorieLogs.Where((e) => e.UserId == user.UserId && e.Date == date).Include(e => e.Food).ThenInclude(s => s.MsFoodNutrients).ThenInclude(t => t.Nutrient).Select(
                    e => new DailyLog
                    {
                        PlanId = e.PlanId,
                        UserId = e.UserId,
                        FoodId = e.FoodId,
                        FoodName = e.Food.FoodName,
                        Calorie = e.Food.Calorie,
                        NutrientsList = e.Food.MsFoodNutrients.Select(
                            t => new Nutrient
                            {
                                NutrientName = t.Nutrient.NutrientName,
                                NutrientAmount = t.Amount,
                                NutrientUnit = t.Nutrient.Unit
                            }
                            ).ToList()
                        //NutrientName = e.Food.MsFoodNutrients.Select(t => t.Nutrient.NutrientName).ToList(),
                        //NutrientAmount = e.Food.MsFoodNutrients.Select(t => t.Amount).ToList()

                    }
                    ).ToListAsync() ?? [];

            return dailyLogs;
        }
    }
}
