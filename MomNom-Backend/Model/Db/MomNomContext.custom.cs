using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using MomNom_Backend.Model.Object;
using MomNom_Backend.Model.Response;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MomNom_Backend.Model.Db;

public partial class MomNomContext : DbContext
{
    //ProcedureTestOutput result etc here...
    public DbSet<ProcedureTestOutput> procedureTestResults { get; set; }

    public DbSet<DailyLog> GetDailyFoodDetail {  get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcedureTestOutput>().HasNoKey();
    }


    //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
