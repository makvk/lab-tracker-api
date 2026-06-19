using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Teachers_TeacherId",
                table: "Submissions");

            migrationBuilder.CreateTable(
                name: "LabWorks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabWorks_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_LabWorkId",
                table: "Submissions",
                column: "LabWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_LabWorks_TeacherId",
                table: "LabWorks",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_LabWorks_LabWorkId",
                table: "Submissions",
                column: "LabWorkId",
                principalTable: "LabWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Teachers_TeacherId",
                table: "Submissions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_LabWorks_LabWorkId",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Teachers_TeacherId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "LabWorks");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_LabWorkId",
                table: "Submissions");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Teachers_TeacherId",
                table: "Submissions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
