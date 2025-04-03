﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WFE.Client.Persistence;

#nullable disable

namespace WFE.Client.Migrations
{
    [DbContext(typeof(ClientDbContext))]
    [Migration("20250330153459_UpdateFluentFields")]
    partial class UpdateFluentFields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflowActor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ActorEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActorEmployeeCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActorFullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActorUsername")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("bit");

                    b.Property<int>("Order")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<Guid>("WorkflowStepId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowStepId");

                    b.ToTable("WorkflowActors");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflowStep", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ApprovalType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Condition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilterMode")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("SoftWarn");

                    b.Property<string>("StepName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StepOrder")
                        .HasColumnType("int");

                    b.Property<Guid?>("WorkflowId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowId");

                    b.ToTable("WorkflowSteps");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.LeaveRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("RequestedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VacationDays")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("LeaveRequests");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflowActor", b =>
                {
                    b.HasOne("WFE.Client.Domain.Entities.ClientWorkflowStep", "Step")
                        .WithMany("Actors")
                        .HasForeignKey("WorkflowStepId");

                    b.Navigation("Step");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflowStep", b =>
                {
                    b.HasOne("WFE.Client.Domain.Entities.ClientWorkflow", "Workflow")
                        .WithMany("Steps")
                        .HasForeignKey("WorkflowId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Workflow");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflow", b =>
                {
                    b.Navigation("Steps");
                });

            modelBuilder.Entity("WFE.Client.Domain.Entities.ClientWorkflowStep", b =>
                {
                    b.Navigation("Actors");
                });
#pragma warning restore 612, 618
        }
    }
}
