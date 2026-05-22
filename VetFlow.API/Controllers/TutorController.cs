using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace VetFlow.API.Controllers;

/// <summary>Gerenciamento de tutores (responsáveis pelo pet).</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class TutorController(ITutorRepository tutorRepository) : ControllerBase
{
    /// <summary>Lista todos os tutores com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TutorResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string sort = "name")
    {
        var all = tutorRepository.GetAll();
        var sorted = sort.ToLower() switch
        {
            "email"     => all.OrderBy(t => t.Email),
            "createdat" => all.OrderBy(t => t.CreatedAt),
            _           => all.OrderBy(t => t.Name)
        };
        var paged = sorted.Skip((page - 1) * size).Take(size).Select(TutorResponse.FromDomain).ToList();
        Response.Headers.Append("X-Total-Count", all.Count.ToString());
        Response.Headers.Append("X-Page", page.ToString());
        Response.Headers.Append("X-Page-Size", size.ToString());
        return Ok(paged);
    }

    /// <summary>Busca tutor pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var tutor = tutorRepository.GetById(id);
        if (tutor is null) return NotFound();
        return Ok(TutorResponse.FromDomain(tutor));
    }

    /// <summary>Busca tutor pelo e-mail.</summary>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByEmail([FromQuery] string email)
    {
        var tutor = tutorRepository.GetByEmail(email);
        if (tutor is null) return NotFound();
        return Ok(TutorResponse.FromDomain(tutor));
    }

    /// <summary>Cria um novo tutor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] TutorRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (tutorRepository.ExistsByEmail(request.Email))
            return BadRequest("Já existe um tutor com este e-mail.");
        try
        {
            var tutor = tutorRepository.Add(request.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, TutorResponse.FromDomain(tutor));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Atualiza dados de um tutor.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] TutorRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var tutor = tutorRepository.GetById(id);
        if (tutor is null) return NotFound();
        try
        {
            tutor.UpdateName(request.Name);
            tutor.UpdatePhone(request.Phone);
            tutorRepository.Update(tutor);
            return Ok(TutorResponse.FromDomain(tutor));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Remove um tutor pelo Id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!tutorRepository.Delete(id)) return NotFound();
        return NoContent();
    }
}
