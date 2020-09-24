using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class RefactoredDataModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_message_templates_message_template_id",
                table: "messages");

            migrationBuilder.DropTable(
                name: "user_message_configs");

            migrationBuilder.DropIndex(
                name: "ix_messages_message_template_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "has_been_read_at",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "has_been_sent_at",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "message_template_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "to_be_deleted_at",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "type",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "colid_entry",
                table: "colid_entry_subscriptions");

            migrationBuilder.DropColumn(
                name: "next_execution_at",
                table: "colid_entry_subscriptions");

            migrationBuilder.DropColumn(
                name: "notification_interval",
                table: "colid_entry_subscriptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "delete_on",
                table: "messages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "read_on",
                table: "messages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "send_on",
                table: "messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "colid_pid_uri",
                table: "colid_entry_subscriptions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "colid_entry_subscriptions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "message_configs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    send_interval = table.Column<int>(nullable: false),
                    delete_interval = table.Column<int>(nullable: false),
                    user_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_configs", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_configs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_welcome_messages_type",
                table: "welcome_messages",
                column: "type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_id",
                table: "users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_message_templates_type",
                table: "message_templates",
                column: "type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_message_configs_user_id",
                table: "message_configs",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message_configs");

            migrationBuilder.DropIndex(
                name: "ix_welcome_messages_type",
                table: "welcome_messages");

            migrationBuilder.DropIndex(
                name: "ix_users_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_message_templates_type",
                table: "message_templates");

            migrationBuilder.DropColumn(
                name: "delete_on",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "read_on",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "send_on",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "colid_pid_uri",
                table: "colid_entry_subscriptions");

            migrationBuilder.DropColumn(
                name: "note",
                table: "colid_entry_subscriptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "has_been_read_at",
                table: "messages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "has_been_sent_at",
                table: "messages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "message_template_id",
                table: "messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "to_be_deleted_at",
                table: "messages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "colid_entry",
                table: "colid_entry_subscriptions",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "next_execution_at",
                table: "colid_entry_subscriptions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "notification_interval",
                table: "colid_entry_subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "user_message_configs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    delete_read_messages_after = table.Column<int>(type: "int", nullable: false),
                    delete_sent_messages_after = table.Column<int>(type: "int", nullable: false),
                    message_template_id = table.Column<int>(type: "int", nullable: true),
                    messages_type = table.Column<int>(type: "int", nullable: false),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_message_configs", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_message_configs_message_templates_message_template_id",
                        column: x => x.message_template_id,
                        principalTable: "message_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_message_configs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_messages_message_template_id",
                table: "messages",
                column: "message_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_message_configs_message_template_id",
                table: "user_message_configs",
                column: "message_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_message_configs_user_id",
                table: "user_message_configs",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_message_templates_message_template_id",
                table: "messages",
                column: "message_template_id",
                principalTable: "message_templates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
