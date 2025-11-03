namespace MomNom_Backend.Model.Object
{
    public class WeightGainCalc
    {
        public string MonthYear {  get; set; }

        public decimal MonthlyGain { get; set; }

        public decimal TotalGain { get; set; }

        public decimal RecGain { get; set; }

        public string Percentage {  get; set; }
    }
}
