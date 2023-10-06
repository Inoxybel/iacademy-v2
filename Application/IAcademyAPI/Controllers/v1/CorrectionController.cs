using CrossCutting.Helpers;
using Domain.DTO.Correction;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/correction")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class CorrectionController : ControllerBase
{
    private readonly ICorrectionService _service;

    public CorrectionController(ICorrectionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Recupera uma correcao
    /// </summary>
    /// <param name="correctionId">Identificacao da correcao (GUID de 36 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Objeto de correcao</returns>
    [HttpGet("{correctionId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get([FromRoute] string correctionId, CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _service.Get(correctionId, ownerId, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Atualiza uma correcao
    /// </summary>
    /// <param name="correctionId">Identificacao da correcao (GUID de 36 caracteres)</param>
    /// <param name="correction">Objeto contento a nova correcao</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPut("{correctionId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update([FromRoute] string correctionId, [FromBody] CorrectionUpdateRequest correction, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _service.Update(correctionId, correction, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }
}
