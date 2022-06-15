﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpletnaTrgovinaDiploma.Data;

namespace SpletnaTrgovinaDiploma.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220528160820_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Brand", b =>
                {
                    b.Property<int>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrandId");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.BrandItem", b =>
                {
                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.HasKey("BrandId", "ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("BrandsItems");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ItemCategory")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("ItemId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.BrandItem", b =>
                {
                    b.HasOne("SpletnaTrgovinaDiploma.Models.Brand", "Brand")
                        .WithMany("BrandsItems")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SpletnaTrgovinaDiploma.Models.Item", "Item")
                        .WithMany("BrandsItems")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Brand", b =>
                {
                    b.Navigation("BrandsItems");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Item", b =>
                {
                    b.Navigation("BrandsItems");
                });
#pragma warning restore 612, 618
        }
    }
}
