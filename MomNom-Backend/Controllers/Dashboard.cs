using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using MomNom_Backend;
using MomNom_Backend.Handler;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Dashboard : ControllerBase
    {
        private readonly MomNomContext _context;
        private readonly CallProcedureHandler _procedureHandler;

        public Dashboard(MomNomContext context)
        {
            _context = context;
            _procedureHandler = new CallProcedureHandler(context);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<DashboardResponse>>> dashboard([FromHeader] string authentication)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && e.planStatus == "AC").Count();

                List<Plan> plans = _context.MsPlans.Where((e) => e.UserId == user.UserId).Select(
                    e => new Plan
                    {
                        Age = e.Age,
                        BmiCategory = e.BmiCategory,
                        CalFirstTrimester = e.CalFirstTrimester,
                        CalSecondThirdTrimester = e.CalSecondThirdTrimester,
                        Height = e.Height,
                        PlanId = e.PlanId,
                        PrePregnancyWeight = e.PrePregnancyWeight,
                        StartWeek = e.StartWeek,
                        Weight = e.Weight,
                    }
                    ).ToList() ?? [];

                List<DailyLog> dailyLog = await _procedureHandler.GetDailyFoodReport(user, planId, DateOnly.FromDateTime(DateTime.Now));
                List<DailyLog> logs = dailyLog.Take(4).ToList();

                return new BaseResponse<DashboardResponse>(new DashboardResponse { Plans = plans, Username = user.Username, dailyLogs = logs});
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<DashboardResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<DashboardResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<DashboardResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
