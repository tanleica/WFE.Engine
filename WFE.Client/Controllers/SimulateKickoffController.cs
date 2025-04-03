using Microsoft.AspNetCore.Mvc;
using WFE.Client.DTOs;
using WFE.Client.Persistence;
using WFE.Client.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace WFE.Client.Controllers;

[ApiController]
[Route("api/simulate")]
public class SimulateKickoffController(
    HttpClient httpClient,
    ClientDbContext db
    ) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ClientDbContext _db = db;

    [HttpPost("kickoff")]
    public async Task<IActionResult> KickoffAsync(KickoffRequestDto kickoff)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/ApprovalRouting/start", kickoff);

        var body = await response.Content.ReadAsStringAsync();
        return Content($"Engine responded with {response.StatusCode}:\n\n{body}", "text/plain");
    }

    [HttpPost("kickoff/{workflowId:guid}")]
    public async Task<IActionResult> KickoffFromTemplate(Guid workflowId)
    {
        var workflow = await _db.ClientWorkflows
            .Include(w => w.Steps)
                .ThenInclude(s => s.Actors)
            .FirstOrDefaultAsync(w => w.Id == workflowId);

        if (workflow == null)
            return NotFound($"Workflow ID not found: {workflowId}");

        var kickoff = new KickoffRequestDto
        {
            RequestedByUsername = "john",
            RequestedByFullName = "John Doe",
            RequestedByEmail = "john@example.com",
            RequestedByEmployeeCode = "EMP001",
            Reason = "Kickoff via Client DB Template",
            RequestedAt = DateTime.UtcNow,

            EncryptedConnectionString = "0gr8kJ0ZRWm/rmVj2kkrRXwn9vzRpzUQ+Dcgm5FNG1RYoTudpehA/d3tqHhhAOSV1Dn+MI1UwSjWrVJFnQJ7PQsBmeztzYk52k7QznOfeY64x/k60JmzVdVV4JeLfJ2jQ8Rq59pHAI0LoGyUMZiRQmmUp9e9stc2n4MesZeo8V4=",
            DbType = "SqlServer",

            Attributes = [], // you can later fetch real attributes here
            Steps = workflow.Steps.OrderBy(s => s.StepOrder).Select(s => new PlannedStepDto
            {
                StepName = s.StepName,
                StepOrder = s.StepOrder,
                ApprovalType = s.ApprovalType,
                RuleTree = new RuleNodeDto
                {
                    RuleName = s.StepName + " Rule",
                    ConditionScript = s.Condition,
                    LogicalOperator = LogicalOperators.Leaf,
                    FilterMode = s.FilterMode
                },
                Actors = s.Actors.Select(a => new PlannedActorDto
                {
                    Username = a.ActorUsername,
                    FullName = a.ActorFullName,
                    Email = a.ActorEmail,
                    EmployeeCode = a.ActorEmployeeCode
                }).ToList()
            }).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/ApprovalRouting/start", kickoff);
        var content = await response.Content.ReadAsStringAsync();

        return Ok(new
        {
            status = response.StatusCode,
            engineResponse = content
        });
    }

}
