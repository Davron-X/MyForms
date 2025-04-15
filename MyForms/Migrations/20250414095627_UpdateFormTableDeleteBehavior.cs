using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyForms.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFormTableDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "Forms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms",
                column: "FilledBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "Forms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms",
                column: "FilledBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
