using Microsoft.AspNetCore.Mvc;
using FormSubmissionSystem.Application.DTOs;
using FormSubmissionSystem.Application.UseCases;

namespace FormSubmissionSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubmissionsController : ControllerBase
{
    private readonly GetSubmissionsUseCase _getSubmissionsUseCase;
    private readonly GetSubmissionByIdUseCase _getSubmissionByIdUseCase;
    private readonly CreateSubmissionUseCase _createSubmissionUseCase;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(
        GetSubmissionsUseCase getSubmissionsUseCase,
        GetSubmissionByIdUseCase getSubmissionByIdUseCase,
        CreateSubmissionUseCase createSubmissionUseCase,
        ILogger<SubmissionsController> logger)
    {
        _getSubmissionsUseCase = getSubmissionsUseCase;
        _getSubmissionByIdUseCase = getSubmissionByIdUseCase;
        _createSubmissionUseCase = createSubmissionUseCase;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubmissionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SubmissionResponse>>> GetSubmissions([FromQuery] string? search = null)
    {
        var submissions = await _getSubmissionsUseCase.ExecuteAsync(search);
        return Ok(submissions);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubmissionResponse>> GetSubmission(Guid id)
    {
        var submission = await _getSubmissionByIdUseCase.ExecuteAsync(id);

        if (submission == null)
        {
            _logger.LogWarning("Submission with ID {SubmissionId} not found", id);
            return NotFound(new ErrorResponse { Error = "Submission not found" });
        }

        return Ok(submission);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubmissionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SubmissionResponse>> CreateSubmission([FromBody] CreateSubmissionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorResponse { Error = "Invalid request data" });
        }

        var submission = await _createSubmissionUseCase.ExecuteAsync(request);

        return CreatedAtAction(nameof(GetSubmission), new { id = submission.Id }, submission);
    }
}
