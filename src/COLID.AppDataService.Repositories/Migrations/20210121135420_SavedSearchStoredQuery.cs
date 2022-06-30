using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class SavedSearchStoredQuery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_stored_queries_users_user_id",
                table: "stored_queries");

            migrationBuilder.DropIndex(
                name: "ix_stored_queries_user_id",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "last_execution_result",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "next_execution_at",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "query_json",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "query_name",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "stored_queries");

            migrationBuilder.AddColumn<DateTime>(
                name: "latest_execution_date",
                table: "stored_queries",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_search_results",
                table: "stored_queries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "search_result_hash",
                table: "stored_queries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "search_term",
                table: "search_filter_data_marketplace",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "stored_query_id",
                table: "search_filter_data_marketplace",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_search_filter_data_marketplace_stored_query_id",
                table: "search_filter_data_marketplace",
                column: "stored_query_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_search_filter_data_marketplace_stored_queries_stored_query_id",
                table: "search_filter_data_marketplace",
                column: "stored_query_id",
                principalTable: "stored_queries",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_search_filter_data_marketplace_stored_queries_stored_query_id",
                table: "search_filter_data_marketplace");

            migrationBuilder.DropIndex(
                name: "ix_search_filter_data_marketplace_stored_query_id",
                table: "search_filter_data_marketplace");

            migrationBuilder.DropColumn(
                name: "latest_execution_date",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "number_search_results",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "search_result_hash",
                table: "stored_queries");

            migrationBuilder.DropColumn(
                name: "search_term",
                table: "search_filter_data_marketplace");

            migrationBuilder.DropColumn(
                name: "stored_query_id",
                table: "search_filter_data_marketplace");

            migrationBuilder.AddColumn<string>(
                name: "last_execution_result",
                table: "stored_queries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "next_execution_at",
                table: "stored_queries",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "query_json",
                table: "stored_queries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "query_name",
                table: "stored_queries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "stored_queries",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_stored_queries_user_id",
                table: "stored_queries",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_stored_queries_users_user_id",
                table: "stored_queries",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
