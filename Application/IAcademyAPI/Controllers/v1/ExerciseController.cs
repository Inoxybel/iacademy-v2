using CrossCutting.Enums;
using Domain.DTO.Exercise;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/exercise")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet("{exerciseId}")]
    public async Task<IActionResult> Get([FromRoute] string exerciseId, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.Get(exerciseId, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("owner/{ownerId}/type/{type}")]
    public async Task<IActionResult> GetAllByOwnerIdAndType([FromRoute] string ownerId, [FromRoute] ExerciseType type, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.GetAllByOwnerIdAndType(ownerId, type, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] ExerciseRequest request, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.Save(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost("request-exercise-creation/content/{contentId}")]
    public async Task<IActionResult> RequestExerciseCreation([FromRoute] string contentId, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.MakeExercise(contentId, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPut("{exerciseId}")]
    public async Task<IActionResult> Update([FromRoute] string exerciseId, [FromBody] ExerciseRequest request, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.Update(exerciseId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }
}
