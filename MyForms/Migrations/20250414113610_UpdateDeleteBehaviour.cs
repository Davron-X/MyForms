using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyForms.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Templates_TemplateId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Templates_TemplateId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "TemplateUserAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "Forms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilledBy",
                table: "Forms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses",
                columns: new[] { "TemplateId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Templates_TemplateId",
                table: "Comments",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms",
                column: "FilledBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Templates_TemplateId",
                table: "Likes",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Templates_TemplateId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_AspNetUsers_FilledBy",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Templates_TemplateId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Templates_TemplateId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "TemplateUserAccesses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TemplateId",
                table: "Forms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "FilledBy",
                table: "Forms",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses",
                columns: new[] { "TemplateId", "UserId" },
                unique: true,
                filter: "[TemplateId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Templates_TemplateId",
                table: "Comments",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Templates_TemplateId",
                table: "Likes",
                column: "TemplateId",
                principalTable: "Templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Templates_AspNetUsers_CreatedById",
                table: "Templates",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateUserAccesses_AspNetUsers_UserId",
                table: "TemplateUserAccesses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
