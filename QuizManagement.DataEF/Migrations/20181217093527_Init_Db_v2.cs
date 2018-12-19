using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizManagement.DataEF.Migrations
{
    public partial class Init_Db_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_SubjectChapterDetails_SubjectChapterDetailId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "SubjectChapterDetailId",
                table: "Questions",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_SubjectChapterDetailId",
                table: "Questions",
                newName: "IX_Questions_SubjectId");

            migrationBuilder.AddColumn<int>(
                name: "ChapterId",
                table: "Questions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ChapterId",
                table: "Questions",
                column: "ChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Chapters_ChapterId",
                table: "Questions",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Subjects_SubjectId",
                table: "Questions",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Chapters_ChapterId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Subjects_SubjectId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ChapterId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "Questions",
                newName: "SubjectChapterDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_SubjectId",
                table: "Questions",
                newName: "IX_Questions_SubjectChapterDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_SubjectChapterDetails_SubjectChapterDetailId",
                table: "Questions",
                column: "SubjectChapterDetailId",
                principalTable: "SubjectChapterDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
