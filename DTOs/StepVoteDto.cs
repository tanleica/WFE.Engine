using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFE.Engine.Contracts;

namespace WFE.Engine.DTOs
{
    public class StepVoteDto
    {
        public Guid CorrelationId { get; set; }
        public string StepName { get; set; } = default!;

        public Actor Actor {get; set;} = new();
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }
}