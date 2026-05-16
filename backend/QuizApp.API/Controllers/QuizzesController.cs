using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.DTOs;
using QuizApp.API.Services;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizzesController : ControllerBase
{
    private readonly QuizService _quizService;

    public QuizzesController(QuizService quizService) => _quizService = quizService;

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get list of all active quizzes</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetQuizzes() =>
        Ok(await _quizService.GetActiveQuizzesAsync());

    /// <summary>Get quiz details with questions (no correct answers exposed)</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuiz(int id)
    {
        var quiz = await _quizService.GetQuizDetailAsync(id);
        if (quiz == null) return NotFound(new { message = "Quiz not found." });
        return Ok(quiz);
    }

    /// <summary>Start a new quiz attempt</summary>
    [HttpPost("{id}/attempt")]
    public async Task<IActionResult> StartAttempt(int id)
    {
        try
        {
            var result = await _quizService.StartAttemptAsync(id, GetUserId());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Submit answers for a quiz attempt</summary>
    [HttpPost("attempts/{attemptId}/submit")]
    public async Task<IActionResult> SubmitAttempt(int attemptId, [FromBody] SubmitAttemptDto dto)
    {
        try
        {
            var result = await _quizService.SubmitAttemptAsync(attemptId, GetUserId(), dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
