using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFE.Engine.DTOs
{
    public class RequestAttributeDto
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string ValueClrType { get; set; } = default!;
    }
}