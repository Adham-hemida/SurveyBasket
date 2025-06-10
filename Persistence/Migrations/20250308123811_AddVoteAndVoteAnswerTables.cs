using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Persistence.Migrations
{
	/// <inheritdoc />
	public partial class AddVoteAndVoteAnswerTables : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Votes",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
					PollId = table.Column<int>(type: "int", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Votes", x => x.Id);
					table.ForeignKey(
						name: "FK_Votes_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Votes_Polls_PollId",
						column: x => x.PollId,
						principalTable: "Polls",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "VoteAnswers",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					AnswerId = table.Column<int>(type: "int", nullable: false),
					VoteId = table.Column<int>(type: "int", nullable: false),
					QuestionId = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_VoteAnswers", x => x.Id);
					table.ForeignKey(
						name: "FK_VoteAnswers_Answers_AnswerId",
						column: x => x.AnswerId,
						principalTable: "Answers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_VoteAnswers_Questions_QuestionId",
						column: x => x.QuestionId,
						principalTable: "Questions",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_VoteAnswers_Votes_VoteId",
						column: x => x.VoteId,
						principalTable: "Votes",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_VoteAnswers_AnswerId",
				table: "VoteAnswers",
				column: "AnswerId");

			migrationBuilder.CreateIndex(
				name: "IX_VoteAnswers_QuestionId_VoteId",
				table: "VoteAnswers",
				columns: new[] { "QuestionId", "VoteId" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_VoteAnswers_VoteId",
				table: "VoteAnswers",
				column: "VoteId");

			migrationBuilder.CreateIndex(
				name: "IX_Votes_PollId_UserId",
				table: "Votes",
				columns: new[] { "PollId", "UserId" },
				unique: true);

			migrationBuilder.CreateIndex(
				name: "IX_Votes_UserId",
				table: "Votes",
				column: "UserId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "VoteAnswers");

			migrationBuilder.DropTable(
				name: "Votes");
		}
	}
}
