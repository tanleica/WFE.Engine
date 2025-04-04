namespace WFE.Engine.Contracts
{
    public class Actor
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
    }
}