using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyForms.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesAndCommentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateUserAccesses",
                table: "TemplateUserAccesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TemplateUserAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TemplateTags",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "TemplateQuestions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TemplateQuestions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateUserAccesses",
                table: "TemplateUserAccesses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TemplateComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateComments_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemplateLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateLikes_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses",
                columns: new[] { "TemplateId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateTags_TemplateId_TagId",
                table: "TemplateTags",
                columns: new[] { "TemplateId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateComments_TemplateId",
                table: "TemplateComments",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateComments_UserId",
                table: "TemplateComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateLikes_TemplateId_UserId",
                table: "TemplateLikes",
                columns: new[] { "TemplateId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateLikes_UserId",
                table: "TemplateLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateComments");

            migrationBuilder.DropTable(
                name: "TemplateLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateUserAccesses",
                table: "TemplateUserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_TemplateUserAccesses_TemplateId_UserId",
                table: "TemplateUserAccesses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags");

            migrationBuilder.DropIndex(
                name: "IX_TemplateTags_TemplateId_TagId",
                table: "TemplateTags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TemplateUserAccesses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TemplateTags");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "TemplateQuestions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "TemplateQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateUserAccesses",
                table: "TemplateUserAccesses",
                columns: new[] { "TemplateId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TemplateTags",
                table: "TemplateTags",
                columns: new[] { "TemplateId", "TagId" });
        }
    }
}
