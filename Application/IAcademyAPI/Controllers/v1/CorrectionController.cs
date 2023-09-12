using Domain.DTO.Correction;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/correction")]
public class CorrectionController : ControllerBase
{
    private readonly ICorrectionService _service;

    public CorrectionController(ICorrectionService service)
    {
        _service = service;
    }

    [HttpGet("{correctionId}")]
    public async Task<IActionResult> Get([FromRoute] string correctionId)
    {
        var result = await _service.Get(correctionId);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPut("{correctionId}")]
    public async Task<IActionResult> Update([FromRoute] string correctionId, [FromBody] CorrectionUpdateRequest correction)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.Update(correctionId, correction);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }
}
