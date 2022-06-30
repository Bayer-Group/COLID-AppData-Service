using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class UpdatedDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_colid_entry_subscriptions_users_user_id",
                table: "colid_entry_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_users_search_filters_editor_search_filter_editor_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_colid_entry_subscriptions_users_user_id",
                table: "colid_entry_subscriptions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_search_filters_editor_search_filter_editor_id",
                table: "users",
                column: "search_filter_editor_id",
                principalTable: "search_filters_editor",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_colid_entry_subscriptions_users_user_id",
                table: "colid_entry_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_users_search_filters_editor_search_filter_editor_id",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_colid_entry_subscriptions_users_user_id",
                table: "colid_entry_subscriptions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_users_search_filters_editor_search_filter_editor_id",
                table: "users",
                column: "search_filter_editor_id",
                principalTable: "search_filters_editor",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
