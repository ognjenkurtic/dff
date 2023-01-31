﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dffbackend.Models;

#nullable disable

namespace dffbackend.Migrations
{
    [DbContext(typeof(DffContext))]
    partial class DffContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("dffbackend.Models.FactoringCompany", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("FactoringCompanies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3d45d3e4-f585-46b8-9e6b-721abf7484e4"),
                            ApiKey = "P6xSXlImETYMpojIUHE0e7E12byrqIjYUFzDTLzKBzTWf2qWV57fu2lej8CmQElN",
                            Email = "test@finspot.rs",
                            Name = "Finspot faktor"
                        });
                });

            modelBuilder.Entity("dffbackend.Models.Signature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FactoringCompanyId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Signature1")
                        .HasColumnType("longtext");

                    b.Property<string>("Signature2")
                        .HasColumnType("longtext");

                    b.Property<string>("Signature3")
                        .HasColumnType("longtext");

                    b.Property<string>("Signature4")
                        .HasColumnType("longtext");

                    b.Property<string>("Signature5")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("FactoringCompanyId");

                    b.ToTable("Signatures");
                });

            modelBuilder.Entity("dffbackend.Models.Signature", b =>
                {
                    b.HasOne("dffbackend.Models.FactoringCompany", "FactoringCompany")
                        .WithMany("Signatures")
                        .HasForeignKey("FactoringCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FactoringCompany");
                });

            modelBuilder.Entity("dffbackend.Models.FactoringCompany", b =>
                {
                    b.Navigation("Signatures");
                });
#pragma warning restore 612, 618
        }
    }
}
