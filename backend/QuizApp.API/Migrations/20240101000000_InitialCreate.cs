using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuizApp.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "User"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "General"),
                    TimeLimitSeconds = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Options_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttemptId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Options_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAnswers_QuizAttempts_AttemptId",
                        column: x => x.AttemptId,
                        principalTable: "QuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Indexes
            migrationBuilder.CreateIndex(name: "IX_Users_Email", table: "Users", column: "Email", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Users_Username", table: "Users", column: "Username", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Questions_QuizId", table: "Questions", column: "QuizId");
            migrationBuilder.CreateIndex(name: "IX_Options_QuestionId", table: "Options", column: "QuestionId");
            migrationBuilder.CreateIndex(name: "IX_QuizAttempts_QuizId", table: "QuizAttempts", column: "QuizId");
            migrationBuilder.CreateIndex(name: "IX_QuizAttempts_UserId", table: "QuizAttempts", column: "UserId");
            migrationBuilder.CreateIndex(name: "IX_UserAnswers_AttemptId", table: "UserAnswers", column: "AttemptId");
            migrationBuilder.CreateIndex(name: "IX_UserAnswers_QuestionId", table: "UserAnswers", column: "QuestionId");
            migrationBuilder.CreateIndex(name: "IX_UserAnswers_SelectedOptionId", table: "UserAnswers", column: "SelectedOptionId");

            // Seed data
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Username", "Email", "PasswordHash", "Role", "CreatedAt" },
                values: new object[] { 1, "admin", "admin@quizapp.com", BCrypt.Net.BCrypt.HashPassword("Admin@123"), "Admin", new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "Title", "Description", "Category", "TimeLimitSeconds", "IsActive", "CreatedBy", "CreatedAt" },
                values: new object[] { 1, "General Knowledge", "Test your general knowledge with these 5 questions!", "General", 120, true, 1, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "QuizId", "Text", "Order", "Points" },
                values: new object[,]
                {
                    { 1, 1, "What is the capital of France?", 1, 1 },
                    { 2, 1, "Which planet is known as the Red Planet?", 2, 1 },
                    { 3, 1, "How many sides does a hexagon have?", 3, 1 },
                    { 4, 1, "Who painted the Mona Lisa?", 4, 1 },
                    { 5, 1, "What is the chemical symbol for water?", 5, 1 },
                    { 6, 1, "What is the capital of India?", 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "Options",
                columns: new[] { "Id", "QuestionId", "Text", "IsCorrect" },
                values: new object[,]
                {
                    { 1, 1, "London", false }, { 2, 1, "Paris", true }, { 3, 1, "Berlin", false }, { 4, 1, "Rome", false },
                    { 5, 2, "Venus", false }, { 6, 2, "Jupiter", false }, { 7, 2, "Mars", true }, { 8, 2, "Saturn", false },
                    { 9, 3, "5", false }, { 10, 3, "6", true }, { 11, 3, "7", false }, { 12, 3, "8", false },
                    { 13, 4, "Van Gogh", false }, { 14, 4, "Picasso", false }, { 15, 4, "Da Vinci", true }, { 16, 4, "Rembrandt", false },
                    { 17, 5, "O2", false }, { 18, 5, "CO2", false }, { 19, 5, "H2O", true }, { 20, 5, "NaCl", false },
                    { 21, 6, "Mumbai", false }, { 22, 6, "New Delhi", true }, { 23, 6, "Kolkata", false }, { 24, 6, "Chennai", false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserAnswers");
            migrationBuilder.DropTable(name: "Options");
            migrationBuilder.DropTable(name: "QuizAttempts");
            migrationBuilder.DropTable(name: "Questions");
            migrationBuilder.DropTable(name: "Quizzes");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
