using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_Web_Application.Migrations
{
    /// <inheritdoc />
    public partial class AddFromColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "addTask",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                table: "addTask");
        }
    }
}
