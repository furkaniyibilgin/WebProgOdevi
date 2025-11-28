using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProgOdev.Migrations
{
    /// <inheritdoc />
    public partial class TrainerBilgisiEklemesi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndHour",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartHour",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "Trainers");
        }
    }
}
