using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MomNom_Backend.Model.Db;

public partial class MomNomContext : DbContext
{
    public MomNomContext()
    {
    }

    public MomNomContext(DbContextOptions<MomNomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MsFood> MsFoods { get; set; }

    public virtual DbSet<MsFoodNutrient> MsFoodNutrients { get; set; }

    public virtual DbSet<MsNutrient> MsNutrients { get; set; }

    public virtual DbSet<MsPlan> MsPlans { get; set; }

    public virtual DbSet<MsRecommendedWeight> MsRecommendedWeights { get; set; }

    public virtual DbSet<MsUser> MsUsers { get; set; }

    public virtual DbSet<TrDailyCalorieLog> TrDailyCalorieLogs { get; set; }

    public virtual DbSet<TrMonthlyWeight> TrMonthlyWeights { get; set; }

    public virtual DbSet<TrUserSession> TrUserSessions { get; set; }

    public virtual DbSet<TrWeeklyExercise> TrWeeklyExercises { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.6-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<MsFood>(entity =>
        {
            entity.HasKey(e => e.FoodId).HasName("PRIMARY");

            entity
                .ToTable("MsFood")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.FoodId).HasColumnName("foodId");
            entity.Property(e => e.Calorie)
                .HasPrecision(10, 2)
                .HasColumnName("calorie");
            entity.Property(e => e.FoodName)
                .HasMaxLength(100)
                .HasColumnName("foodName");
        });

        modelBuilder.Entity<MsFoodNutrient>(entity =>
        {
            entity.HasKey(e => new { e.FoodId, e.NutrientId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("MsFoodNutrient");

            entity.HasIndex(e => e.NutrientId, "fk_msfoodnutrient_msnutrient_nutrientid");

            entity.Property(e => e.FoodId).HasColumnName("foodId");
            entity.Property(e => e.NutrientId).HasColumnName("nutrientId");
            entity.Property(e => e.Amount)
                .HasPrecision(8, 2)
                .HasColumnName("amount");

            entity.HasOne(d => d.Food).WithMany(p => p.MsFoodNutrients)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_msfoodnutrient_msfood_foodid");

            entity.HasOne(d => d.Nutrient).WithMany(p => p.MsFoodNutrients)
                .HasForeignKey(d => d.NutrientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_msfoodnutrient_msnutrient_nutrientid");
        });

        modelBuilder.Entity<MsNutrient>(entity =>
        {
            entity.HasKey(e => e.NutrientId).HasName("PRIMARY");

            entity
                .ToTable("MsNutrient")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.NutrientId).HasColumnName("nutrientId");
            entity.Property(e => e.NutrientName)
                .HasMaxLength(100)
                .HasColumnName("nutrientName");
            entity.Property(e => e.RecommendedAmount)
                .HasPrecision(10, 2)
                .HasColumnName("recommendedAmount");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .HasColumnName("unit");
        });

        modelBuilder.Entity<MsPlan>(entity =>
        {
            entity.HasKey(e => new { e.PlanId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity
                .ToTable("MsPlan")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.BmiCategory, "fk_msplan_msrecommendedweight_bmicategory");

            entity.HasIndex(e => e.UserId, "fk_msplan_msuser_userid");

            entity.Property(e => e.PlanId).HasColumnName("planId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.BmiCategory)
                .HasMaxLength(20)
                .HasColumnName("bmiCategory");
            entity.Property(e => e.CalFirstTrimester)
                .HasPrecision(10, 2)
                .HasColumnName("calFirstTrimester");
            entity.Property(e => e.CalSecondThirdTrimester)
                .HasPrecision(10, 2)
                .HasColumnName("calSecondThirdTrimester");
            entity.Property(e => e.Height)
                .HasPrecision(10, 2)
                .HasColumnName("height");
            entity.Property(e => e.PrePregnancyWeight)
                .HasPrecision(10, 2)
                .HasColumnName("prePregnancyWeight");
            entity.Property(e => e.StartWeek).HasColumnName("startWeek");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.BmiCategoryNavigation).WithMany(p => p.MsPlans)
                .HasForeignKey(d => d.BmiCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_msplan_msrecommendedweight_bmicategory");

            entity.HasOne(d => d.User).WithMany(p => p.MsPlans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_msplan_msuser_userid");
        });

        modelBuilder.Entity<MsRecommendedWeight>(entity =>
        {
            entity.HasKey(e => e.BmiCategory).HasName("PRIMARY");

            entity
                .ToTable("MsRecommendedWeight")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.BmiCategory)
                .HasMaxLength(20)
                .HasColumnName("bmiCategory");
            entity.Property(e => e.MaxWeight)
                .HasPrecision(10, 2)
                .HasColumnName("maxWeight");
            entity.Property(e => e.MinWeight)
                .HasPrecision(10, 2)
                .HasColumnName("minWeight");
        });

        modelBuilder.Entity<MsUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity
                .ToTable("MsUser")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordHash");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<TrDailyCalorieLog>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PlanId, e.FoodId, e.Date })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity
                .ToTable("TrDailyCalorieLog")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.FoodId, "fk_day_food2");

            entity.HasIndex(e => new { e.PlanId, e.UserId }, "fk_day_plan2");

            entity.HasIndex(e => e.UserId, "fk_day_user2");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.PlanId).HasColumnName("planId");
            entity.Property(e => e.FoodId).HasColumnName("foodId");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");

            entity.HasOne(d => d.Food).WithMany(p => p.TrDailyCalorieLogs)
                .HasForeignKey(d => d.FoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trdailycalorielog_msfood_foodid");

            entity.HasOne(d => d.MsPlan).WithMany(p => p.TrDailyCalorieLogs)
                .HasForeignKey(d => new { d.PlanId, d.UserId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trdailycalorielog_msplan_planid_userid");
        });

        modelBuilder.Entity<TrMonthlyWeight>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PlanId, e.Month, e.Year })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity
                .ToTable("TrMonthlyWeight")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => new { e.PlanId, e.UserId }, "fk_mon_plan2");

            entity.HasIndex(e => e.UserId, "fk_mon_user2");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.PlanId).HasColumnName("planId");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.Weight)
                .HasPrecision(10, 2)
                .HasColumnName("weight");

            entity.HasOne(d => d.MsPlan).WithMany(p => p.TrMonthlyWeights)
                .HasForeignKey(d => new { d.PlanId, d.UserId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trmonthlyweight_msplan_planid_userid");
        });

        modelBuilder.Entity<TrUserSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PRIMARY");

            entity
                .ToTable("TrUserSession")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.UserId, "fk_trusersession_msuser_userid");

            entity.Property(e => e.SessionId)
                .HasMaxLength(128)
                .HasColumnName("sessionId");
            entity.Property(e => e.CreateDateTime)
                .HasDefaultValueSql("now()")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDateTime)
                .HasDefaultValueSql("(now() + interval 1 month)")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.TrUserSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_trusersession_msuser_userid");
        });

        modelBuilder.Entity<TrWeeklyExercise>(entity =>
        {
            entity.HasKey(e => new { e.WeekId, e.PlanId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity
                .ToTable("TrWeeklyExercise")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => new { e.PlanId, e.UserId }, "fk_wk_plan2");

            entity.HasIndex(e => e.UserId, "fk_wk_user2");

            entity.Property(e => e.WeekId)
                .ValueGeneratedOnAdd()
                .HasColumnName("weekId");
            entity.Property(e => e.PlanId).HasColumnName("planId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.TotalMinute).HasColumnName("totalMinute");

            entity.HasOne(d => d.MsPlan).WithMany(p => p.TrWeeklyExercises)
                .HasForeignKey(d => new { d.PlanId, d.UserId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_trweeklyexercise_msplan_planid_userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
