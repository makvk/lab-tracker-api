using LabManagement.App.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.App.Features.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly ILabDbContext _context;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(ILabDbContext context, IJwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Сначала ищем в преподавателях
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Email == request.Email, cancellationToken);

        if (teacher != null && teacher.PasswordHash == request.Password)
        {
            var token = _tokenGenerator.GenerateToken(teacher.Id, teacher.Email, "Teacher", teacher.FirstName);
            return new AuthResult(token, teacher.Id, teacher.Email, "Teacher", teacher.FirstName, teacher.LastName);
        }

        // Если не нашли, ищем в студентах
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Email == request.Email, cancellationToken);

        if (student != null && student.PasswordHash == request.Password)
        {
            var token = _tokenGenerator.GenerateToken(student.Id, student.Email, "Student", student.FirstName);
            return new AuthResult(token, student.Id, student.Email, "Student", student.FirstName, student.LastName);
        }

        throw new UnauthorizedAccessException("Неверный Email или пароль");
    }
}