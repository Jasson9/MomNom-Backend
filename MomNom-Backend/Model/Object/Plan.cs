

namespace MomNom_Backend.Model.Object
{
    public class Plan
    {
        public int PlanId { get; set; }

        public int? StartWeek { get; set; }

        public decimal? Weight { get; set; }

        public decimal? Height { get; set; }

        public int? Age { get; set; }

        public decimal? PrePregnancyWeight { get; set; }

        public string? BmiCategory { get; set; }

        public decimal? CalFirstTrimester { get; set; }

        public decimal? CalSecondThirdTrimester { get; set; }
    }
}
