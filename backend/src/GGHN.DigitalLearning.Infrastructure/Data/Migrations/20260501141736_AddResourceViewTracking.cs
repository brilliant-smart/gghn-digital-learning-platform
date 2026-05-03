using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGHN.DigitalLearning.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceViewTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResourceViews_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceViews_ResourceId",
                table: "ResourceViews",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceViews_UserId",
                table: "ResourceViews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceViews_ViewedAt",
                table: "ResourceViews",
                column: "ViewedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceViews");
        }
    }
}
