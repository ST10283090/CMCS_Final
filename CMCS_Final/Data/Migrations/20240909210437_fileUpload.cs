using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_Final.Data.Migrations
{
    /// <inheritdoc />
    public partial class fileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentFileName",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentFilePath",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentFileName",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "DocumentFilePath",
                table: "Claims");
        }
    }
}
