using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dkapi.Migrations
{
    /// <inheritdoc />
    public partial class addProfileKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileKey",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileKey",
                table: "AspNetUsers");
        }
    }
}
