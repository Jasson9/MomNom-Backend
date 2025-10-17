using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class TrMonthlyWeight
{
    public int PlanId { get; set; }

    public int UserId { get; set; }

    public decimal? Weight { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public virtual MsPlan MsPlan { get; set; } = null!;
}
