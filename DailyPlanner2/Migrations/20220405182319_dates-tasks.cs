using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyPlanner2.Migrations
{
    public partial class datestasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
