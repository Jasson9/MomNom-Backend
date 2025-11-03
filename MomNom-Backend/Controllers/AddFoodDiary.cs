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
using MomNom_Backend.Model.Request;
using MomNom_Backend.Model.Response;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MomNom_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddFoodDiary : ControllerBase
    {
        private readonly MomNomContext _context;

        public AddFoodDiary(MomNomContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<AddDiaryResponse>>> AddFood([FromHeader] string authentication, [FromBody] AddDiaryRequest diaryReq)
        {
            try
            {
                var user = await Auth.ValidateAuthToken(_context, authentication);
                var res = new List<int>();
                var handler = new FoodDetailHandler(_context);
                var planId = _context.MsPlans.Where(e => e.UserId == user.UserId && ((diaryReq.PlanId != null && diaryReq.PlanId == e.PlanId) || e.PlanStatus == "AC")).FirstOrDefault()?.PlanId;

                if (diaryReq?.FoodItems == null || diaryReq.FoodItems.Count() == 0)
                {
                    throw new BadRequestException<AddDiaryResponse>("Food items cannot be empty");
                }

                if(planId == null)
                {
                    throw new BadRequestException<AddDiaryResponse>("Selected plan not found. Please create a plan first.");
                }

                foreach (var item in diaryReq.FoodItems)
                {
                    if (item.FoodName?.Trim() == null) {
                        throw new BadRequestException<AddDiaryResponse>("Food Name cannot be empty");
                    }
                    var temp = await handler.GetFoodDetails(item.FoodName);
                    if (temp?.FoodId == null)
                    {
                        throw new BadRequestException<AddDiaryResponse>($"Food '{item.FoodName}' not found");
                    }
                    item.FoodId = temp.FoodId;
                    res.Add(temp.FoodId);
                }

                foreach (var item in diaryReq.FoodItems)
                {
                    var food = _context.TrDailyCalorieLogs.Where(e => e.FoodId == item.FoodId && e.PlanId == planId && e.UserId == user.UserId && e.Date == DateOnly.FromDateTime(diaryReq.Date ?? DateTime.Now)).FirstOrDefault();

                    if (food != null) {
                        food.Amount = food.Amount + Convert.ToDecimal(item.AmountGr);
                        _context.TrDailyCalorieLogs.Update(food);
                    }
                    else
                    {
                        _context.Add(new TrDailyCalorieLog
                        {
                            FoodId = (int)item.FoodId,
                            UserId = user.UserId,
                            PlanId = (int)planId,
                            Amount = Convert.ToDecimal(item.AmountGr),
                            Date = DateOnly.FromDateTime(diaryReq.Date ?? DateTime.Now),
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return new BaseResponse<AddDiaryResponse>(new AddDiaryResponse
                {
                     foodIds = res
                });
            }
            catch (FetchNutritionNotFoundException<MsFood> e)
            {
                return new NotFoundException<AddDiaryResponse>(e.ErrorMessage).toResponseOutput();
            }
            catch (UnauthorizedException<MsUser> ex)
            {
                return new UnauthorizedException<AddDiaryResponse>(ex.ErrorMessage).toResponseOutput();
            }
            catch (BaseException<AddDiaryResponse> ex)
            {
                return ex.toResponseOutput();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new InternalServerErrorException<AddDiaryResponse>("Unexpected internal server error occured").toResponseOutput();
            }

        }

    }
}
