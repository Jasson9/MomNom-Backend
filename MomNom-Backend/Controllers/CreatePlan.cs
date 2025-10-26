using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using MomNom_Backend;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatePlan : ControllerBase
    {
        private readonly MomNomContext _context;

        public CreatePlan(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<CreatePlanResponse>>> LoginUser([FromHeader] string authentication, [FromBody] CreatePlanRequest planReq )
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
               
                DateTime.TryParse(planReq.DOBstring, out DateTime dateOfBirth);
                if (dateOfBirth == null)
                {
                    throw new BadRequestException<CreatePlanResponse>("Invalid Date Of Birth");
                }

                if (planReq.weekPregnancy == 0)
                {
                    throw new BadRequestException<CreatePlanResponse>("Invalid Pregnancy Week");
                }

                if (planReq.height == 0)
                {
                    throw new BadRequestException<CreatePlanResponse>("Invalid Height");
                }

                if (planReq.currentWeight == 0)
                {
                    throw new BadRequestException<CreatePlanResponse>("Invalid Current Weight");
                }

                if (planReq.prePregnancyWeight == 0)
                {
                    throw new BadRequestException<CreatePlanResponse>("Invalid Pre Pregnancy Weight");
                }

                var today = DateTime.Today;

                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age)) age--;

                // TODO: Add BMI Calculate, calorie, etc here or use trigger instead
                string bmiCategory = "Normal";
                decimal calFirstTrimester = 200;
                decimal calSecondTrimester = 300;

                var planCnt = _context.MsPlans.Where(e => e.UserId == user.UserId).Count();

                var plan = _context.MsPlans.Add(
                    new MsPlan
                        {
                            Age = age,
                            PlanId = planCnt,
                            StartWeek = planReq.weekPregnancy,
                            Weight = planReq.currentWeight,
                            PrePregnancyWeight = planReq.prePregnancyWeight,
                            Height = planReq.height,
                            UserId = user.UserId,
                            CalFirstTrimester = calFirstTrimester,
                            BmiCategory = bmiCategory,
                            CalSecondThirdTrimester = calSecondTrimester,
                        }
                    );
                await _context.SaveChangesAsync();

                return new BaseResponse<CreatePlanResponse>(new CreatePlanResponse { planId = plan.Entity.PlanId});
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<CreatePlanResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<CreatePlanResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<CreatePlanResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
