using VetFlow.Application.DTOs;
using VetFlow.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace VetFlow.API.Controllers;

/// <summary>
/// Consultas e agendamentos dos pets nas clínicas parceiras.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AppointmentController(
    IAppointmentRepository appointmentRepository,
    IPetRepository petRepository,
    IClinicRepository clinicRepository) : ControllerBase
{
    /// <summary>Lista todos os agendamentos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var list = appointmentRepository.GetAll().Select(AppointmentResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Busca agendamento pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var a = appointmentRepository.GetById(id);
        if (a is null) return NotFound();
        return Ok(AppointmentResponse.FromDomain(a));
    }

    /// <summary>Lista agendamentos de um pet.</summary>
    [HttpGet("by-pet/{petId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByPet(Guid petId)
    {
        var list = appointmentRepository.GetByPetId(petId).Select(AppointmentResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Lista agendamentos pendentes (não concluídos e futuros).</summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(IReadOnlyList<AppointmentResponse>), StatusCodes.Status200OK)]
    public IActionResult GetPending()
    {
        var list = appointmentRepository.GetPending().Select(AppointmentResponse.FromDomain).ToList();
        return Ok(list);
    }

    /// <summary>Cria um novo agendamento.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] AppointmentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!petRepository.ExistsById(request.PetId)) return BadRequest("Pet não encontrado.");
        if (!clinicRepository.ExistsById(request.ClinicId)) return BadRequest("Clínica não encontrada.");
        try
        {
            var a = appointmentRepository.Add(request.ToDomain());
            return CreatedAtAction(nameof(GetById), new { id = a.Id }, AppointmentResponse.FromDomain(a));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Atualiza dados de um agendamento (clínica, horário, tipo e observações).</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] AppointmentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var a = appointmentRepository.GetById(id);
        if (a is null) return NotFound();
        if (!clinicRepository.ExistsById(request.ClinicId)) return BadRequest("Clínica não encontrada.");
        try
        {
            a.Update(request.ClinicId, request.ScheduledAt, request.Type, request.Notes);
            appointmentRepository.Update(a);
            return Ok(AppointmentResponse.FromDomain(a));
        }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>Marca um agendamento como concluído.</summary>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Complete(Guid id)
    {
        var a = appointmentRepository.GetById(id);
        if (a is null) return NotFound();
        a.MarkCompleted();
        appointmentRepository.Update(a);
        return Ok(AppointmentResponse.FromDomain(a));
    }

    /// <summary>Remove um agendamento pelo Id.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!appointmentRepository.Delete(id)) return NotFound();
        return NoContent();
    }
}
