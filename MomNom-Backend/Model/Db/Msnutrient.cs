using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsNutrient
{
    public int NutrientId { get; set; }

    public string NutrientName { get; set; } = null!;

    public decimal? RecommendedAmount { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<MsFoodNutrient> MsFoodNutrients { get; set; } = new List<MsFoodNutrient>();
}
