using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class ApplicationVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application_versions",
                columns: table => new
                {
                    application = table.Column<string>(nullable: false),
                    release_date = table.Column<DateTime>(nullable: false),
                    pipeline_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_versions", x => new { x.application, x.release_date });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_versions");
        }
    }
}
