using System;
using System.Collections.Generic;

namespace MomNom_Backend.Model.Db;

public partial class MsPlan
{
    public int PlanId { get; set; }

    public int UserId { get; set; }

    public string? planStatus {  get; set; }

    public int? StartWeek { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public int? Age { get; set; }

    public decimal? PrePregnancyWeight { get; set; }

    public string BmiCategory { get; set; } = null!;

    public decimal? CalFirstTrimester { get; set; }

    public decimal? CalSecondThirdTrimester { get; set; }

    public virtual MsRecommendedWeight BmiCategoryNavigation { get; set; } = null!;

    public virtual ICollection<TrDailyCalorieLog> TrDailyCalorieLogs { get; set; } = new List<TrDailyCalorieLog>();

    public virtual ICollection<TrMonthlyWeight> TrMonthlyWeights { get; set; } = new List<TrMonthlyWeight>();

    public virtual ICollection<TrWeeklyExercise> TrWeeklyExercises { get; set; } = new List<TrWeeklyExercise>();

    public virtual MsUser User { get; set; } = null!;
}
