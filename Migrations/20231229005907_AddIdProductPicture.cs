using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace dkapi.Migrations
{
    /// <inheritdoc />
    public partial class AddIdProductPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPicture",
                table: "ProductPicture");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductPicture",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPicture",
                table: "ProductPicture",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPicture_ProductId",
                table: "ProductPicture",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductPicture",
                table: "ProductPicture");

            migrationBuilder.DropIndex(
                name: "IX_ProductPicture_ProductId",
                table: "ProductPicture");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductPicture");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductPicture",
                table: "ProductPicture",
                column: "ProductId");
        }
    }
}
