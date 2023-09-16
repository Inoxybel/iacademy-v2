using Domain.DTO.Summary;
using Domain.DTO;
using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/summary")]
public class SummaryController : ControllerBase
{
    private readonly ISummaryService _summaryService;
    private readonly IContentService _contentService;

    public SummaryController(
        ISummaryService summaryService,
        IContentService contentService)
    {
        _summaryService = summaryService;
        _contentService = contentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResult<Summary>>> Get(string id, CancellationToken cancellationToken)
    {
        var result = await _summaryService.Get(id, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllByCategory(string category, bool isAvailable = true, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllByCategory(category, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("category/{category}/subcategory/{subcategory}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllByCategoryAndSubcategory(string category, string subcategory, bool isAvailable = true, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllByCategoryAndSubcategory(category, subcategory, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllByOwnerId(string ownerId, bool isAvailable = true, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllByOwnerId(ownerId, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("subcategory/{subcategory}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllBySubcategory(string subcategory, bool isAvailable = true, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllBySubcategory(subcategory, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> Save([FromBody] SummaryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _summaryService.Save(request, string.Empty, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    [HttpPost("enroll")]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> EnrollUser([FromBody] SummaryMatriculationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newContentResult = await _contentService.CopyContentsToEnrollUser(request, cancellationToken);

        if (!newContentResult.Success)
            return BadRequest(newContentResult.ErrorMessage);

        return Created(string.Empty, newContentResult.Data);
    }

    [HttpPut("{summaryId}")]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> Update(string summaryId, [FromBody] SummaryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _summaryService.Update(summaryId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }
}
