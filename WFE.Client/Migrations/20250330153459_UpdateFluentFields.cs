using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Client.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFluentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowActors_WorkflowSteps_WorkflowStepId",
                table: "WorkflowActors");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkflowId",
                table: "WorkflowSteps",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "FilterMode",
                table: "WorkflowSteps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SoftWarn",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowActors_WorkflowSteps_WorkflowStepId",
                table: "WorkflowActors",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowActors_WorkflowSteps_WorkflowStepId",
                table: "WorkflowActors");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkflowId",
                table: "WorkflowSteps",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilterMode",
                table: "WorkflowSteps",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SoftWarn");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowActors_WorkflowSteps_WorkflowStepId",
                table: "WorkflowActors",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
