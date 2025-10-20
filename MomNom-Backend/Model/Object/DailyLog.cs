namespace MomNom_Backend.Model.Object
{
    public class DailyLog
    {
        public int PlanId { get; set; }

        public int UserId { get; set; }

        public int FoodId { get; set; }

        public string FoodName { get; set; }

        public decimal Calorie { get; set; }

        public List<Nutrient> NutrientsList { get; set; }

        public decimal? Amount { get; set; }

        public DateOnly Date { get; set; }
    }
}
