namespace MomNom_Backend.Model.Request
{
    public class DiaryFoodItemRequest
    {
        public string FoodName { get; set; }
        public double? AmountGr { get; set; }
        public string? FdcId { get; set; } // optional
        public int? FoodId;
    }

    public class AddDiaryRequest
    {
        public DateTime? Date { get; set; }
        public int? PlanId { get; set; } // optional
        public List<DiaryFoodItemRequest>? FoodItems { get; set; }
    }
}
