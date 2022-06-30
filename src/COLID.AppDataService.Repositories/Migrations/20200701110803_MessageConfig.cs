using Microsoft.EntityFrameworkCore.Migrations;

namespace Repositories.Migrations
{
    public partial class MessageConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO message_configs
                SELECT null, now(), now(), 3, 4, id
                FROM users us
                WHERE NOT EXISTS
	                (
                    SELECT null
                    FROM message_configs mc
                    WHERE mc.user_id = us.id
                    );"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
