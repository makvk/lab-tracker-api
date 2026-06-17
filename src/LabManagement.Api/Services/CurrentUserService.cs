using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace LabManagement.Api.Services;

public class CurrentUserService(HttpContextAccessor _httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var claimGuidString = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => 
                claim.Type == ClaimTypes.NameIdentifier
            );
            if (claimGuidString == null)
            {
                return null;
            }
            return Guid.TryParse(claimGuidString.Value, out var id) ? id : null;
        }
    }
    public string? UserName
    {
        get
        { 
            var claimName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => 
                claim.Type == ClaimTypes.Name
            ); 
            if (claimName == null)
            {
                return null;
            }
            return claimName.Value;
        }
    }
    public string? Role
    {
        get
        { 
            var claimRole = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => 
                claim.Type == ClaimTypes.Role
            ); 
            if (claimRole == null)
            {
                return null;
            }
            return claimRole.Value;
        }
    }
    public bool IsAuth => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}