using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model;
using MomNom_Backend.Model.Db;
using MomNom_Backend.Model.Exception;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Response;
using MySqlConnector;
using Newtonsoft.Json;
using System.Text.Json;

namespace MomNom_Backend.Handler
{
    public class FoodDetailHandler
    {
        private readonly MomNomContext _context;

        public FoodDetailHandler(MomNomContext context)
        {
            _context = context;
        }

        public async Task<MsFood> FetchNutritionsFromEdamam(string foodName)
        {
            using HttpClient client = new HttpClient();
            Edamam secrets = AppSettings.Secrets.Edamam;
            var url = $"https://api.edamam.com/api/nutrition-data?app_id={secrets.AppId}&app_key={secrets.AppKey}&nutrition-type=logging&ingr={foodName}";

            try
            {
                // Send the GET request asynchronously
                HttpResponseMessage response = await client.GetAsync(url);

                // Ensure the request was successful (status code 2xx)
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string responseBody = await response.Content.ReadAsStringAsync();

                var res = JsonConvert.DeserializeObject<EdamamNutritionDataResponse>(responseBody);
                if(res.Ingredients[0].Parsed == null)
                {
                    throw new FetchNutritionNotFoundException<MsFood>($"Food item '{foodName}' not found in Edamam database.");
                }
                var parsed = res.Ingredients[0].Parsed[0];
                var weight = parsed.Weight;
                double? calorie = (parsed.Nutrients.GetValueOrDefault("ENERC_KCAL")?.Quantity ?? 0) / weight;
                double? protein = (parsed.Nutrients.GetValueOrDefault("PROCNT")?.Quantity ?? 0) / weight;
                double? carbohydrate = (parsed.Nutrients.GetValueOrDefault("CHOCDF")?.Quantity ?? 0) / weight;
                double? fiber = (parsed.Nutrients.GetValueOrDefault("FIBTG")?.Quantity ?? 0) / weight;
                double? vitaminA = (parsed.Nutrients.GetValueOrDefault("VITA_RAE")?.Quantity ?? 0) / weight;
                double? vitaminC = (parsed.Nutrients.GetValueOrDefault("VITC")?.Quantity ?? 0) / weight;
                double? vitaminD = (parsed.Nutrients.GetValueOrDefault("VITD")?.Quantity ?? 0) / weight;
                double? calcium = (parsed.Nutrients.GetValueOrDefault("CA")?.Quantity ?? 0) / weight;
                double? iron = (parsed.Nutrients.GetValueOrDefault("FE")?.Quantity ?? 0) / weight;
                double? zinc = (parsed.Nutrients.GetValueOrDefault("ZN")?.Quantity ?? 0) / weight;

                var foodRes = _context.MsFoods.Add(new MsFood
                {
                    FoodName = foodName,
                    Calorie = Convert.ToDecimal(calorie),   
                    WeightPerServing = Convert.ToDecimal(weight)
                });

                await _context.SaveChangesAsync();

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(protein), NutrientId=1, FoodId = foodRes.Entity.FoodId }
                    );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(carbohydrate), NutrientId = 2, FoodId = foodRes.Entity.FoodId }
                    );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(fiber), NutrientId = 3, FoodId = foodRes.Entity.FoodId }
                    );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(vitaminA), NutrientId = 5, FoodId = foodRes.Entity.FoodId }
                 );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(vitaminC), NutrientId = 6, FoodId = foodRes.Entity.FoodId }
                 );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(vitaminD), NutrientId = 7, FoodId = foodRes.Entity.FoodId }
                 );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(calcium), NutrientId = 8, FoodId = foodRes.Entity.FoodId }
                 );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(iron), NutrientId = 9, FoodId = foodRes.Entity.FoodId }
                 );

                _context.MsFoodNutrients.Add(
                    new MsFoodNutrient { Amount = Convert.ToDecimal(zinc), NutrientId = 10, FoodId = foodRes.Entity.FoodId }
                 );

                await _context.SaveChangesAsync();
                return foodRes.Entity;
            }
            catch (FetchNutritionNotFoundException<MsFood> e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FetchNutritionException<MsFood>($"Unexpected error on Fetch Nutrition: {e.Message}");
            }
        }

        public async Task<Model.Object.Food> GetFoodDetails(string foodName)
        {
            var foodObj = _context.MsFoods.Where(e=> e.FoodName == foodName.Trim()).FirstOrDefault();
            if (foodObj == null)
            {
                foodObj = await FetchNutritionsFromEdamam(foodName);
            }

            var nutrients = _context.MsFoodNutrients
                .Where(e => e.FoodId == foodObj.FoodId)
                .Include(e => e.Nutrient)
                .Select(e => new Model.Object.Nutrient
                {
                    nutrientName = e.Nutrient.NutrientName,
                    amount = e.Amount,
                    unit = e.Nutrient.Unit
                })
                .ToList();

            return new Food {
                FoodId = foodObj.FoodId,
                FoodName = foodObj.FoodName,
                AmountGr = Convert.ToDouble(foodObj.WeightPerServing),
                Nutrients = nutrients
            };
        }
    }
}
