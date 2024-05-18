using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations
{
    /// <inheritdoc />
    public partial class da : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Products_productid",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "totallike",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "productid",
                table: "Likes",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "Userid",
                table: "Likes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "like1",
                table: "Likes",
                newName: "IsLiked");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_productid",
                table: "Likes",
                newName: "IX_Likes_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Products_ProductId",
                table: "Likes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Products_ProductId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Likes",
                newName: "Userid");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Likes",
                newName: "productid");

            migrationBuilder.RenameColumn(
                name: "IsLiked",
                table: "Likes",
                newName: "like1");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_ProductId",
                table: "Likes",
                newName: "IX_Likes_productid");

            migrationBuilder.AddColumn<int>(
                name: "totallike",
                table: "Likes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Products_productid",
                table: "Likes",
                column: "productid",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
