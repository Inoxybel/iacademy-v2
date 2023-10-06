using CrossCutting.Enums;
using Domain.DTO.Exercise;
using Domain.Entities.Exercise;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace IAcademyAPI.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/exercise")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    /// <summary>
    /// Recupera um exercicio
    /// </summary>
    /// <param name="exerciseId">Identificacao do exercicio (GUID de 36 caracteres)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Objeto com os exercicios</returns>
    [HttpGet("{exerciseId}")]
    [ProducesResponseType(typeof(Exercise), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Get([FromRoute] string exerciseId, CancellationToken cancellationToken)
    {
        var result = await _exerciseService.Get(exerciseId, cancellationToken);

        return result.Success ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Recupera os exercicios pelo OwnerId e Tipo
    /// </summary>
    /// <param name="ownerId">Identificacao do usuario (GUID de 36 caracteres)</param>
    /// <param name="type">Tipo de exercicio (Default ou Pendency)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de objetos com exercicios</returns>
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(List<Exercise>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> GetAllByOwnerIdAndType([FromRoute] ExerciseType type, CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirst("OwnerId")?.Value;

        if (string.IsNullOrEmpty(ownerId))
            return BadRequest("Invalid Token");

        var result = await _exerciseService.GetAllByOwnerIdAndType(ownerId, type, cancellationToken);

        return result.Success ? Ok(result.Data) : NotFound(result.ErrorMessage);
    }

    /// <summary>
    /// Criar um novo exercicio
    /// </summary>
    /// <param name="request">Objeto contendo os exercicios</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Identificacao do exercicio (GUID de 36 caracteres)</returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Save([FromBody] ExerciseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _exerciseService.Save(request, cancellationToken);

        return result.Success ? Created(string.Empty, result.Data) : BadRequest(result.ErrorMessage);
    }

    /// <summary>
    /// Atualiza um exercicio
    /// </summary>
    /// <param name="exerciseId">Identificacao do exercicio</param>
    /// <param name="request">Objeto contendo o novo exercicio</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    [HttpPut("{exerciseId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] string exerciseId, [FromBody] ExerciseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _exerciseService.Update(exerciseId, request, cancellationToken);

        return result.Success ? NoContent() : BadRequest(result.ErrorMessage);
    }
}
