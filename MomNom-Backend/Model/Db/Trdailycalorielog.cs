using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class TrDailyCalorieLog
{
    public int PlanId { get; set; }

    public int UserId { get; set; }

    public int FoodId { get; set; }

    public decimal? Amount { get; set; }

    public DateOnly Date { get; set; }

    public virtual MsFood Food { get; set; } = null!;

    public virtual MsPlan MsPlan { get; set; } = null!;
}
