using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsFood
{
    public int FoodId { get; set; }

    public string FoodName { get; set; } = null!;

    public decimal Calorie { get; set; }

    public virtual ICollection<MsFoodNutrient> MsFoodNutrients { get; set; } = new List<MsFoodNutrient>();

    public virtual ICollection<TrDailyCalorieLog> TrDailyCalorieLogs { get; set; } = new List<TrDailyCalorieLog>();
}
