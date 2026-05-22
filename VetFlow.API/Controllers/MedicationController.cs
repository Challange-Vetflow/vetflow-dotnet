using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace VetFlow.API.Controllers;

/// <summary>
/// Medicamentos prescritos aos pets — controle de adesão terapêutica.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class MedicationController(IMedicationRepository medicationRepository, IPetRepository petRepository) : ControllerBase
{
    /// <summary>Lista todos os medicamentos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MedicationResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var list = medicationRepository.GetAll().Select(MedicationResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Busca medicamento pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MedicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var m = medicationRepository.GetById(id);
        if (m is null) return NotFound();
        return Ok(MedicationResponse.FromDomain(m));
    }

    /// <summary>Lista medicamentos de um pet.</summary>
    [HttpGet("by-pet/{petId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<MedicationResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByPet(Guid petId)
    {
        var list = medicationRepository.GetByPetId(petId).Select(MedicationResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Lista medicamentos ativos.</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<MedicationResponse>), StatusCodes.Status200OK)]
    public IActionResult GetActive()
    {
        var list = medicationRepository.GetActive().Select(MedicationResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Prescreve um novo medicamento para um pet.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MedicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] MedicationRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!petRepository.ExistsById(request.PetId)) return BadRequest("Pet não encontrado.");
        try
        {
            var m = medicationRepository.Add(request.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = m.Id }, MedicationResponse.FromDomain(m));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Atualiza dados de um medicamento (nome, dosagem, frequência e datas).</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MedicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] MedicationRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var m = medicationRepository.GetById(id);
        if (m is null) return NotFound();
        try
        {
            m.Update(request.Name, request.Dosage, request.Frequency, request.StartDate, request.EndDate);
            medicationRepository.Update(m);
            return Ok(MedicationResponse.FromDomain(m));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Suspende um medicamento ativo.</summary>
    [HttpPut("{id:guid}/suspend")]
    [ProducesResponseType(typeof(MedicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Suspend(Guid id)
    {
        var m = medicationRepository.GetById(id);
        if (m is null) return NotFound();
        m.Suspend();
        medicationRepository.Update(m);
        return Ok(MedicationResponse.FromDomain(m));
    }

    /// <summary>Marca medicamento como concluído.</summary>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(typeof(MedicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Complete(Guid id)
    {
        var m = medicationRepository.GetById(id);
        if (m is null) return NotFound();
        m.Complete();
        medicationRepository.Update(m);
        return Ok(MedicationResponse.FromDomain(m));
    }

    /// <summary>Remove um medicamento pelo Id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!medicationRepository.Delete(id)) return NotFound();
        return NoContent();
    }
}
