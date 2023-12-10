using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddYearMileageCubicCapacityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CubicCapacity",
                table: "Car",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Mileage",
                table: "Car",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Car",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CubicCapacity",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Car");
        }
    }
}
