using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Engine.Migrations
{
    /// <inheritdoc />
    public partial class Update31032025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionString",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "DbType",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "ConnectionString",
                table: "WorkflowRule");

            migrationBuilder.DropColumn(
                name: "DbType",
                table: "WorkflowRule");

            migrationBuilder.AddColumn<string>(
                name: "DbType",
                table: "WorkflowInstances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EncryptedConnectionString",
                table: "WorkflowInstances",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DbType",
                table: "WorkflowInstances");

            migrationBuilder.DropColumn(
                name: "EncryptedConnectionString",
                table: "WorkflowInstances");

            migrationBuilder.AddColumn<string>(
                name: "ConnectionString",
                table: "WorkflowSteps",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DbType",
                table: "WorkflowSteps",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConnectionString",
                table: "WorkflowRule",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DbType",
                table: "WorkflowRule",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
