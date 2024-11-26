using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using ProdajnikWeb.Models;

namespace ProdajnikWeb.Data;

public partial class ProdajnikContext : DbContext
{
    public ProdajnikContext()
    {
    }

    public ProdajnikContext(DbContextOptions<ProdajnikContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BuildingMaterial> BuildingMaterials { get; set; }

    public virtual DbSet<ConstructionObject> ConstructionObjects { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ObjectMaterial> ObjectMaterials { get; set; }

    public virtual DbSet<ObjectWork> ObjectWorks { get; set; }


    public virtual DbSet<WorkType> WorkTypes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка первичного ключа для BuildingMaterial
        modelBuilder.Entity<BuildingMaterial>()
            .HasKey(bm => bm.MaterialId);

        // Настройка других сущностей (ConstructionObject, Customer и т.д.)
        modelBuilder.Entity<ConstructionObject>()
        .HasKey(co => co.ObjectId);
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerId);
        modelBuilder.Entity<ObjectMaterial>()
            .HasKey(om => om.ObjectMaterialId);
        modelBuilder.Entity<ObjectWork>()
            .HasKey(ow => ow.ObjectWorkId);
        modelBuilder.Entity<WorkType>()
            .HasKey(wt => wt.WorkTypeId);

        base.OnModelCreating(modelBuilder);
    }


}