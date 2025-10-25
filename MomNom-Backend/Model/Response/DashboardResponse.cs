using MomNom_Backend.Model.Object;

namespace MomNom_Backend.Model.Response
{
    public class DashboardResponse
    {
        public List<Plan> Plans { get; set; } = [];
        public string Username {get; set;}
        public List<DailyLog> dailyLogs { get; set; }
    }
}
