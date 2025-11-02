using Microsoft.EntityFrameworkCore;

namespace MomNom_Backend.Model.Object
{
    [Keyless]
    public class DailyLog
    {
        public string FoodName { get; set; }

        public decimal Amount { get; set; }

        public decimal TotalCalories { get; set; }

        public string NutrientsList { get; set; }

        public List<Nutrient> NutrientsListDetail { get; set; }
    }
}
