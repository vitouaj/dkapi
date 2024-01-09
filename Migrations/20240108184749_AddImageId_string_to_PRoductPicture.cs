using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dkapi.Migrations
{
    /// <inheritdoc />
    public partial class AddImageId_string_to_PRoductPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BinaryData",
                table: "ProductPictures");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "ProductPictures",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "ProductPictures");

            migrationBuilder.AddColumn<string>(
                name: "BinaryData",
                table: "ProductPictures",
                type: "text",
                nullable: true);
        }
    }
}
