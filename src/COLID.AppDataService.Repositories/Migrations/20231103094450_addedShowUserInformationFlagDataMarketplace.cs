using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class addedShowUserInformationFlagDataMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_welcome_messages_type",
                table: "welcome_messages",
                newName: "IX_welcome_messages_type");

            migrationBuilder.RenameIndex(
                name: "ix_users_search_filter_editor_id",
                table: "users",
                newName: "IX_users_search_filter_editor_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_id",
                table: "users",
                newName: "IX_users_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_default_consumer_group_id",
                table: "users",
                newName: "IX_users_default_consumer_group_id");

            migrationBuilder.RenameIndex(
                name: "ix_search_filter_data_marketplace_user_id",
                table: "search_filter_data_marketplace",
                newName: "IX_search_filter_data_marketplace_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_search_filter_data_marketplace_stored_query_id",
                table: "search_filter_data_marketplace",
                newName: "IX_search_filter_data_marketplace_stored_query_id");

            migrationBuilder.RenameIndex(
                name: "ix_messages_user_id",
                table: "messages",
                newName: "IX_messages_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_message_templates_type",
                table: "message_templates",
                newName: "IX_message_templates_type");

            migrationBuilder.RenameIndex(
                name: "ix_message_configs_user_id",
                table: "message_configs",
                newName: "IX_message_configs_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_favorites_lists_user_id",
                table: "favorites_lists",
                newName: "IX_favorites_lists_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_favorites_lists_id",
                table: "favorites_lists",
                newName: "IX_favorites_lists_id");

            migrationBuilder.RenameIndex(
                name: "ix_favorites_list_entries_favorites_list_id",
                table: "favorites_list_entries",
                newName: "IX_favorites_list_entries_favorites_list_id");

            migrationBuilder.RenameIndex(
                name: "ix_colid_entry_subscriptions_user_id",
                table: "colid_entry_subscriptions",
                newName: "IX_colid_entry_subscriptions_user_id");

            migrationBuilder.AddColumn<bool>(
                name: "show_user_information_flag_data_marketplace",
                table: "users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "show_user_information_flag_data_marketplace",
                table: "users");

            migrationBuilder.RenameIndex(
                name: "IX_welcome_messages_type",
                table: "welcome_messages",
                newName: "ix_welcome_messages_type");

            migrationBuilder.RenameIndex(
                name: "IX_users_search_filter_editor_id",
                table: "users",
                newName: "ix_users_search_filter_editor_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_id",
                table: "users",
                newName: "ix_users_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_default_consumer_group_id",
                table: "users",
                newName: "ix_users_default_consumer_group_id");

            migrationBuilder.RenameIndex(
                name: "IX_search_filter_data_marketplace_user_id",
                table: "search_filter_data_marketplace",
                newName: "ix_search_filter_data_marketplace_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_search_filter_data_marketplace_stored_query_id",
                table: "search_filter_data_marketplace",
                newName: "ix_search_filter_data_marketplace_stored_query_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_user_id",
                table: "messages",
                newName: "ix_messages_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_templates_type",
                table: "message_templates",
                newName: "ix_message_templates_type");

            migrationBuilder.RenameIndex(
                name: "IX_message_configs_user_id",
                table: "message_configs",
                newName: "ix_message_configs_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_favorites_lists_user_id",
                table: "favorites_lists",
                newName: "ix_favorites_lists_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_favorites_lists_id",
                table: "favorites_lists",
                newName: "ix_favorites_lists_id");

            migrationBuilder.RenameIndex(
                name: "IX_favorites_list_entries_favorites_list_id",
                table: "favorites_list_entries",
                newName: "ix_favorites_list_entries_favorites_list_id");

            migrationBuilder.RenameIndex(
                name: "IX_colid_entry_subscriptions_user_id",
                table: "colid_entry_subscriptions",
                newName: "ix_colid_entry_subscriptions_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "email_address",
                table: "users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "department",
                table: "users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "search_result_hash",
                table: "stored_queries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "filter_json",
                table: "search_filters_editor",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "search_term",
                table: "search_filter_data_marketplace",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "pid_uri",
                table: "search_filter_data_marketplace",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "search_filter_data_marketplace",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "filter_json",
                table: "search_filter_data_marketplace",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "subject",
                table: "messages",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "body",
                table: "messages",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "additional_info",
                table: "messages",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "subject",
                table: "message_templates",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "body",
                table: "message_templates",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "favorites_lists",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "piduri",
                table: "favorites_list_entries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "personal_note",
                table: "favorites_list_entries",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "uri",
                table: "consumer_groups",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "note",
                table: "colid_entry_subscriptions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "colid_pid_uri",
                table: "colid_entry_subscriptions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "pipeline_id",
                table: "application_versions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
