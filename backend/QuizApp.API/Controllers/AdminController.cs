using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.API.DTOs;
using QuizApp.API.Services;

namespace QuizApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly QuizService _quizService;

    public AdminController(QuizService quizService) => _quizService = quizService;

    private int GetAdminId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ─── Quiz CRUD ──────────────────────────────────────────────────────────

    /// <summary>Get all quizzes (including inactive)</summary>
    [HttpGet("quizzes")]
    public async Task<IActionResult> GetAllQuizzes() =>
        Ok(await _quizService.GetAllQuizzesAdminAsync());

    /// <summary>Create a new quiz</summary>
    [HttpPost("quizzes")]
    public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto dto)
    {
        var quiz = await _quizService.CreateQuizAsync(dto, GetAdminId());
        return CreatedAtAction(nameof(GetAllQuizzes), new { id = quiz.Id }, quiz);
    }

    /// <summary>Update an existing quiz</summary>
    [HttpPut("quizzes/{id}")]
    public async Task<IActionResult> UpdateQuiz(int id, [FromBody] CreateQuizDto dto)
    {
        var ok = await _quizService.UpdateQuizAsync(id, dto);
        return ok ? NoContent() : NotFound(new { message = "Quiz not found." });
    }

    /// <summary>Toggle quiz active/inactive</summary>
    [HttpPatch("quizzes/{id}/toggle")]
    public async Task<IActionResult> ToggleQuiz(int id)
    {
        var ok = await _quizService.ToggleQuizActiveAsync(id);
        return ok ? Ok(new { message = "Quiz status toggled." }) : NotFound(new { message = "Quiz not found." });
    }

    /// <summary>Delete a quiz</summary>
    [HttpDelete("quizzes/{id}")]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        var ok = await _quizService.DeleteQuizAsync(id);
        return ok ? NoContent() : NotFound(new { message = "Quiz not found." });
    }

    // ─── Question CRUD ──────────────────────────────────────────────────────

    /// <summary>Add a question (with options) to a quiz</summary>
    [HttpPost("questions")]
    public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDto dto)
    {
        var question = await _quizService.CreateQuestionAsync(dto);
        return Ok(question);
    }

    /// <summary>Delete a question</summary>
    [HttpDelete("questions/{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var ok = await _quizService.DeleteQuestionAsync(id);
        return ok ? NoContent() : NotFound(new { message = "Question not found." });
    }

    // ─── Attempts ───────────────────────────────────────────────────────────

    /// <summary>Get all user attempts</summary>
    [HttpGet("attempts")]
    public async Task<IActionResult> GetAllAttempts() =>
        Ok(await _quizService.GetAllAttemptsAsync());
}
