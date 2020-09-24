using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "consumer_groups",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    uri = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consumer_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "message_templates",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    subject = table.Column<string>(nullable: true),
                    body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "search_filters_editor",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    filter_json = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_search_filters_editor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "welcome_messages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    content = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_welcome_messages", x => x.id);
                });
            migrationBuilder.Sql(@"INSERT INTO welcome_messages(type, content, created_at) VALUES (0, '<h1>Welcome to the Editor</h1>', '2020-05-07 13:37:00.000000');");
            migrationBuilder.Sql(@"INSERT INTO welcome_messages(type, content, created_at) VALUES (1, '<h1>Welcome to the Data Marketplace', '2020-05-07 13:37:00.000000');");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    email_address = table.Column<string>(nullable: true),
                    last_login_data_marketplace = table.Column<DateTime>(nullable: true),
                    last_login_editor = table.Column<DateTime>(nullable: true),
                    last_time_checked = table.Column<DateTime>(nullable: true),
                    default_consumer_group_id = table.Column<int>(nullable: true),
                    search_filter_editor_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_consumer_groups_default_consumer_group_id",
                        column: x => x.default_consumer_group_id,
                        principalTable: "consumer_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_users_search_filters_editor_search_filter_editor_id",
                        column: x => x.search_filter_editor_id,
                        principalTable: "search_filters_editor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "colid_entry_subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    colid_entry = table.Column<string>(nullable: true),
                    notification_interval = table.Column<int>(nullable: false),
                    next_execution_at = table.Column<DateTime>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_colid_entry_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_colid_entry_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    subject = table.Column<string>(nullable: true),
                    body = table.Column<string>(nullable: true),
                    has_been_read_at = table.Column<DateTime>(nullable: true),
                    has_been_sent_at = table.Column<DateTime>(nullable: true),
                    to_be_deleted_at = table.Column<DateTime>(nullable: true),
                    message_template_id = table.Column<int>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_message_templates_message_template_id",
                        column: x => x.message_template_id,
                        principalTable: "message_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "search_filter_data_marketplace",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    filter_json = table.Column<string>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_search_filter_data_marketplace", x => x.id);
                    table.ForeignKey(
                        name: "fk_search_filter_data_marketplace_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stored_queries",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    query_name = table.Column<string>(nullable: true),
                    query_json = table.Column<string>(nullable: true),
                    execution_interval = table.Column<int>(nullable: false),
                    last_execution_result = table.Column<string>(nullable: true),
                    next_execution_at = table.Column<DateTime>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stored_queries", x => x.id);
                    table.ForeignKey(
                        name: "fk_stored_queries_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_message_configs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    messages_type = table.Column<int>(nullable: false),
                    delete_sent_messages_after = table.Column<int>(nullable: false),
                    delete_read_messages_after = table.Column<int>(nullable: false),
                    message_template_id = table.Column<int>(nullable: true),
                    user_id = table.Column<Guid>(nullable: true)
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
                name: "ix_colid_entry_subscriptions_user_id",
                table: "colid_entry_subscriptions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_message_template_id",
                table: "messages",
                column: "message_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_user_id",
                table: "messages",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_search_filter_data_marketplace_user_id",
                table: "search_filter_data_marketplace",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_stored_queries_user_id",
                table: "stored_queries",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_message_configs_message_template_id",
                table: "user_message_configs",
                column: "message_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_message_configs_user_id",
                table: "user_message_configs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_default_consumer_group_id",
                table: "users",
                column: "default_consumer_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_search_filter_editor_id",
                table: "users",
                column: "search_filter_editor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "colid_entry_subscriptions");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "search_filter_data_marketplace");

            migrationBuilder.DropTable(
                name: "stored_queries");

            migrationBuilder.DropTable(
                name: "user_message_configs");

            migrationBuilder.DropTable(
                name: "welcome_messages");

            migrationBuilder.DropTable(
                name: "message_templates");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "consumer_groups");

            migrationBuilder.DropTable(
                name: "search_filters_editor");
        }
    }
}
