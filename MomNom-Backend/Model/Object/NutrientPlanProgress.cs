using Microsoft.EntityFrameworkCore;

namespace MomNom_Backend.Model.Object
{
    [Keyless]
    public class NutrientPlanProgress
    {
        public string nutrientName { get; set; }

        public decimal nutrientAmount { get; set; }

        public decimal goalAmount { get; set; }

        public string unit { get; set; }
    }
}
