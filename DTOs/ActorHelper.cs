using WFE.Engine.Contracts;

namespace WFE.Engine.DTOs;

public static class ActorHelper
{
    public static Actor ToActor(Guid userId, string username, string fullName, string email, string employeeCode)
        => new()
        {
            Id = userId,
            Username = username,
            FullName = fullName,
            Email = email,
            EmployeeCode = employeeCode
        };

}