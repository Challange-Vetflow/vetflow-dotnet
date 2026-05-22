using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace VetFlow.API.Controllers;

/// <summary>Clínicas veterinárias parceiras da plataforma VetFlow.</summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ClinicController(IClinicRepository clinicRepository) : ControllerBase
{
    /// <summary>Lista todas as clínicas com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ClinicResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        var all = clinicRepository.GetAll();
        var paged = all.OrderBy(c => c.Name).Skip((page - 1) * size).Take(size)
            .Select(ClinicResponse.FromDomain).ToList();
        Response.Headers.Append("X-Total-Count", all.Count.ToString());
        return Ok(paged);
    }

    /// <summary>Busca clínica pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClinicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var clinic = clinicRepository.GetById(id);
        if (clinic is null) return NotFound();
        return Ok(ClinicResponse.FromDomain(clinic));
    }

    /// <summary>Cria uma nova clínica parceira.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClinicResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] ClinicRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (clinicRepository.ExistsByName(request.Name))
            return BadRequest("Já existe uma clínica com este nome.");
        try
        {
            var clinic = clinicRepository.Add(request.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, ClinicResponse.FromDomain(clinic));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Atualiza dados de uma clínica.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClinicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] ClinicRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var clinic = clinicRepository.GetById(id);
        if (clinic is null) return NotFound();
        try
        {
            clinic.Update(request.Name, request.Address, request.Phone, request.Email);
            clinicRepository.Update(clinic);
            return Ok(ClinicResponse.FromDomain(clinic));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Remove uma clínica pelo Id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!clinicRepository.Delete(id)) return NotFound();
        return NoContent();
    }
}
