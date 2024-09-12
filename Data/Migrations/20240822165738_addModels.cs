using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProjectPrac7.Data.Migrations
{
    /// <inheritdoc />
    public partial class addModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPost_Categories_GetCategoriesId",
                table: "CategoryPost");

            migrationBuilder.RenameColumn(
                name: "GetCategoriesId",
                table: "CategoryPost",
                newName: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPost_Categories_CategoriesId",
                table: "CategoryPost",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPost_Categories_CategoriesId",
                table: "CategoryPost");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryPost",
                newName: "GetCategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPost_Categories_GetCategoriesId",
                table: "CategoryPost",
                column: "GetCategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
