using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Engine.Migrations
{
    /// <inheritdoc />
    public partial class update3032025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanActorVote",
                table: "StepProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConditionPassed",
                table: "StepProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FilterMode",
                table: "StepProgresses",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkflowRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuleName = table.Column<string>(type: "text", nullable: false),
                    ConditionScript = table.Column<string>(type: "text", nullable: false),
                    DbType = table.Column<int>(type: "integer", nullable: false),
                    ConnectionString = table.Column<string>(type: "text", nullable: false),
                    LogicalOperator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowRule_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowRule_WorkflowStepId",
                table: "WorkflowRule",
                column: "WorkflowStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowRule");

            migrationBuilder.DropColumn(
                name: "CanActorVote",
                table: "StepProgresses");

            migrationBuilder.DropColumn(
                name: "ConditionPassed",
                table: "StepProgresses");

            migrationBuilder.DropColumn(
                name: "FilterMode",
                table: "StepProgresses");
        }
    }
}
