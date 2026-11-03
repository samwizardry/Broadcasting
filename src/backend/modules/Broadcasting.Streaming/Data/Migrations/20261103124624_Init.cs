using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Broadcasting.Streaming.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "streaming");

            migrationBuilder.CreateTable(
                name: "StreamChannels",
                schema: "streaming",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    ChannelSlug = table.Column<string>(type: "text", nullable: false),
                    StreamSecretKey = table.Column<string>(type: "text", nullable: true),
                    IsLive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamChannels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreamChannels_ChannelSlug",
                schema: "streaming",
                table: "StreamChannels",
                column: "ChannelSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StreamChannels_Username",
                schema: "streaming",
                table: "StreamChannels",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamChannels",
                schema: "streaming");
        }
    }
}
