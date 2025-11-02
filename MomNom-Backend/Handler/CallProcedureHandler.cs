using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MySqlConnector;
using System.Text.Json;

namespace MomNom_Backend.Handler
{
    public class CallProcedureHandler
    {
        private readonly MomNomContext _context;

        public CallProcedureHandler(MomNomContext context)
        {
            _context = context;
        }

        public async Task<List<DailyLog>> GetDailyFoodReport(int userId, int planId, DateOnly date)
        {
            var userIdParameter = new MySqlParameter("@p_userId", userId);
            var planIdParameter = new MySqlParameter("@p_planId", planId);
            var dateParameter = new MySqlParameter("@p_date", date);

            List<DailyLog> dailyLogs = await _context.GetDailyFoodDetailResult.FromSqlRaw<DailyLog>("CALL GetDailyFoodDetail(@p_userId, @p_planId, @p_date)", userIdParameter, planIdParameter, dateParameter).ToListAsync() ?? [];
            
            var dailyLog = dailyLogs.Select(x => new DailyLog
            {
                FoodName = x.FoodName,
                Amount = x.Amount,
                TotalCalories = x.TotalCalories,
                NutrientsListDetail = string.IsNullOrEmpty(x.NutrientsList) ? new List<Nutrient>() : JsonSerializer.Deserialize<List<Nutrient>>(x.NutrientsList)
            }).ToList() ?? [];

            return dailyLog;
        }

        public async Task<List<WeeklyLog>> GetWeeklyNutritionReport(int userId, int planId, int month, int year)
        {
            var userIdParameter = new MySqlParameter("@p_userId", userId);
            var planIdParameter = new MySqlParameter("@p_planId", planId);
            var monthParameter = new MySqlParameter("@p_month", month);
            var yearParameter = new MySqlParameter("@p_year", year);

            List<WeeklyLog> weeklyLogs = await _context.GetWeeklyNutritionReport.FromSqlRaw("CALL GetWeeklyNutritionReport(@p_userId, @p_planId, @p_month, @p_year)", userIdParameter, planIdParameter, monthParameter, yearParameter).ToListAsync() ?? [];

            return weeklyLogs;
        }

        public async Task<List<NutrientPlanProgress>> GetDailyNutritionReport(int userId, int planId, DateOnly date)
        {
            var userIdParameter = new MySqlParameter("@p_userId", userId);
            var planIdParameter = new MySqlParameter("@p_planId", planId);
            var dateParameter = new MySqlParameter("@p_date", date);

            List<NutrientPlanProgress> dailyNutrient = await _context.GetDailyNutritionReport.FromSqlRaw("CALL GetDailyNutritionReport(@p_userId, @p_planId, @p_date)", userIdParameter, planIdParameter, dateParameter).ToListAsync() ?? [];

            return dailyNutrient;
        }

        public async Task<List<WeightGain>> GetWeightGainReport(int userId, int planId, DateOnly date)
        {
            var userIdParameter = new MySqlParameter("@p_userId", userId);
            var planIdParameter = new MySqlParameter("@p_planId", planId);
            var dateParameter = new MySqlParameter("@p_date", date);

            List<WeightGain> weightGainList = await _context.GetWeightGainReport.FromSqlRaw("CALL GetWeightGainReport(@p_userId, @p_planId, @p_date)", userIdParameter, planIdParameter, dateParameter).ToListAsync() ?? [];

            return weightGainList;
        }
    }
}
