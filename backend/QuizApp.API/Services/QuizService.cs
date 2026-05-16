using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using QuizApp.API.DTOs;
using QuizApp.API.Models;

namespace QuizApp.API.Services;

public class QuizService
{
    private readonly QuizDbContext _db;

    public QuizService(QuizDbContext db) => _db = db;

    // ─── User-facing ──────────────────────────────────────────────────────────

    public async Task<List<QuizListDto>> GetActiveQuizzesAsync()
    {
        return await _db.Quizzes
            .Where(q => q.IsActive)
            .Select(q => new QuizListDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                Category = q.Category,
                TimeLimitSeconds = q.TimeLimitSeconds,
                QuestionCount = q.Questions.Count,
                IsActive = q.IsActive
            })
            .ToListAsync();
    }

    public async Task<QuizDetailDto?> GetQuizDetailAsync(int quizId)
    {
        var quiz = await _db.Quizzes
            .Include(q => q.Questions.OrderBy(qu => qu.Order))
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.IsActive);

        if (quiz == null) return null;

        return new QuizDetailDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            Category = quiz.Category,
            TimeLimitSeconds = quiz.TimeLimitSeconds,
            Questions = quiz.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Order = q.Order,
                Points = q.Points,
                Options = q.Options.Select(o => new OptionDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            }).ToList()
        };
    }

    public async Task<StartAttemptResponseDto> StartAttemptAsync(int quizId, int userId)
    {
        var quiz = await _db.Quizzes
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(q => q.Id == quizId && q.IsActive)
            ?? throw new Exception("Quiz not found or inactive.");

        var attempt = new QuizAttempt
        {
            UserId = userId,
            QuizId = quizId,
            TotalQuestions = quiz.Questions.Count,
            StartedAt = DateTime.UtcNow
        };

        _db.QuizAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        return new StartAttemptResponseDto
        {
            AttemptId = attempt.Id,
            StartedAt = attempt.StartedAt
        };
    }

    public async Task<AttemptResultDto> SubmitAttemptAsync(int attemptId, int userId, SubmitAttemptDto dto)
    {
        var attempt = await _db.QuizAttempts
            .Include(a => a.Quiz)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId && !a.IsCompleted)
            ?? throw new Exception("Attempt not found or already completed.");

        var questions = await _db.Questions
            .Include(q => q.Options)
            .Where(q => q.QuizId == attempt.QuizId)
            .ToListAsync();

        var questionResults = new List<QuestionResultDto>();
        int correctCount = 0;
        int totalScore = 0;

        foreach (var q in questions)
        {
            var submitted = dto.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
            var correctOption = q.Options.First(o => o.IsCorrect);
            bool isCorrect = submitted?.SelectedOptionId == correctOption.Id;

            if (isCorrect)
            {
                correctCount++;
                totalScore += q.Points;
            }

            _db.UserAnswers.Add(new UserAnswer
            {
                AttemptId = attempt.Id,
                QuestionId = q.Id,
                SelectedOptionId = submitted?.SelectedOptionId
            });

            questionResults.Add(new QuestionResultDto
            {
                QuestionId = q.Id,
                QuestionText = q.Text,
                SelectedOptionId = submitted?.SelectedOptionId,
                CorrectOptionId = correctOption.Id,
                IsCorrect = isCorrect,
                Options = q.Options.Select(o => new OptionResultDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            });
        }

        attempt.Score = totalScore;
        attempt.CorrectAnswers = correctCount;
        attempt.CompletedAt = DateTime.UtcNow;
        attempt.IsCompleted = true;

        await _db.SaveChangesAsync();

        var timeTaken = attempt.CompletedAt.Value - attempt.StartedAt;

        return new AttemptResultDto
        {
            AttemptId = attempt.Id,
            QuizTitle = attempt.Quiz.Title,
            Score = totalScore,
            TotalQuestions = attempt.TotalQuestions,
            CorrectAnswers = correctCount,
            Percentage = attempt.TotalQuestions > 0
                ? Math.Round((double)correctCount / attempt.TotalQuestions * 100, 1)
                : 0,
            TimeTaken = timeTaken,
            QuestionResults = questionResults
        };
    }

    // ─── Leaderboard ──────────────────────────────────────────────────────────

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int? quizId = null)
    {
        var query = _db.QuizAttempts
            .Include(a => a.User)
            .Include(a => a.Quiz)
            .Where(a => a.IsCompleted);

        if (quizId.HasValue)
            query = query.Where(a => a.QuizId == quizId.Value);

        var attempts = await query
            .OrderByDescending(a => a.Score)
            .ThenBy(a => a.CompletedAt - a.StartedAt)
            .Take(50)
            .ToListAsync();

        return attempts.Select((a, i) => new LeaderboardEntryDto
        {
            Rank = i + 1,
            Username = a.User.Username,
            QuizTitle = a.Quiz.Title,
            Score = a.Score,
            TotalQuestions = a.TotalQuestions,
            Percentage = a.TotalQuestions > 0
                ? Math.Round((double)a.CorrectAnswers / a.TotalQuestions * 100, 1)
                : 0,
            CompletedAt = a.CompletedAt ?? a.StartedAt
        }).ToList();
    }

    // ─── Admin ────────────────────────────────────────────────────────────────

    public async Task<List<QuizListDto>> GetAllQuizzesAdminAsync()
    {
        return await _db.Quizzes
            .Select(q => new QuizListDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description,
                Category = q.Category,
                TimeLimitSeconds = q.TimeLimitSeconds,
                QuestionCount = q.Questions.Count,
                IsActive = q.IsActive
            })
            .ToListAsync();
    }

    public async Task<Quiz> CreateQuizAsync(CreateQuizDto dto, int adminId)
    {
        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            TimeLimitSeconds = dto.TimeLimitSeconds,
            CreatedBy = adminId,
            IsActive = true
        };
        _db.Quizzes.Add(quiz);
        await _db.SaveChangesAsync();
        return quiz;
    }

    public async Task<bool> UpdateQuizAsync(int id, CreateQuizDto dto)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.Category = dto.Category;
        quiz.TimeLimitSeconds = dto.TimeLimitSeconds;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleQuizActiveAsync(int id)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz == null) return false;
        quiz.IsActive = !quiz.IsActive;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteQuizAsync(int id)
    {
        var quiz = await _db.Quizzes.FindAsync(id);
        if (quiz == null) return false;
        _db.Quizzes.Remove(quiz);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Question> CreateQuestionAsync(CreateQuestionDto dto)
    {
        var question = new Question
        {
            QuizId = dto.QuizId,
            Text = dto.Text,
            Order = dto.Order,
            Points = dto.Points
        };

        foreach (var o in dto.Options)
        {
            question.Options.Add(new Option { Text = o.Text, IsCorrect = o.IsCorrect });
        }

        _db.Questions.Add(question);
        await _db.SaveChangesAsync();
        return question;
    }

    public async Task<bool> DeleteQuestionAsync(int id)
    {
        var q = await _db.Questions.FindAsync(id);
        if (q == null) return false;
        _db.Questions.Remove(q);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<AdminAttemptDto>> GetAllAttemptsAsync()
    {
        return await _db.QuizAttempts
            .Include(a => a.User)
            .Include(a => a.Quiz)
            .OrderByDescending(a => a.StartedAt)
            .Select(a => new AdminAttemptDto
            {
                AttemptId = a.Id,
                Username = a.User.Username,
                QuizTitle = a.Quiz.Title,
                Score = a.Score,
                TotalQuestions = a.TotalQuestions,
                Percentage = a.TotalQuestions > 0
                    ? Math.Round((double)a.CorrectAnswers / a.TotalQuestions * 100, 1)
                    : 0,
                StartedAt = a.StartedAt,
                CompletedAt = a.CompletedAt,
                IsCompleted = a.IsCompleted
            })
            .ToListAsync();
    }
}
