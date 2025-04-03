namespace WFE.Engine.Contracts;

public interface IRequestApproved
{
    Guid CorrelationId { get; }
    Guid WorkflowId { get; }
    DateTime ApprovedAt { get; }
    string FinalStepName { get; }

    string FinalApprovedByUsername { get; }
    string FinalApprovedByFullName { get; }
    string FinalApprovedByEmail { get; }
    string FinalApprovedByEmployeeCode { get; }
 
}
