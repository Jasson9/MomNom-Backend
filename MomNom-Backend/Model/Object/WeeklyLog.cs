using Microsoft.EntityFrameworkCore;

namespace MomNom_Backend.Model.Object
{
    [Keyless]
    public class WeeklyLog
    {
        public int weekId { get; set; }

        public DateOnly weekStart {  get; set; }

        public DateOnly weekEnd { get; set;}

        public decimal totalCalories { get; set; }

        public decimal totalProtein { get; set; }

        public decimal totalCarbohydrates { get; set; }

        public decimal totalFiber { get; set; }
    }
}
