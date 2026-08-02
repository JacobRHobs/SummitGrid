using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummitGrid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRockTypeToRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RockType",
                table: "Routes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RockType",
                table: "Routes");
        }
    }
}
