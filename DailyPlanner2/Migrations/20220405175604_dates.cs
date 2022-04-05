using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyPlanner2.Migrations
{
    public partial class dates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DateId",
                table: "tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "dates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tasks_DateId",
                table: "tasks",
                column: "DateId");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_dates_DateId",
                table: "tasks",
                column: "DateId",
                principalTable: "dates",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_dates_DateId",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "dates");

            migrationBuilder.DropIndex(
                name: "IX_tasks_DateId",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "DateId",
                table: "tasks");
        }
    }
}
