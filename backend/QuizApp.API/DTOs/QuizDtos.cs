namespace QuizApp.API.DTOs;

// ─── Auth ───────────────────────────────────────────────────────────────────
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int UserId { get; set; }
}

// ─── Quiz ────────────────────────────────────────────────────────────────────
public class QuizListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TimeLimitSeconds { get; set; }
    public int QuestionCount { get; set; }
    public bool IsActive { get; set; }
}

public class QuizDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int TimeLimitSeconds { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public int Points { get; set; }
    public List<OptionDto> Options { get; set; } = new();
}

public class OptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

// ─── Attempt ─────────────────────────────────────────────────────────────────
public class StartAttemptResponseDto
{
    public int AttemptId { get; set; }
    public DateTime StartedAt { get; set; }
}

public class SubmitAnswerDto
{
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }
}

public class SubmitAttemptDto
{
    public List<SubmitAnswerDto> Answers { get; set; } = new();
}

public class AttemptResultDto
{
    public int AttemptId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public double Percentage { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public List<QuestionResultDto> QuestionResults { get; set; } = new();
}

public class QuestionResultDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int? SelectedOptionId { get; set; }
    public int CorrectOptionId { get; set; }
    public bool IsCorrect { get; set; }
    public List<OptionResultDto> Options { get; set; } = new();
}

public class OptionResultDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

// ─── Leaderboard ─────────────────────────────────────────────────────────────
public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string Username { get; set; } = string.Empty;
    public string QuizTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public DateTime CompletedAt { get; set; }
}

// ─── Admin ───────────────────────────────────────────────────────────────────
public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int TimeLimitSeconds { get; set; } = 600;
}

public class CreateQuestionDto
{
    public int QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public int Points { get; set; } = 1;
    public List<CreateOptionDto> Options { get; set; } = new();
}

public class CreateOptionDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class AdminAttemptDto
{
    public int AttemptId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string QuizTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
}
