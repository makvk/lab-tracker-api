namespace LabManagement.Api.Services;

public interface ICurrentUserService
{
    Guid? UserId        { get; }
    string? Name    { get; }
    string? Role        { get; }
    bool IsAuth         { get; }

}