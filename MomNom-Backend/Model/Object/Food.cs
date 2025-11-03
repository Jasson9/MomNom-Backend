using Newtonsoft.Json;

namespace MomNom_Backend.Model.Object
{
    public class Food
    {
        public int FoodId { get; set; }
        public string? FoodName { get; set; }
        public double? AmountGr { get; set; }
        public List<Nutrient>? Nutrients { get; set; }
        public string? FdcId { get; set; } //optional
    }
}
