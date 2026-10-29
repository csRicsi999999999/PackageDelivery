using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDelivery.Migrations
{
    /// <inheritdoc />
    public partial class packagedelivery4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Packages",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FragileLevel",
                table: "Packages",
                type: "INTEGER",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "FragileLevel",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "HandlingInstructions",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "MaxTemperature",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "MinTemperature",
                table: "Packages");
        }
    }
}
