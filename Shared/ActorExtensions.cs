using WFE.Engine.Contracts;

namespace WFE.Engine.Shared;

public static class ActorExtensions
{
    public static (string Username, string FullName, string Email, string EmployeeCode) Flatten(this Actor actor)
    {
        return (actor.Username, actor.FullName, actor.Email, actor.EmployeeCode);
    }

    // ✅ The Deconstruct(...) method enables C# tuple unpacking with elegance.
    public static void Deconstruct(this Actor actor, out string username, out string fullName, out string email, out string employeeCode)
    {
        username = actor.Username;
        fullName = actor.FullName;
        email = actor.Email;
        employeeCode = actor.EmployeeCode;
    }
}
