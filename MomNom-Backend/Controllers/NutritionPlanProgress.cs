using Microsoft.AspNetCore.Mvc;
using MomNom_Backend.Handler;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Response;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionPlanProgress : ControllerBase
    {
        private readonly MomNomContext _context;
        private readonly CallProcedureHandler _procedureHandler;

        public NutritionPlanProgress(MomNomContext context)
        {
            _context = context;
            _procedureHandler = new CallProcedureHandler(context);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<NutrientPlanProgressResponse>>> dailyNutritionProgress([FromHeader] string authentication, [FromBody] DateOnly date)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && e.PlanStatus == "AC").Count();

                List<NutrientPlanProgress> nutrientPlanProgress = await _procedureHandler.GetDailyNutritionReport(user.UserId, planId, date);

                return new BaseResponse<NutrientPlanProgressResponse>(new NutrientPlanProgressResponse { nutrientPlanProgresses = nutrientPlanProgress });
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<NutrientPlanProgressResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<NutrientPlanProgressResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<NutrientPlanProgressResponse>("Unexpected internal server error occured").toResponseOutput();
            }
        }
    }
}
