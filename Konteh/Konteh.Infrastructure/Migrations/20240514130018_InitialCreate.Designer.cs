﻿// <auto-generated />
using System;
using Konteh.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Konteh.Infrastructure.Migrations;

[DbContext(typeof(KontehContext))]
[Migration("20240514130018_InitialCreate")]
partial class InitialCreate
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.4")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Answer", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<bool>("IsCorrect")
                    .HasColumnType("bit");

                b.Property<int?>("QuestionId")
                    .HasColumnType("int");

                b.Property<string>("Text")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("QuestionId");

                b.ToTable("Answers");
            });

        modelBuilder.Entity("Question", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<int>("Category")
                    .HasColumnType("int");

                b.Property<string>("QuestionText")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("Type")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.ToTable("Questions");
            });

        modelBuilder.Entity("Answer", b =>
            {
                b.HasOne("Question", null)
                    .WithMany("Answers")
                    .HasForeignKey("QuestionId");
            });

        modelBuilder.Entity("Question", b =>
            {
                b.Navigation("Answers");
            });
#pragma warning restore 612, 618
    }
}
