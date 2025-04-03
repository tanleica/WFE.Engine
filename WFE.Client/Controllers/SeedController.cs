using Microsoft.AspNetCore.Mvc;
using WFE.Client.Persistence;
using WFE.Client.Domain.Entities;

namespace WFE.Client.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController(ClientDbContext db) : ControllerBase
{

    private readonly ClientDbContext _db = db;

    [HttpPost("leave-request")]
    public async Task<IActionResult> AddDemoLeaveRequest()
    {
        var demoRequest = new LeaveRequest
        {
            RequestedBy = "john",
            VacationDays = 5
        };

        await _db.LeaveRequests.AddAsync(demoRequest);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Demo leave request added",
            id = demoRequest.Id,
            requestedBy = demoRequest.RequestedBy,
            vacationDays = demoRequest.VacationDays
        });
    }

    [HttpPost("workflow")]
    public async Task<IActionResult> SaveWorkflow([FromBody] ClientWorkflow dto)
    {
        if (dto.Steps is null || dto.Steps.Count == 0)
            return BadRequest("Workflow must have at least one step.");

        foreach (var step in dto.Steps)
        {
            if (step.Actors is null || step.Actors.Count == 0)
                return BadRequest($"Step '{step.StepName}' must have at least one actor.");
        }

        _db.ClientWorkflows.Add(dto);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            Success = true,
            Message = $"✅ Workflow '{dto.Name}' saved with {dto.Steps.Count} step(s).",
            dto.Id
        });
    }

}