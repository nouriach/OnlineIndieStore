using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineIndieStore.Migrations
{
    public partial class CatEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CategoryName",
                table: "Category",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
