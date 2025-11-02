namespace MomNom_Backend.Model.Request
{
    public class NewWeightRequest
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public decimal Weight { get; set; }
    }
}
