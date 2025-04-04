using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Engine.Migrations
{
    /// <inheritdoc />
    public partial class AddStepAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StepAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkflowStepId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReplacedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepAssignments_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepAssignments_WorkflowStepId",
                table: "StepAssignments",
                column: "WorkflowStepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepAssignments");
        }
    }
}
