using Microsoft.EntityFrameworkCore.Migrations;

namespace AmpRageRepo.Migrations
{
    public partial class ChangedValuesInDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tire_front",
                table: "Data3",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "tire_back",
                table: "Data3",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "tire_front",
                table: "Data3",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "tire_back",
                table: "Data3",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
