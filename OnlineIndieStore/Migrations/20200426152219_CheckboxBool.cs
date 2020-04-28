using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineIndieStore.Migrations
{
    public partial class CheckboxBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Category",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Category");
        }
    }
}
