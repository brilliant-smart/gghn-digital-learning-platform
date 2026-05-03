using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGHN.DigitalLearning.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendPublicationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalUrl",
                table: "Publications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyFindings",
                table: "Publications",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicationType",
                table: "Publications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Publications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Publications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalUrl",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "KeyFindings",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "PublicationType",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Publications");
        }
    }
}
