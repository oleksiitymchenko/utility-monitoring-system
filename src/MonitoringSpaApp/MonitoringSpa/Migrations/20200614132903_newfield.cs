using Microsoft.EntityFrameworkCore.Migrations;

namespace MonitoringSpa.Migrations
{
    public partial class newfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlobName",
                table: "TelemetryRecord",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ControllerRegistry",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobName",
                table: "TelemetryRecord");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ControllerRegistry",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
