using MomNom_Backend.Model.Object;

namespace MomNom_Backend.Model.Response
{
    public class DashboardResponse
    {
        public List<Plan>? Plans { get; set; }

        public string? Username {get; set;}

        public object? RemainingNutritions { get; set; }
        
        public List<DailyLog>? DailyLogs { get; set; }

        public WeightGainCalc? CurrWeightGain { get; set; }

        public List<string>? TipsList { get; set; }
    }
}
