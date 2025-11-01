using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
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
        public async Task<ActionResult<BaseResponse<DashboardResponse>>> Dashboards([FromHeader] string authentication)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && e.planStatus == "AC").Count();
                DateOnly date = DateOnly.FromDateTime(DateTime.Now);

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

                var dashboardNutrients = new List<string>
                {
                    "Calorie",
                    "Carbohydrate",
                    "Protein",
                    "Fiber"
                };

                List<NutrientPlanProgress> nutrientPlanProgress = await _procedureHandler.GetDailyNutritionReport(user.UserId, planId, date);
                List<NutrientPlanProgress> nutrients = nutrientPlanProgress.Where(x => dashboardNutrients.Contains(x.nutrientName)).ToList();
                var remainingNutritions = nutrients.Select(x => new
                {
                    x.nutrientName,
                    remainingNutrient = x.goalAmount - x.nutrientAmount
                }).ToList();

                List<WeightGain> weightGainList = await _procedureHandler.GetWeightGainReport(user.UserId, planId, date);
                var currWeightGain = weightGainList.OrderBy(x => x.year).ThenBy(x => x.monthNumber).Select(x => new WeightGainCalc
                {
                    MonthYear = x.monthName,
                    MonthlyGain = x.monthlyGain,
                    TotalGain = x.totalGain,
                    Percentage = Math.Round(((x.totalGain / x.recGain) * 100), 2) + "%"
                }).FirstOrDefault();

                List<DailyLog> dailyLog = await _procedureHandler.GetDailyFoodReport(user.UserId, planId, date);
                List<DailyLog> logs = dailyLog.Take(4).ToList();

                //Mungkin akan nambah
                var tipsList = new List<string>
                {
                    "Make sure you receive at least four antenatal (before-birth) visits from a health care professional",
                    "Ask your health worker about when to come for antenatal care",
                    "Learn about danger signs in pregnancy like bleeding, fever, swollen hands and feet, and blurred vision",
                    "To avoid complications, plan for your baby to be delivered at a health care facility by a skilled birth attendant, which can include a midwife, nurse or physician",
                    "Women go through emotional, physical and psychological changes like gaining weight and losing self-confidence during and after pregnancy. It's fine to seek for help :)",
                    "Speak to a family member or friend about what you’re going through",
                    "Join a mothers group or an association of women with children to connect with other mothers",
                    "It is advised to put the child on the mother’s chest immediately after birth",
                    "Gently soothe, stroke and hold your child, smiling and talking to the baby at this time are good for stimulation",
                    "Aim for at least eight hours of sleep every night. Resting on the left or right side will keep blood flowing well to the baby and ease swelling",
                    "Avoid nausea triggers. Whether it's the smell of foods in the break room or other odors or tastes"
                };
                var random = new Random();
                var randomTips = tipsList.OrderBy(x => random.Next()).Take(4).ToList();

                return new BaseResponse<DashboardResponse>(new DashboardResponse { Plans = plans, Username = user.Username, RemainingNutritions = remainingNutritions, CurrWeightGain = currWeightGain , DailyLogs = logs, TipsList = randomTips});
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
