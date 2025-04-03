using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Engine.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkflowInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "WorkflowInstances",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "WorkflowInstances");
        }
    }
}
