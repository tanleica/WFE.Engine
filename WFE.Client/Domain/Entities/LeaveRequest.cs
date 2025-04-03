namespace WFE.Client.Domain.Entities
{
    public class LeaveRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RequestedBy { get; set; } = string.Empty;
        public int VacationDays { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}