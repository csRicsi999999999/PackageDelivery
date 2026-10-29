using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDelivery.Migrations
{
    /// <inheritdoc />
    public partial class packagedelivery2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandlingInstructions",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "MaxTemperature",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "MinTemperature",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Packages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HandlingInstructions",
                table: "Packages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxTemperature",
                table: "Packages",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinTemperature",
                table: "Packages",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Packages",
                type: "INTEGER",
                nullable: true);
        }
    }
}
