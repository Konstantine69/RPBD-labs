using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StroitelPodryadchik.Models;

namespace StroitelPodryadchik.Data;

public partial class StroitelPodryadchikContext : DbContext
{
    public StroitelPodryadchikContext()
    {
    }

    public StroitelPodryadchikContext(DbContextOptions<StroitelPodryadchikContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BuildingMaterial> BuildingMaterials { get; set; }

    public virtual DbSet<ConstructionObject> ConstructionObjects { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<ObjectMaterial> ObjectMaterials { get; set; }

    public virtual DbSet<ObjectWork> ObjectWorks { get; set; }

    public virtual DbSet<ViewConstructionObjectsCustomer> ViewConstructionObjectsCustomers { get; set; }

    public virtual DbSet<ViewConstructionObjectsMaterial> ViewConstructionObjectsMaterials { get; set; }

    public virtual DbSet<ViewConstructionObjectsWorkType> ViewConstructionObjectsWorkTypes { get; set; }

    public virtual DbSet<ViewFullConstructionObjectInfo> ViewFullConstructionObjectInfos { get; set; }

    public virtual DbSet<WorkType> WorkTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db8329.databaseasp.net; Database=db8329; User Id=db8329; Password=2Bb_a7@QCx4=; Encrypt=False; MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuildingMaterial>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Building__C50613175593D77B");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.CertificateNumber).HasMaxLength(50);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.MaterialName).HasMaxLength(100);
            entity.Property(e => e.PurchaseVolume).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ConstructionObject>(entity =>
        {
            entity.HasKey(e => e.ObjectId).HasName("PK__Construc__9A6192B117D28CEB");

            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.GeneralContractor).HasMaxLength(100);
            entity.Property(e => e.ObjectName).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.ConstructionObjects)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Construct__Custo__3B75D760");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B83FE62655");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.OrganizationName).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<ObjectMaterial>(entity =>
        {
            entity.HasKey(e => e.ObjectMaterialId).HasName("PK__ObjectMa__707B4AB402336A21");

            entity.Property(e => e.ObjectMaterialId).HasColumnName("ObjectMaterialID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");

            entity.HasOne(d => d.Material).WithMany(p => p.ObjectMaterials)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ObjectMat__Mater__44FF419A");

            entity.HasOne(d => d.Object).WithMany(p => p.ObjectMaterials)
                .HasForeignKey(d => d.ObjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ObjectMat__Objec__440B1D61");
        });

        modelBuilder.Entity<ObjectWork>(entity =>
        {
            entity.HasKey(e => e.ObjectWorkId).HasName("PK__ObjectWo__53959C52A30FB37F");

            entity.ToTable("ObjectWork");

            entity.Property(e => e.ObjectWorkId).HasColumnName("ObjectWorkID");
            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.WorkTypeId).HasColumnName("WorkTypeID");

            entity.HasOne(d => d.Object).WithMany(p => p.ObjectWorks)
                .HasForeignKey(d => d.ObjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ObjectWor__Objec__403A8C7D");

            entity.HasOne(d => d.WorkType).WithMany(p => p.ObjectWorks)
                .HasForeignKey(d => d.WorkTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ObjectWor__WorkT__412EB0B6");
        });

        modelBuilder.Entity<ViewConstructionObjectsCustomer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_ConstructionObjects_Customers");

            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.АдресЗаказчика)
                .HasMaxLength(200)
                .HasColumnName("Адрес заказчика");
            entity.Property(e => e.Генподрядчик).HasMaxLength(100);
            entity.Property(e => e.ГородЗаказчика)
                .HasMaxLength(100)
                .HasColumnName("Город заказчика");
            entity.Property(e => e.ДатаВводаВЭксплуатацию).HasColumnName("Дата ввода в эксплуатацию");
            entity.Property(e => e.ДатаЗаключенияДоговора).HasColumnName("Дата заключения договора");
            entity.Property(e => e.ДатаСдачиОбъекта).HasColumnName("Дата сдачи объекта");
            entity.Property(e => e.Заказчик).HasMaxLength(100);
            entity.Property(e => e.НаименованиеОбъекта)
                .HasMaxLength(100)
                .HasColumnName("Наименование объекта");
            entity.Property(e => e.ПереченьВыполняемыхРабот).HasColumnName("Перечень выполняемых работ");
            entity.Property(e => e.ТелефонЗаказчика)
                .HasMaxLength(20)
                .HasColumnName("Телефон заказчика");
        });

        modelBuilder.Entity<ViewConstructionObjectsMaterial>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_ConstructionObjects_Materials");

            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.ДатаСертификата).HasColumnName("Дата сертификата");
            entity.Property(e => e.НаименованиеМатериала)
                .HasMaxLength(100)
                .HasColumnName("Наименование материала");
            entity.Property(e => e.НаименованиеОбъекта)
                .HasMaxLength(100)
                .HasColumnName("Наименование объекта");
            entity.Property(e => e.НомерСертификата)
                .HasMaxLength(50)
                .HasColumnName("Номер сертификата");
            entity.Property(e => e.ОбъемЗакупки)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Объем закупки");
            entity.Property(e => e.Производитель).HasMaxLength(100);
        });

        modelBuilder.Entity<ViewConstructionObjectsWorkType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_ConstructionObjects_WorkTypes");

            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.ДатаЛицензии).HasColumnName("Дата лицензии");
            entity.Property(e => e.КодВКлассификаторе)
                .HasMaxLength(20)
                .HasColumnName("Код в классификаторе");
            entity.Property(e => e.НаименованиеОбъекта)
                .HasMaxLength(100)
                .HasColumnName("Наименование объекта");
            entity.Property(e => e.НомерЛицензии)
                .HasMaxLength(50)
                .HasColumnName("Номер лицензии");
            entity.Property(e => e.СрокДействияЛицензии).HasColumnName("Срок действия лицензии");
        });

        modelBuilder.Entity<ViewFullConstructionObjectInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Full_ConstructionObject_Info");

            entity.Property(e => e.ObjectId).HasColumnName("ObjectID");
            entity.Property(e => e.АдресЗаказчика)
                .HasMaxLength(200)
                .HasColumnName("Адрес заказчика");
            entity.Property(e => e.Генподрядчик).HasMaxLength(100);
            entity.Property(e => e.ГородЗаказчика)
                .HasMaxLength(100)
                .HasColumnName("Город заказчика");
            entity.Property(e => e.ДатаВводаВЭксплуатацию).HasColumnName("Дата ввода в эксплуатацию");
            entity.Property(e => e.ДатаЗаключенияДоговора).HasColumnName("Дата заключения договора");
            entity.Property(e => e.ДатаСдачиОбъекта).HasColumnName("Дата сдачи объекта");
            entity.Property(e => e.Заказчик).HasMaxLength(100);
            entity.Property(e => e.КодРаботыВКлассификаторе)
                .HasMaxLength(20)
                .HasColumnName("Код работы в классификаторе");
            entity.Property(e => e.НаименованиеМатериала)
                .HasMaxLength(100)
                .HasColumnName("Наименование материала");
            entity.Property(e => e.НаименованиеОбъекта)
                .HasMaxLength(100)
                .HasColumnName("Наименование объекта");
            entity.Property(e => e.НомерЛицензииНаРаботы)
                .HasMaxLength(50)
                .HasColumnName("Номер лицензии на работы");
            entity.Property(e => e.ОбъемЗакупкиМатериала)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Объем закупки материала");
            entity.Property(e => e.ПереченьВыполняемыхРабот).HasColumnName("Перечень выполняемых работ");
            entity.Property(e => e.ПроизводительМатериала)
                .HasMaxLength(100)
                .HasColumnName("Производитель материала");
            entity.Property(e => e.ТелефонЗаказчика)
                .HasMaxLength(20)
                .HasColumnName("Телефон заказчика");
        });

        modelBuilder.Entity<WorkType>(entity =>
        {
            entity.HasKey(e => e.WorkTypeId).HasName("PK__WorkType__CCC06CC09316B0A2");

            entity.Property(e => e.WorkTypeId).HasColumnName("WorkTypeID");
            entity.Property(e => e.ClassifierCode).HasMaxLength(20);
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
