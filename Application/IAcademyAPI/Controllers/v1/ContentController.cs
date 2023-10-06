using CrossCutting.Helpers;
using Domain.DTO.Content;
using Domain.Entities.Contents;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/content")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }

    /// <summary>
    /// Recuperar um conteudo
    /// </summary>
    /// <param name="contentId">Identificacao do conteudo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>conteudo recuperado</returns>
    [HttpGet("{contentId}")]
    [ProducesResponseType(typeof(Content), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get(string contentId, CancellationToken cancellationToken = default)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _contentService.Get(contentId, cancellationToken);

        return result.Success ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Recuperar todos conteudos de um sumario
    /// </summary>
    /// <param name="summaryId">Identificacao do sumario (GUID de 36 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista com todos conteudos recuperados</returns>
    [HttpGet("summary/{summaryId}")]
    [ProducesResponseType(typeof(List<Content>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllBySummaryId(string summaryId, CancellationToken cancellationToken = default)
    {
        var result = await _contentService.GetAllBySummaryId(summaryId, cancellationToken);

        if (!result.Success || !result.Data.Any())
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Criar um novo conteudo
    /// </summary>
    /// <param name="request">Objeto contendo o conteudo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do conteudo criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Save([FromBody] ContentRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _contentService.Save(request, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Criar mais de um conteudo
    /// </summary>
    /// <param name="contents">Lista de objetos com os conteudos a serem criados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista com as identificacoes de todos conteudos criados</returns>
    [HttpPost("save-all")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> SaveAll([FromBody] List<ContentRequest> contents, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _contentService.SaveAll(contents, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Atualizar um conteudo
    /// </summary>
    /// <param name="contentId">Identificacao do conteudo (GUID de 36 caracteres)</param>
    /// <param name="request">Objeto com o conteudo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPut("{contentId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] string contentId, [FromBody] ContentRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _contentService.Update(contentId, request, cancellationToken);

        return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Atualizar mais de um conteudo
    /// </summary>
    /// <param name="summaryId">Identificacao do sumario (GUID de 36 caracteres)</param>
    /// <param name="contents">Lista de conteudos atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPut("update-all/summary/{summaryId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAll(string summaryId, [FromBody] List<Content> contents, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _contentService.UpdateAll(summaryId, contents, cancellationToken);

        return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
    }
}
