namespace QuizApp.API.Models;

public class Question
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public int Points { get; set; } = 1;

    public Quiz Quiz { get; set; } = null!;
    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
