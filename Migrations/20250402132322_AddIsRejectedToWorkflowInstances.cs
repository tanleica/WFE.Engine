using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Engine.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRejectedToWorkflowInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "WorkflowInstances",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "WorkflowInstances");
        }
    }
}
