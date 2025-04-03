using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFE.Client.Migrations
{
    /// <inheritdoc />
    public partial class RenameActorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "WorkflowActors");

            migrationBuilder.RenameColumn(
                name: "ActorName",
                table: "WorkflowActors",
                newName: "ActorUsername");

            migrationBuilder.AddColumn<string>(
                name: "ActorEmail",
                table: "WorkflowActors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActorEmployeeCode",
                table: "WorkflowActors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ActorFullName",
                table: "WorkflowActors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActorEmail",
                table: "WorkflowActors");

            migrationBuilder.DropColumn(
                name: "ActorEmployeeCode",
                table: "WorkflowActors");

            migrationBuilder.DropColumn(
                name: "ActorFullName",
                table: "WorkflowActors");

            migrationBuilder.RenameColumn(
                name: "ActorUsername",
                table: "WorkflowActors",
                newName: "ActorName");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "WorkflowActors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
