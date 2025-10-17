using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsRecommendedWeight
{
    public string BmiCategory { get; set; } = null!;

    public decimal? MinWeight { get; set; }

    public decimal? MaxWeight { get; set; }

    public virtual ICollection<MsPlan> MsPlans { get; set; } = new List<MsPlan>();
}
