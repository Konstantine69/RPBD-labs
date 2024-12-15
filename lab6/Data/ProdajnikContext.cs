using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using lab6.Models;

namespace lab6.Data;

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

    public virtual DbSet<ObjectWork> ObjectWork { get; set; }



    public virtual DbSet<WorkType> WorkTypes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Настройка первичных ключей
        modelBuilder.Entity<BuildingMaterial>()
            .HasKey(bm => bm.MaterialId);

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

        // Настройка связей
        modelBuilder.Entity<ConstructionObject>()
            .HasMany(co => co.ObjectWorks) // У ConstructionObject много ObjectWorks
            .WithOne(ow => ow.Object) // У ObjectWork одна связь с ConstructionObject
            .HasForeignKey(ow => ow.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ConstructionObject>()
            .HasMany(co => co.ObjectMaterials) // У ConstructionObject много ObjectMaterials
            .WithOne(om => om.Object) // У ObjectMaterial одна связь с ConstructionObject
            .HasForeignKey(om => om.ObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ObjectMaterial>()
            .HasOne(om => om.Material) // У ObjectMaterial один BuildingMaterial
            .WithMany(bm => bm.ObjectMaterials) // У BuildingMaterial много ObjectMaterials
            .HasForeignKey(om => om.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ObjectWork>()
            .HasOne(ow => ow.WorkType) // У ObjectWork один WorkType
            .WithMany(wt => wt.ObjectWorks) // У WorkType много ObjectWorks
            .HasForeignKey(ow => ow.WorkTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }



}