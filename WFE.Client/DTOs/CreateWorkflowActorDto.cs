public class CreateWorkflowActorDto
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
    public int Order { get; set; } = 1;
}
