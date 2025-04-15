using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyForms.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFormTableDeleteBehavior3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses",
                columns: new[] { "TemplateId", "UserId" },
                unique: true,
                filter: "[TemplateId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses",
                columns: new[] { "TemplateId", "UserId" },
                unique: true);
        }
    }
}
