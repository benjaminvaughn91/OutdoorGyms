using Microsoft.EntityFrameworkCore.Migrations;

namespace EnvironmentCrime.Migrations
{
    public partial class SecondCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "Employees",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
