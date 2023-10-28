using Domain.DTO.Summary;
using Domain.DTO;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using CrossCutting.Helpers;
using Domain.Entities.Summary;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/summary")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Recupera um sumario
    /// </summary>
    /// <param name="id">Identificacao do sumario (GUID de 36 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Objeto de sumario</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Summary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ServiceResult<Summary>>> Get(string id, CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.Get(id, cancellationToken);

        return result.Success ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Recupera todos sumarios por categoria
    /// </summary>
    /// <param name="category">Categoria do treinamento</param>
    /// <param name="isAvailable">Esta disponivel? Padrao: true</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de sumarios</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(PaginatedResult<Summary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PaginatedResult<Summary>>> GetAllByCategory(
        [FromQuery] PaginationRequest pagination,
        string category,
        bool isAvailable = true,
        CancellationToken cancellationToken = default)
    {
        var document = User.FindFirst("Document")?.Value;
        var companyRef = User.FindFirst("CompanyRef")?.Value;

        if (string.IsNullOrEmpty(document) || string.IsNullOrEmpty(companyRef))
            return BadRequest("Invalid Token");

        var result = await _summaryService.GetAllByCategory(pagination, category, document, companyRef, isAvailable, cancellationToken);

        return result.Success ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Recupera todos sumarios por categoria e subcategoria
    /// </summary>
    /// <param name="category">Categoria do treinamento</param>
    /// <param name="subcategory">Subcategoria do treinamento</param>
    /// <param name="isAvailable">Esta disponivel? Padrao: true</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de sumarios</returns>
    [HttpGet("category/{category}/subcategory/{subcategory}")]
    [ProducesResponseType(typeof(PaginatedResult<Summary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PaginatedResult<Summary>>> GetAllByCategoryAndSubcategory(
        [FromQuery] PaginationRequest pagination, 
        string category, 
        string subcategory, 
        bool isAvailable = true,
        CancellationToken cancellationToken = default)
    {
        var document = User.FindFirst("Document")?.Value;
        var companyRef = User.FindFirst("CompanyRef")?.Value;

        if (string.IsNullOrEmpty(document) || string.IsNullOrEmpty(companyRef))
            return BadRequest("Invalid Token");

        var result = await _summaryService.GetAllByCategoryAndSubcategory(pagination, category, subcategory, document, companyRef, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Recupera todos treinamentos que o usuario esta matriculado
    /// </summary>
    /// <param name="isAvailable">Esta disponivel? Padrao: true</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de sumarios</returns>
    [HttpGet("enrolled")]
    [ProducesResponseType(typeof(PaginatedResult<Summary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PaginatedResult<Summary>>> GetAllByOwnerId(
        [FromQuery] PaginationRequest pagination, 
        bool isAvailable = true,
        CancellationToken cancellationToken = default)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.GetAllByOwnerId(pagination, ownerId, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Recupera todos treinamentos disponiveis para o usuario
    /// </summary>
    /// <param name="isAvailable">Esta disponivel? Padrao: true</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de sumarios</returns>
    [HttpGet("available")]
    [ProducesResponseType(typeof(PaginatedResult<Summary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PaginatedResult<Summary>>> GetAllAvaliableByDocument(
        [FromQuery] PaginationRequest pagination,
        bool isAvailable = true, 
        CancellationToken cancellationToken = default)
    {
        var document = User.FindFirst("Document")?.Value;
        var companyRef = User.FindFirst("CompanyRef")?.Value;
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(document) || string.IsNullOrEmpty(companyRef) || string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.GetAllAvaliableByDocument(pagination, ownerId, document, companyRef, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Recupera todos treinamentos da empresa
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de sumarios</returns>
    [HttpGet("company/available")]
    [ProducesResponseType(typeof(PaginatedResult<SummaryResumeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<PaginatedResult<SummaryResumeResponse>>> GetAllAvaliableToCompany(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.GetAllAvailableToCompany(pagination, ownerId, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Recupera todos sumarios por subcategoria
    /// </summary>
    /// <param name="subcategory">Subcategoria do treinamento</param>
    /// <param name="isAvailable">Esta disponivel? Padrao: true</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de sumarios</returns>
    [HttpGet("subcategory/{subcategory}")]
    [ProducesResponseType(typeof(PaginatedResult<Summary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ServiceResult<Summary>>> GetAllBySubcategory(
        [FromQuery] PaginationRequest pagination, 
        string subcategory, 
        bool isAvailable = true,
        CancellationToken cancellationToken = default)
    {
        var document = User.FindFirst("Document")?.Value;
        var companyRef = User.FindFirst("CompanyRef")?.Value;

        if (string.IsNullOrEmpty(document) || string.IsNullOrEmpty(companyRef))
            return BadRequest("Invalid Token");

        var result = await _summaryService.GetAllBySubcategory(pagination, subcategory, document, companyRef, isAvailable, cancellationToken);

        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }

    /// <summary>
    /// Cria um novo sumario
    /// </summary>
    /// <param name="request">Objeto contendo o sumario</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do sumario criado, persistido no banco de dados</returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> Save([FromBody] SummaryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (!MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.Save(request, string.Empty, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Created(string.Empty, result.Data);
    }

    /// <summary>
    /// Efetua a matricula de um usuario em um treinamento
    /// </summary>
    /// <param name="request">Objeto contendo informacoes do treinamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPost("enroll")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> EnrollUser([FromBody] SummaryMatriculationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;
        var document = User.FindFirst("Document")?.Value;
        var companyRef = User.FindFirst("CompanyRef")?.Value;

        if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(companyRef) || string.IsNullOrEmpty(document))
            return BadRequest("Invalid Token");

        var newContentResult = await _contentService.CopyContentsToEnrollUser(request, ownerId, companyRef, document, cancellationToken);

        if (!newContentResult.Success)
            return BadRequest(newContentResult.ErrorMessage);

        return Created(string.Empty, newContentResult.Data);
    }

    /// <summary>
    /// Atualiza um sumario
    /// </summary>
    /// <param name="summaryId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPut("{summaryId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> Update(string summaryId, [FromBody] SummaryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (!MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.Update(summaryId, request, cancellationToken);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return NoContent();
    }        
}
