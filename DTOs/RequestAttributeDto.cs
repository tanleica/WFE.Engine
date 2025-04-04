namespace WFE.Engine.DTOs
{
    public class RequestAttributeDto
    {
        public string Key { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string ValueClrType { get; set; } = default!;
    }
}