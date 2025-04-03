using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFE.Engine.DTOs
{
    public class StepVoteDto
    {
        public Guid CorrelationId { get; set; }
        public string StepName { get; set; } = default!;        
        public string ActorUsername { get; set; } = default!;
        public string ActorFullName { get; set; } = default!;
        public string ActorEmail { get; set; } = default!;
        public string ActorEmployeeCode { get; set; } = default!;
        public bool IsApproved { get; set; }
        public string? Reason { get; set; }
    }
}