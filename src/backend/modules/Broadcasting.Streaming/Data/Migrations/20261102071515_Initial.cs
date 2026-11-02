using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Broadcasting.Streaming.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "streaming");

            migrationBuilder.CreateTable(
                name: "Streamers",
                schema: "streaming",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ChannelSlug = table.Column<string>(type: "text", nullable: false),
                    StreamSecretKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streamers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_ChannelSlug",
                schema: "streaming",
                table: "Streamers",
                column: "ChannelSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streamers_UserId",
                schema: "streaming",
                table: "Streamers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Streamers",
                schema: "streaming");
        }
    }
}
