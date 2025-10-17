using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsFoodNutrient
{
    public int FoodId { get; set; }

    public int NutrientId { get; set; }

    public decimal Amount { get; set; }

    public virtual MsFood Food { get; set; } = null!;

    public virtual MsNutrient Nutrient { get; set; } = null!;
}
