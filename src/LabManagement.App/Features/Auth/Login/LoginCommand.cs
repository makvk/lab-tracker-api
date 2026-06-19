using MediatR;

namespace LabManagement.App.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

public record AuthResult(
    string Token, 
    Guid UserId, 
    string Email, 
    string Role, 
    string FirstName, 
    string LastName
);