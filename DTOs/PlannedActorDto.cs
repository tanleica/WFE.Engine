using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFE.Engine.DTOs
{
    public class PlannedActorDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
    }
}