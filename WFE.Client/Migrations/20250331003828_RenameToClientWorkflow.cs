using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Client.Migrations
{
    /// <inheritdoc />
    public partial class RenameToClientWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSteps_Workflows_WorkflowId",
                table: "WorkflowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Workflows",
                table: "Workflows");

            migrationBuilder.RenameTable(
                name: "Workflows",
                newName: "ClientWorkflows");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientWorkflows",
                table: "ClientWorkflows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSteps_ClientWorkflows_WorkflowId",
                table: "WorkflowSteps",
                column: "WorkflowId",
                principalTable: "ClientWorkflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSteps_ClientWorkflows_WorkflowId",
                table: "WorkflowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientWorkflows",
                table: "ClientWorkflows");

            migrationBuilder.RenameTable(
                name: "ClientWorkflows",
                newName: "Workflows");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workflows",
                table: "Workflows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSteps_Workflows_WorkflowId",
                table: "WorkflowSteps",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
