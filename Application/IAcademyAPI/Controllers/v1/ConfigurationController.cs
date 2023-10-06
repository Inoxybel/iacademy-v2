using CrossCutting.Helpers;
using Domain.DTO.Configuration;
using Domain.Entities.Configuration;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/configurations")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    /// <summary>
    /// Recuperar uma configuracao
    /// </summary>
    /// <param name="configurationId">Identificacao da configuracao (GUID de 36 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Objeto contendo as configuracoes</returns>
    [HttpGet("{configurationId}")]
    [ProducesResponseType(typeof(Configuration), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get(string configurationId, CancellationToken cancellationToken = default)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _configurationService.Get(configurationId, cancellationToken);

        return result.Success ? Ok(result.Data) : NotFound();
    }

    /// <summary>
    /// Criar nova configuracao
    /// </summary>
    /// <param name="configurationRequest">Objeto de configuracao</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Create([FromBody] ConfigurationRequest configurationRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _configurationService.Create(configurationRequest, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Atualizar uma configuracao
    /// </summary>
    /// <param name="configurationId">Identificacao da configuracao (GUID de 36 caracteres)</param>
    /// <param name="configurationRequest">Objeto com as novas configuracoes</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPut("{configurationId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string configurationId, [FromBody] ConfigurationRequest configurationRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _configurationService.Update(configurationId, configurationRequest, cancellationToken);

        return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
    }
}
