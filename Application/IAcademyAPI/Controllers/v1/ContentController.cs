using Domain.DTO.Content;
using Domain.Entities;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet("{contentId}")]
    public async Task<IActionResult> Get(string contentId, CancellationToken cancellationToken = default)
    {
        var result = await _contentService.Get(contentId, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("summary/{summaryId}")]
    public async Task<IActionResult> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default)
    {
        var result = await _contentService.GetAllBySummaryId(summaryId, cancellationToken);

        if (!result.Success || !result.Data.Any())
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] ContentRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _contentService.Save(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost("save-all")]
    public async Task<IActionResult> SaveAll([FromBody] List<ContentRequest> contents, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _contentService.SaveAll(contents, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPut("{contentId}")]
    public async Task<IActionResult> Update([FromRoute] string contentId, [FromBody] ContentRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _contentService.Update(contentId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPut("update-all/summary/{summaryId}")]
    public async Task<IActionResult> UpdateAll(string summaryId, [FromBody] List<Content> contents, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _contentService.UpdateAll(summaryId, contents, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }
}
