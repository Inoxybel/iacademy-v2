using Domain.DTO;
using Domain.DTO.Content;
using Domain.DTO.Correction;
using Domain.DTO.Summary;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;


namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly ISummaryService _summaryService;
    private readonly IContentService _contentService;
    private readonly IExerciseService _exerciseService;
    private readonly ICorrectionService _correctionService;

    public AIController(
        ISummaryService summaryService,
        IContentService contentService,
        IExerciseService exerciseService,
        ICorrectionService correctionService)
    {
        _summaryService = summaryService;
        _contentService = contentService;
        _exerciseService = exerciseService;
        _correctionService = correctionService;
    }


    [HttpPost("summary/create")]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> MakeSummary([FromBody] SummaryCreationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _summaryService.RequestCreationToAI(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    [HttpPost("summary/{summaryId}/create-content-by-subtopic")]
    public async Task<IActionResult> RequestContentCreationToAI([FromRoute] string summaryId, [FromBody] AIContentCreationRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _contentService.MakeContent(summaryId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    [HttpPost("content/{contentId}/new-content")]
    public async Task<IActionResult> MakeNewAlternativeContent(string contentId, CancellationToken cancellationToken = default)
    {
        var result = await _contentService.MakeAlternativeContent(contentId, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    [HttpPost("content/{contentId}/request-exercise-creation")]
    public async Task<IActionResult> RequestExerciseCreation([FromRoute] string contentId, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.MakeExercise(contentId, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    [HttpPost("exercise/{exerciseId}/request-correction")]
    public async Task<IActionResult> MakeCorrection([FromRoute] string exerciseId, [FromBody] CreateCorrectionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _correctionService.MakeCorrection(exerciseId, request);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }
}
