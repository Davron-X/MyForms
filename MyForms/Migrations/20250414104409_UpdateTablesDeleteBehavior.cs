using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyForms.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateUserAccesses_Templates_TemplateId",
                table: "TemplateUserAccesses");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateUserAccesses_Templates_TemplateId",
                table: "TemplateUserAccesses",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateUserAccesses_Templates_TemplateId",
                table: "TemplateUserAccesses");

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateUserAccesses_Templates_TemplateId",
                table: "TemplateUserAccesses",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
