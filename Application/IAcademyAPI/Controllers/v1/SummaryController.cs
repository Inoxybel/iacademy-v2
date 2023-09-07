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

    public SummaryController(ISummaryService summaryService)
    {
        _summaryService = summaryService;
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
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllByCategory(string category, bool isAvailable = false, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllByCategory(category, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("category/{category}/subcategory/{subcategory}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllByCategoryAndSubcategory(string category, string subcategory, bool isAvailable = false, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllByCategoryAndSubcategory(category, subcategory, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllByOwnerId(string ownerId, bool isAvailable = false, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllByOwnerId(ownerId, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("subcategory/{subcategory}")]
    public async Task<ActionResult<ServiceResult<List<Summary>>>> GetAllBySubcategory(string subcategory, bool isAvailable = false, CancellationToken cancellationToken = default)
    {
        var result = await _summaryService.GetAllBySubcategory(subcategory, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> Save([FromBody] SummaryRequest request, CancellationToken cancellationToken)
    {
        var result = await _summaryService.Save(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost("/enroll")]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> EnrollUser([FromBody] SummaryMatriculationRequest request, CancellationToken cancellationToken)
    {
        var result = await _summaryService.EnrollUser(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost("/byai")]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> MakeSummary([FromBody] SummaryCreationRequest request, CancellationToken cancellationToken)
    {
        var result = await _summaryService.RequestCreationToAI(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPut("{summaryId}")]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> Update(string summaryId, [FromBody] SummaryRequest request, CancellationToken cancellationToken)
    {
        var result = await _summaryService.Update(summaryId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }


}
