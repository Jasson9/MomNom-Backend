using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class TrWeeklyExercise
{
    public int WeekId { get; set; }

    public int PlanId { get; set; }

    public int UserId { get; set; }

    public int? TotalMinute { get; set; }

    public string? Description { get; set; }

    public virtual MsPlan MsPlan { get; set; } = null!;
}
