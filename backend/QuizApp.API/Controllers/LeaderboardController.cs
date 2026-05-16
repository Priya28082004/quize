using Microsoft.AspNetCore.Mvc;
using QuizApp.API.Services;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly QuizService _quizService;

    public LeaderboardController(QuizService quizService) => _quizService = quizService;

    /// <summary>Get global leaderboard (top 50 scores across all quizzes)</summary>
    [HttpGet]
    public async Task<IActionResult> GetGlobal() =>
        Ok(await _quizService.GetLeaderboardAsync());

    /// <summary>Get leaderboard for a specific quiz</summary>
    [HttpGet("{quizId}")]
    public async Task<IActionResult> GetByQuiz(int quizId) =>
        Ok(await _quizService.GetLeaderboardAsync(quizId));
}
