using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Handler;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Response;
using System.Globalization;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeeklyLogReport : ControllerBase
    {
        private readonly MomNomContext _context;
        private readonly CallProcedureHandler _procedureHandler;

        public WeeklyLogReport(MomNomContext context)
        {
            _context = context;
            _procedureHandler = new CallProcedureHandler(context);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<WeeklyLogResponse>>> weeklylog([FromHeader] string authentication)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && e.planStatus == "AC").Count();
                var monthYearList = await _context.TrDailyCalorieLogs.Select(e => new {
                    e.Date.Month,
                    e.Date.Year
                }).Distinct().OrderBy(e => e.Year).ThenBy(e => e.Month).ToListAsync() ?? [];

                var allWeeklyLogList = new List<WeeklyLog>();
                foreach ( var monthYear in monthYearList )
                {
                    List<WeeklyLog> weeklyLogs = await _procedureHandler.GetWeeklyNutritionReport(user.UserId, planId, monthYear.Month, monthYear.Year);

                    allWeeklyLogList.AddRange(weeklyLogs);

                }

                var allWeeklyLogGroup = allWeeklyLogList.GroupBy(x => new { x.weekStart.Month, x.weekStart.Year }).Select(y => new WeeklyLogGroup
                {
                    Year = y.Key.Year,
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(y.Key.Month),
                    weeklyLogDetail = y.ToList()
                }).ToList();

                return new BaseResponse<WeeklyLogResponse>(new WeeklyLogResponse { weeklyLogs = allWeeklyLogGroup });
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<WeeklyLogResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<WeeklyLogResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<WeeklyLogResponse>("Unexpected internal server error occured").toResponseOutput();
            }
        }
    }
}
