using Microsoft.AspNetCore.Mvc;
using MomNom_Backend.Handler;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using System.Drawing;
using System.Numerics;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightPlanProgress : ControllerBase
    {
        private readonly MomNomContext _context;
        private readonly CallProcedureHandler _procedureHandler;

        public WeightPlanProgress(MomNomContext context)
        {
            _context = context;
            _procedureHandler = new CallProcedureHandler(context);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<WeightPlanProgressResponse>>> WeightGainPlanProgress([FromHeader] string authentication)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && e.PlanStatus == "AC").Count();

                List<WeightGain> weightGainList = await _procedureHandler.GetWeightGainReport(user.UserId, planId, DateOnly.FromDateTime(DateTime.Now));

                var weightGainListCalc = weightGainList.OrderBy(x => x.year).ThenBy(x => x.monthNumber).Select(x => new WeightGainCalc
                {
                    MonthYear = x.monthName + " " + x.year,
                    MonthlyGain = x.monthlyGain,
                    TotalGain = x.totalGain,
                    Percentage = Math.Round(((x.totalGain / x.recGain) * 100), 2) + "%"
                }).ToList() ?? [];

                return new BaseResponse<WeightPlanProgressResponse>(new WeightPlanProgressResponse { weightGainProgress = weightGainListCalc });
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<WeightPlanProgressResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<WeightPlanProgressResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<WeightPlanProgressResponse>("Unexpected internal server error occured").toResponseOutput();
            }
        }

        [HttpPost("AddNewWeight")]
        public async Task<ActionResult<BaseResponse<TrMonthlyWeight>>> AddNewMonthlyWeight([FromHeader] string authentication, [FromBody] NewWeightRequest weightReq)
        {
            try
            {

            var user = await Auth.ValidateAuthToken(_context, authentication);
            var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && e.PlanStatus == "AC").Count();

            if (weightReq.Month < 1 || weightReq.Month > 12)
            {
                throw new BadRequestException<TrMonthlyWeight>("Invalid month!");
            }
            if (weightReq.Year == 0)
            {
                throw new BadRequestException<TrMonthlyWeight>("Invalid year!");
            }
            if (weightReq.Weight == 0)
            {
                throw new BadRequestException<TrMonthlyWeight>("Invalid weight!");
            }

            var newMonthlyWeight = _context.TrMonthlyWeights.Add(new TrMonthlyWeight
            {
                UserId = user.UserId,
                PlanId = planId,
                Weight = weightReq.Weight,
                Year = weightReq.Year,
                Month = weightReq.Month
            });
            await _context.SaveChangesAsync();
            
            return new BaseResponse<TrMonthlyWeight>(newMonthlyWeight.Entity);
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<TrMonthlyWeight>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<TrMonthlyWeight> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<TrMonthlyWeight>("Unexpected internal server error occured").toResponseOutput();
            }
        }
    }
}
