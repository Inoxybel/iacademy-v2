using Domain.DTO.Configuration;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/configurations")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpGet("{configurationId}")]
    public async Task<IActionResult> Get(string configurationId, CancellationToken cancellationToken = default)
    {
        var result = await _configurationService.Get(configurationId, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConfigurationRequest configurationRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _configurationService.Create(configurationRequest, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    [HttpPut("{configurationId}")]
    public async Task<IActionResult> Update(string configurationId, [FromBody] ConfigurationRequest configurationRequest, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _configurationService.Update(configurationId, configurationRequest, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }
}
