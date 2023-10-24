using CrossCutting.Enums;
using CrossCutting.Extensions;
using CrossCutting.Helpers;
using Domain.DTO;
using Domain.DTO.Content;
using Domain.DTO.Correction;
using Domain.DTO.Summary;
using Domain.Entities.Feedback;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Route("api/ai")]
[Authorize]
[ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class AIController : ControllerBase
{
    private readonly ISummaryService _summaryService;
    private readonly IContentService _contentService;
    private readonly IExerciseService _exerciseService;
    private readonly ICorrectionService _correctionService;

    public AIController(
        ISummaryService summaryService,
        IContentService contentService,
        IExerciseService exerciseService,
        ICorrectionService correctionService)
    {
        _summaryService = summaryService;
        _contentService = contentService;
        _exerciseService = exerciseService;
        _correctionService = correctionService;
    }

    /// <summary>
    /// Criar uma nova base de treinamento em formato de sumario
    /// </summary>
    /// <param name="request">Objeto de requisição com parametros de configuracoes</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do sumario criado, persistido no banco de dados</returns>
    [HttpPost("summary/create")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<ActionResult<ServiceResult<SummaryResponse>>> MakeSummary([FromBody] SummaryCreationRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (!MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _summaryService.RequestCreationToAI(request, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Criar um novo conteudo didatico (principal e exercicios) baseado no índice informado
    /// </summary>
    /// <param name="summaryId">Identificacao do sumario (GUID de 36 caracteres)</param>
    /// <param name="request">Objeto que contém o número do índice que deseja que o conteudo seja criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do conteudo criado, persistido no banco de dados</returns>
    [HttpPost("summary/{summaryId}/create-content-by-subtopic")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> RequestContentCreationToAI([FromRoute] string summaryId, [FromBody] AIContentCreationRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (!MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _contentService.MakeContent(summaryId, request, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Criar um novo exemplo/explicacao alternativo para parte de um conteudo
    /// </summary>
    /// <param name="contentId">Identificacao do conteudo (GUID de 36 caracteres)</param>
    /// <param name="request">Objeto que contém o número de Identificacao da parte que sera alterada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do conteudo alterado, persistido no banco de dados</returns>
    [HttpPost("content/{contentId}/new-content")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MakeNewAlternativeContent(string contentId, SubcontentRecreationRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;
        var textGenres = User.FindFirst("TextGenres")?.Value;

        if (string.IsNullOrEmpty(ownerId) || string.IsNullOrEmpty(textGenres))
            return BadRequest("Invalid Token");

        var listTextGenres = textGenres.Deserialize<List<TextGenre>>();

        var result = await _contentService.MakeAlternativeContent(contentId, request, listTextGenres, cancellationToken);

        return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Efetua a criacao de exercicios para um conteudo
    /// </summary>
    /// <param name="contentId">Identificacao do conteudo (GUID de 36 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do exercicio criado, persistido no banco de dados</returns>
    [HttpPost("content/{contentId}/request-exercise-creation")]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    public async Task<IActionResult> RequestExerciseCreation([FromRoute] string contentId, CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (!MasterOwner.Validate(ownerId))
            return BadRequest("Invalid Token");

        var result = await _exerciseService.MakeExercise(contentId, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Solicita correcao do exercicio informado
    /// </summary>
    /// <param name="exerciseId">Identificacao do exercicio (GUID de 36 caracteres)</param>
    /// <param name="request">Objeto contendo informacoes dos exercicios e respostas</param>
    /// <returns>Objeto de correcao do exercicio</returns>
    [HttpPost("exercise/{exerciseId}/request-correction")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Correction), StatusCodes.Status201Created)]
    public async Task<IActionResult> MakeCorrection([FromRoute] string exerciseId, [FromBody] CreateCorrectionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _correctionService.MakeCorrection(exerciseId, ownerId, request);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }
}
