using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class FavoritesList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favorites_lists",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorites_lists", x => x.id);
                    table.ForeignKey(
                        name: "fk_favorites_lists_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorites_list_entries",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    favorites_list_id = table.Column<int>(nullable: true),
                    piduri = table.Column<string>(nullable: true),
                    personal_note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_favorites_list_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_favorites_list_entries_favorites_lists_favorites_list_id",
                        column: x => x.favorites_list_id,
                        principalTable: "favorites_lists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_favorites_list_entries_favorites_list_id",
                table: "favorites_list_entries",
                column: "favorites_list_id");

            migrationBuilder.CreateIndex(
                name: "ix_favorites_lists_id",
                table: "favorites_lists",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_favorites_lists_user_id",
                table: "favorites_lists",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorites_list_entries");

            migrationBuilder.DropTable(
                name: "favorites_lists");
        }
    }
}
