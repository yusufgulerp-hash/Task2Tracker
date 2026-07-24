using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task2Tracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskBlockerNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockerNote",
                table: "Tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockerNote",
                table: "Tasks");
        }
    }
}
