namespace MomNom_Backend.Model.Object
{
    public class WeeklyLogGroup
    {
        public string Month { get; set; }

        public int Year { get; set; }

        public List<WeeklyLog> weeklyLogDetail { get; set; }
    }
}
