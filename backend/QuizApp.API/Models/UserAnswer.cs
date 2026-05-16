namespace QuizApp.API.Models;

public class UserAnswer
{
    public int Id { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }

    public QuizAttempt Attempt { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public Option? SelectedOption { get; set; }
}
