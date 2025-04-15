using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyForms.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviour3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Templates",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Templates",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
