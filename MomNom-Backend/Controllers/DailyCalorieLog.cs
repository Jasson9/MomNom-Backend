using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DailyCalorieLog : ControllerBase
    {
        private readonly MomNomContext _context;

        public DailyCalorieLog(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<DailyCalorieLogResponse>>> dailycalorielog([FromHeader] string authentication, [FromBody] DateOnly date)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);

                List<DailyLog> dailyLogs = _context.TrDailyCalorieLogs.Where((e) => e.UserId == user.UserId && e.Date == date).Include(e => e.Food).ThenInclude(s => s.MsFoodNutrients).ThenInclude(t => t.Nutrient).Select(
                    e => new DailyLog
                    {
                        PlanId = e.PlanId,
                        UserId = e.UserId,
                        FoodId = e.FoodId,
                        FoodName = e.Food.FoodName,
                        Calorie = e.Food.Calorie,
                        NutrientsList = e.Food.MsFoodNutrients.Select(
                            t => new Nutrient
                            {
                                NutrientName = t.Nutrient.NutrientName,
                                NutrientAmount = t.Amount,
                                NutrientUnit = t.Nutrient.Unit
                            }
                            ).ToList()
                        //NutrientName = e.Food.MsFoodNutrients.Select(t => t.Nutrient.NutrientName).ToList(),
                        //NutrientAmount = e.Food.MsFoodNutrients.Select(t => t.Amount).ToList()

                    }
                    ).ToList() ?? [];

                return new BaseResponse<DailyCalorieLogResponse>(new DailyCalorieLogResponse { dailyLogs = dailyLogs });
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<DailyCalorieLogResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<DailyCalorieLogResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<DailyCalorieLogResponse>("Unexpected internal server error occured").toResponseOutput();
            }
        }
    }
}
