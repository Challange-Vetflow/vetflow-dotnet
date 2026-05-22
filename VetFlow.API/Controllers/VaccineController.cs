using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace VetFlow.API.Controllers;

/// <summary>Registro de vacinas aplicadas nos pets.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class VaccineController(IVaccineRepository vaccineRepository, IPetRepository petRepository) : ControllerBase
{
    /// <summary>Lista todas as vacinas com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VaccineResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var all = vaccineRepository.GetAll();
        var paged = all.OrderBy(v => v.NextDoseAt).Skip((page - 1) * size).Take(size)
            .Select(VaccineResponse.FromDomain).ToList();
        Response.Headers.Append("X-Total-Count", all.Count.ToString());
        return Ok(paged);
    }

    /// <summary>Busca vacina pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VaccineResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var v = vaccineRepository.GetById(id);
        if (v is null) return NotFound();
        return Ok(VaccineResponse.FromDomain(v));
    }

    /// <summary>Lista vacinas de um pet.</summary>
    [HttpGet("by-pet/{petId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<VaccineResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByPet(Guid petId)
    {
        var list = vaccineRepository.GetByPetId(petId).Select(VaccineResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Lista vacinas vencidas.</summary>
    [HttpGet("expired")]
    [ProducesResponseType(typeof(IReadOnlyList<VaccineResponse>), StatusCodes.Status200OK)]
    public IActionResult GetExpired()
    {
        var list = vaccineRepository.GetExpired().Select(VaccineResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Registra uma vacina aplicada em um pet.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VaccineResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] VaccineRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!petRepository.ExistsById(request.PetId)) return BadRequest("Pet não encontrado.");
        try
        {
            var v = vaccineRepository.Add(request.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = v.Id }, VaccineResponse.FromDomain(v));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Atualiza dados de uma vacina.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VaccineResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] VaccineRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var v = vaccineRepository.GetById(id);
        if (v is null) return NotFound();
        try
        {
            v.Update(request.VaccineName, request.AppliedAt, request.NextDoseAt, request.Batch);
            vaccineRepository.Update(v);
            return Ok(VaccineResponse.FromDomain(v));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Remove um registro de vacina.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!vaccineRepository.Delete(id)) return NotFound();
        return NoContent();
    }
}
