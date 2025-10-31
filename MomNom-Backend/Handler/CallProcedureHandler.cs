using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Object;
using MySqlConnector;

namespace MomNom_Backend.Handler
{
    public class CallProcedureHandler
    {
        private readonly MomNomContext _context;

        public CallProcedureHandler(MomNomContext context)
        {
            _context = context;
        }

        public async Task<List<DailyLog>> GetDailyFoodReport(MsUser user, int planId, DateOnly date)
        {
            var userIdParameter = new MySqlParameter("@p_userId", user.UserId);
            var planIdParameter = new MySqlParameter("@p_planId", planId);
            var dateParameter = new MySqlParameter("@p_date", date);

            List<DailyLog> dailyLogs = await _context.GetDailyFoodDetail.FromSqlRaw<DailyLog>("CALL TestProcedure @p_userId @p_planId @p_date", userIdParameter, planIdParameter, dateParameter).ToListAsync() ?? [];

            return dailyLogs;
        }
    }
}
