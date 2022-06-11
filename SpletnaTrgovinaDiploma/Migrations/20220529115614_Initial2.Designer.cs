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
    [Migration("20220529115614_Initial2")]
    partial class Initial2
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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ProfilePictureURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Brand_Item", b =>
                {
                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.HasKey("BrandId", "ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("Brands_Items");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ItemCategory")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Brand_Item", b =>
                {
                    b.HasOne("SpletnaTrgovinaDiploma.Models.Brand", "Brand")
                        .WithMany("Brands_Items")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SpletnaTrgovinaDiploma.Models.Item", "Item")
                        .WithMany("Brands_Items")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Brand", b =>
                {
                    b.Navigation("Brands_Items");
                });

            modelBuilder.Entity("SpletnaTrgovinaDiploma.Models.Item", b =>
                {
                    b.Navigation("Brands_Items");
                });
#pragma warning restore 612, 618
        }
    }
}
