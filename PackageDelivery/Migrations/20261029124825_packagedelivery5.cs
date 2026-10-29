using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDelivery.Migrations
{
    /// <inheritdoc />
    public partial class packagedelivery5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoxType",
                table: "Packages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxType",
                table: "Packages");
        }
    }
}
