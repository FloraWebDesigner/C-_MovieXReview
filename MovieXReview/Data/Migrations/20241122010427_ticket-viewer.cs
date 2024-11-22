using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieXReview.Data.Migrations
{
    /// <inheritdoc />
    public partial class ticketviewer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewerId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ViewerId",
                table: "Tickets",
                column: "ViewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Viewers_ViewerId",
                table: "Tickets",
                column: "ViewerId",
                principalTable: "Viewers",
                principalColumn: "ViewerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Viewers_ViewerId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ViewerId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ViewerId",
                table: "Tickets");
        }
    }
}
