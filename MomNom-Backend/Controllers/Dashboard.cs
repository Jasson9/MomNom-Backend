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
                var plan = _context.MsPlans.Where(e => e.UserId == user.UserId && e.PlanStatus == "AC").FirstOrDefault();
                DateOnly date = DateOnly.FromDateTime(DateTime.Now);

                // return empty plan, front end will redirect to create plan page
                if (plan == null)
                {
                    return new BaseResponse<DashboardResponse>(
                    new DashboardResponse{
                        Plans = new List<Plan>(),
                        Username = user.Username,
                        RemainingNutritions = new List<object>(),
                        CurrWeightGain = null,
                        DailyLogs = new List<DailyLog>(),
                    });
                }

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

                List<NutrientPlanProgress> nutrientPlanProgress = await _procedureHandler.GetDailyNutritionReport(user.UserId, plan.PlanId, date);
                List<NutrientPlanProgress> nutrients = nutrientPlanProgress.Where(x => dashboardNutrients.Contains(x.nutrientName)).ToList();
                var remainingNutritions = nutrients.Select(x => new
                {
                    x.nutrientName,
                    remainingNutrient = x.goalAmount - x.nutrientAmount,
                    x.unit
                }).ToList();

                List<WeightGain> weightGainList = await _procedureHandler.GetWeightGainReport(user.UserId, plan.PlanId, date);

                var currWeightGain = weightGainList.OrderByDescending(x => x.year).ThenByDescending(x => x.monthNumber).Select(x => new WeightGainCalc
                {
                    MonthYear = x.monthName,
                    MonthlyGain = x.monthlyGain,
                    TotalGain = x.totalGain,
                    RecGain = x.recGain,
                    Percentage = Math.Round(((x.totalGain / x.recGain) * 100), 2).ToString()
                }).FirstOrDefault();

                List<DailyLog> dailyLog = await _procedureHandler.GetDailyFoodReport(user.UserId, plan.PlanId, date);
                //List<DailyLog> logs = dailyLog.Take(4).ToList();
                List<DailyLog> logs = dailyLog.Select((e) => {
                    var temp= new DailyLog
                    {
                        Amount = e.Amount,
                        FoodName = e.FoodName,
                        TotalCalories = e.TotalCalories,
                        NutrientsListDetail = e.NutrientsListDetail.Where(x => dashboardNutrients.Contains(x.nutrientName)).ToList()
                    };
                    temp.NutrientsListDetail.Add(new Model.Object.Nutrient
                    {
                        amount = temp.TotalCalories,
                        nutrientName = "Calorie",
                        unit = "kcal"
                    });

                    return temp;
                } ).ToList();

                return new BaseResponse<DashboardResponse>(new DashboardResponse { Plans = plans, Username = user.Username, RemainingNutritions = remainingNutritions, CurrWeightGain = currWeightGain , DailyLogs = logs});
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
