using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Handler;
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
        private readonly DailyCalorieHandler _dailyCalorieHandler;

        public DailyCalorieLog(MomNomContext context)
        {
            _context = context;
            _dailyCalorieHandler = new DailyCalorieHandler(context);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<DailyCalorieLogResponse>>> dailycalorielog([FromHeader] string authentication, [FromBody] DateOnly date)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);

                List<DailyLog> dailyLogs = await _dailyCalorieHandler.GetDailyCalorieLogAll(user, date);

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
