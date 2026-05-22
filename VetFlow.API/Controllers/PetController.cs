using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace VetFlow.API.Controllers;

/// <summary>Gerenciamento de pets cadastrados na plataforma.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PetController(IPetRepository petRepository, ITutorRepository tutorRepository) : ControllerBase
{
    /// <summary>Lista todos os pets com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string sort = "name")
    {
        var all = petRepository.GetAll();
        var sorted = sort.ToLower() switch
        {
            "species"   => all.OrderBy(p => p.Species),
            "createdat" => all.OrderBy(p => p.CreatedAt),
            _           => all.OrderBy(p => p.Name)
        };
        var paged = sorted.Skip((page - 1) * size).Take(size).Select(PetResponse.FromDomain).ToList();
        Response.Headers.Append("X-Total-Count", all.Count.ToString());
        Response.Headers.Append("X-Page", page.ToString());
        Response.Headers.Append("X-Page-Size", size.ToString());
        return Ok(paged);
    }

    /// <summary>Busca pet pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var pet = petRepository.GetById(id);
        if (pet is null) return NotFound();
        return Ok(PetResponse.FromDomain(pet));
    }

    /// <summary>Lista pets de um tutor.</summary>
    [HttpGet("by-tutor/{tutorId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByTutor(Guid tutorId)
    {
        var list = petRepository.GetByTutorId(tutorId).Select(PetResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Cria um pet vinculado a um tutor.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] PetRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!tutorRepository.ExistsById(request.TutorId))
            return BadRequest("Tutor não encontrado.");
        try
        {
            var pet = petRepository.Add(request.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, PetResponse.FromDomain(pet));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Atualiza dados de um pet.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(Guid id, [FromBody] PetRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var pet = petRepository.GetById(id);
        if (pet is null) return NotFound();
        try
        {
            pet.Update(request.Name, request.Species, request.Breed, request.BirthDate, request.WeightKg);
            petRepository.Update(pet);
            return Ok(PetResponse.FromDomain(pet));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Remove um pet pelo Id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!petRepository.Delete(id)) return NotFound();
        return NoContent();
    }
}
