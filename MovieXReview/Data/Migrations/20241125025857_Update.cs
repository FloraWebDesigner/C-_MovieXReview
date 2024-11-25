using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieXReview.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Viewers_ViewerId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "ViewerId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Viewers_ViewerId",
                table: "Reviews",
                column: "ViewerId",
                principalTable: "Viewers",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Viewers_ViewerId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "ViewerId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Viewers_ViewerId",
                table: "Reviews",
                column: "ViewerId",
                principalTable: "Viewers",
                principalColumn: "ViewerId");
        }
    }
}
